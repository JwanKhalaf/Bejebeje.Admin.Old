namespace Services
{
  using Common;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using Services.Config;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using ViewModels.Lyric;
  using ViewModels.LyricSlug;

  public class LyricService : ILyricService
  {
    private readonly DatabaseOptions databaseOptions;

    private readonly IArtistService artistService;

    private readonly ILyricSlugService lyricSlugService;

    public LyricService(
      IOptionsMonitor<DatabaseOptions> optionsAccessor,
      IArtistService artistService,
      ILyricSlugService lyricSlugService)
    {
      databaseOptions = optionsAccessor.CurrentValue;
      this.artistService = artistService;
      this.lyricSlugService = lyricSlugService;
    }

    public async Task<LyricViewModel> GetLyricByIdAsync(int id)
    {
      string connectionString = databaseOptions.ConnectionString;
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
      string connectionString = databaseOptions.ConnectionString;
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

      var artist = await artistService.GetArtistByIdAsync(artistId);

      model.Lyrics = lyrics;
      model.Artist = artist;

      return model;
    }

    public async Task UpdateLyricAsync(LyricUpdateViewModel updatedLyric)
    {
      IEnumerable<LyricSlugViewModel> lyricSlugs = await lyricSlugService.GetSlugsForLyricAsync(updatedLyric.Id);

      string updatedSlug = updatedLyric.Title.NormalizeStringForUrl();

      bool slugDoesNotExistAlready = lyricSlugs.Any(s => s.Name != updatedSlug);

      string connectionString = databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update lyrics set title = @title, body = @body where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@id", updatedLyric.Id);
        command.Parameters.AddWithValue("@title", updatedLyric.Title);
        command.Parameters.AddWithValue("@body", updatedLyric.Body);

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
        await SetIsPrimaryAsFalseOnAllLyricSlugsForLyricWithIdAsync(updatedLyric.Id);

        string sqlStatementToAddNewLyricSlug = "insert into lyric_slugs (name, is_primary, created_at, is_deleted, lyric_id) values(@name, @is_primary, @created_at, @is_deleted, @lyric_id)";
        
        int lyricId = updatedLyric.Id;
        bool isPrimary = true;
        bool isDeleted = false;
        DateTime createdAt = DateTime.UtcNow;

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
          NpgsqlCommand command = new NpgsqlCommand(sqlStatementToAddNewLyricSlug, connection);
          command.Parameters.AddWithValue("@name", updatedLyric.Title.NormalizeStringForUrl());
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
    }

    private async Task SetIsPrimaryAsFalseOnAllLyricSlugsForLyricWithIdAsync(int lyricId)
    {
      string connectionString = databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update lyric_slugs set is_primary = false where lyric_id = @lyric_id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
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
  }
}
