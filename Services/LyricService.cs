namespace Services
{
  using Common;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using Config;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
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
      string sqlStatement = "select * from lyrics where id = @id";
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
            lyric.ModifiedAt = reader[5] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[5]);
            lyric.IsDeleted = Convert.ToBoolean(reader[6]);
            lyric.IsApproved = Convert.ToBoolean(reader[7]);
            lyric.ArtistId = Convert.ToInt32(reader[8]);
            lyric.AuthorId = reader[9] == DBNull.Value ? (int?)null : Convert.ToInt32(reader[9]);
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
            lyric.ModifiedAt = reader[5] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[5]);
            lyric.IsDeleted = Convert.ToBoolean(reader[6]);
            lyric.IsApproved = Convert.ToBoolean(reader[7]);
            lyric.ArtistId = Convert.ToInt32(reader[8]);
            lyric.AuthorId = reader[9] == DBNull.Value ? (int?)null : Convert.ToInt32(reader[9]);

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

    public async Task EditLyricAsync(LyricEditViewModel editedLyric)
    {
      IEnumerable<LyricSlugViewModel> lyricSlugs = await _lyricSlugService.GetSlugsForLyricAsync(editedLyric.Id);

      string updatedSlug = editedLyric.Title.NormalizeStringForUrl();

      bool slugDoesNotExistAlready = lyricSlugs.All(s => s.Name != updatedSlug);

      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update lyrics set title = @title, body = @body where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@id", editedLyric.Id);
        command.Parameters.AddWithValue("@title", editedLyric.Title);
        command.Parameters.AddWithValue("@body", editedLyric.Body);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();
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

            NpgsqlDataReader reader = await command.ExecuteReaderAsync();
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
  }
}
