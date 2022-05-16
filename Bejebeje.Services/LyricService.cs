namespace Bejebeje.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Common;
  using Config;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using ViewModels.Lyric;
  using ViewModels.LyricSlug;

  public class LyricService : ILyricService
  {
    private readonly DatabaseOptions _databaseOptions;

    private readonly IArtistService _artistService;

    private readonly ILyricSlugService _lyricSlugService;

    public LyricService(
      IOptionsMonitor<DatabaseOptions> optionsAccessor,
      IArtistService artistService,
      ILyricSlugService lyricSlugService)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
      _artistService = artistService;
      _lyricSlugService = lyricSlugService;
    }

    public async Task<LyricViewModel> GetLyricByIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select l.id, l.title, l.body, l.user_id, l.created_at, l.modified_at, l.is_deleted, l.is_approved, l.artist_id, l.author_id, l.is_verified from lyrics l where id = @id";
      LyricViewModel lyric = null;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@id", id);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            lyric = new LyricViewModel();
            lyric.Id = Convert.ToInt32(reader[0]);
            lyric.Title = Convert.ToString(reader[1]);
            lyric.Body = Convert.ToString(reader[2]);
            lyric.UserId = Convert.ToString(reader[3]);
            lyric.CreatedAt = Convert.ToDateTime(reader[4]);
            lyric.ModifiedAt = reader[5] == DBNull.Value ? null : Convert.ToDateTime(reader[5]);
            lyric.IsDeleted = Convert.ToBoolean(reader[6]);
            lyric.IsApproved = Convert.ToBoolean(reader[7]);
            lyric.ArtistId = Convert.ToInt32(reader[8]);
            lyric.AuthorId = reader[9] == DBNull.Value ? null : Convert.ToInt32(reader[9]);
            lyric.IsVerified = Convert.ToBoolean(reader[10]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return lyric;
    }

    public async Task<ArtistLyricsViewModel> GetLyricsForArtistAsync(int artistId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from lyrics where artist_id = @artist_id order by title";
      ArtistLyricsViewModel model = new ArtistLyricsViewModel();
      List<LyricViewModel> lyrics = new List<LyricViewModel>();

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@artist_id", artistId);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            LyricViewModel lyric = new LyricViewModel();
            lyric.Id = Convert.ToInt32(reader[0]);
            lyric.Title = Convert.ToString(reader[1]);
            lyric.Body = Convert.ToString(reader[2]);
            lyric.UserId = Convert.ToString(reader[3]);
            lyric.CreatedAt = Convert.ToDateTime(reader[4]);
            lyric.ModifiedAt = reader[5] == DBNull.Value ? null : Convert.ToDateTime(reader[5]);
            lyric.IsDeleted = Convert.ToBoolean(reader[6]);
            lyric.IsApproved = Convert.ToBoolean(reader[7]);
            lyric.ArtistId = Convert.ToInt32(reader[8]);
            lyric.AuthorId = reader[9] == DBNull.Value ? null : Convert.ToInt32(reader[9]);

            lyrics.Add(lyric);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      var artist = await _artistService.GetArtistByIdAsync(artistId);

      model.Lyrics = lyrics;
      model.Artist = artist;

      return model;
    }

    public async Task<int> AddLyricAsync(LyricCreateViewModel newLyric)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "insert into lyrics (title, body, user_id, created_at, is_deleted, is_approved, artist_id, is_verified) values (@title, @body, @user_id, @created_at, @is_deleted, @is_approved, @artist_id, @is_verified) returning id";
      int lyricId = 0;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        string title = newLyric.Title.ToLower().ToSentenceCase();
        string body = newLyric.Body;
        string userId = newLyric.UserId;
        DateTime createdAt = DateTime.UtcNow;
        bool isDeleted = false;
        bool isApproved = true;
        int artistId = newLyric.ArtistId;
        bool isVerified = newLyric.IsVerified;

        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@body", body);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@created_at", createdAt);
        command.Parameters.AddWithValue("@is_deleted", isDeleted);
        command.Parameters.AddWithValue("@is_approved", isApproved);
        command.Parameters.AddWithValue("@artist_id", artistId);
        command.Parameters.AddWithValue("@is_verified", isVerified);

        try
        {
          await connection.OpenAsync();

          object lyricIdentity = command.ExecuteScalar();
          lyricId = (int)lyricIdentity;

          LyricSlugCreateViewModel lyricSlug = new LyricSlugCreateViewModel();
          lyricSlug.Name = title.NormalizeStringForUrl();
          lyricSlug.IsPrimary = true;
          lyricSlug.IsDeleted = false;
          lyricSlug.CreatedAt = createdAt;
          lyricSlug.LyricId = lyricId;

          await _lyricSlugService.AddLyricSlugAsync(lyricSlug);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return lyricId;
    }

    public async Task EditLyricAsync(LyricEditViewModel editedLyric)
    {
      IEnumerable<LyricSlugViewModel> lyricSlugs = await _lyricSlugService.GetSlugsForLyricAsync(editedLyric.Id);

      string updatedSlug = editedLyric.Title.NormalizeStringForUrl();
      DateTime modifiedAt = DateTime.UtcNow;

      bool slugDoesNotExistAlready = lyricSlugs.All(s => s.Name != updatedSlug);

      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update lyrics set title = @title, body = @body, is_approved = @is_approved, is_verified = @is_verified, is_deleted = @is_deleted, modified_at = @modified_at where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@id", editedLyric.Id);
        command.Parameters.AddWithValue("@title", editedLyric.Title);
        command.Parameters.AddWithValue("@body", editedLyric.Body);
        command.Parameters.AddWithValue("@is_approved", editedLyric.IsApproved);
        command.Parameters.AddWithValue("@is_verified", editedLyric.IsVerified);
        command.Parameters.AddWithValue("@is_deleted", editedLyric.IsDeleted);
        command.Parameters.AddWithValue("modified_at", modifiedAt);

        try
        {
          await connection.OpenAsync();

          await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      if (slugDoesNotExistAlready)
      {
        await _lyricSlugService.MarkIsPrimaryAsFalseForAllLyricSlugs(editedLyric.Id);

        string sqlStatementToAddNewLyricSlug = "insert into lyric_slugs (name, is_primary, created_at, is_deleted, lyric_id) values(@name, @is_primary, @created_at, @is_deleted, @lyric_id)";

        int lyricId = editedLyric.Id;
        bool isPrimary = true;
        bool isDeleted = false;
        DateTime createdAt = DateTime.UtcNow;

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
          NpgsqlCommand command = new NpgsqlCommand(sqlStatementToAddNewLyricSlug, connection);
          command.Parameters.AddWithValue("@name", editedLyric.Title.NormalizeStringForUrl());
          command.Parameters.AddWithValue("@is_primary", isPrimary);
          command.Parameters.AddWithValue("@created_at", createdAt);
          command.Parameters.AddWithValue("@is_deleted", isDeleted);
          command.Parameters.AddWithValue("@lyric_id", lyricId);

          try
          {
            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
      }
      else
      {
        await _lyricSlugService.MarkIsPrimaryAsFalseForAllLyricSlugs(editedLyric.Id);

        LyricSlugViewModel existingLyricSlug = lyricSlugs.Single(s => s.Name == updatedSlug);

        await _lyricSlugService.MakeLyricSlugPrimaryAsync(existingLyricSlug.Id);
      }
    }

    public async Task<LyricsAwaitingApprovalViewModel> GetLyricsAwaitingApproval()
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select l.id as lyric_id, l.title as lyric_title, a.id as artist_id, a.full_name as artist_name, l.user_id as submitted_by_user_id, l.created_at, l.modified_at from lyrics l left join artists a on a.id = l.artist_id where l.is_deleted = false and l.is_approved = false;";
      LyricsAwaitingApprovalViewModel model = new LyricsAwaitingApprovalViewModel();
      List<LyricApprovalItemViewModel> lyricItems = new List<LyricApprovalItemViewModel>();

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            LyricApprovalItemViewModel lyric = new LyricApprovalItemViewModel();
            lyric.LyricId = Convert.ToInt32(reader[0]);
            lyric.LyricTitle = Convert.ToString(reader[1]);
            lyric.ArtistId = Convert.ToInt32(reader[2]);
            lyric.ArtistName = Convert.ToString(reader[3]);
            lyric.SubmittedBy = "Cano"; // Convert.ToString(reader[2]);
            lyric.CreatedAt = Convert.ToDateTime(reader[5]);
            lyric.ModifiedAt = reader[6] == DBNull.Value ? null : Convert.ToDateTime(reader[6]);

            lyricItems.Add(lyric);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      model.Lyrics = lyricItems;
      
      return model;
    }
  }
}
