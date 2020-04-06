namespace Bejebeje.Services
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Common;
  using Config;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using ViewModels.LyricSlug;

  public class LyricSlugService : ILyricSlugService
  {
    private readonly DatabaseOptions _databaseOptions;

    public LyricSlugService(IOptionsMonitor<DatabaseOptions> optionsAccessor)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
    }

    public async Task<LyricSlugViewModel> GetSlugByIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from lyric_slugs where id = @id";
      LyricSlugViewModel slug = null;

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
            slug = new LyricSlugViewModel();
            slug.Id = Convert.ToInt32(reader[0]);
            slug.Name = Convert.ToString(reader[1]);
            slug.IsPrimary = Convert.ToBoolean(reader[2]);
            slug.CreatedAt = Convert.ToDateTime(reader[3]);
            slug.ModifiedAt = reader[4] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[4]);
            slug.IsDeleted = Convert.ToBoolean(reader[5]);
            slug.LyricId = Convert.ToInt32(reader[6]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return slug;
    }

    public async Task<IEnumerable<LyricSlugViewModel>> GetSlugsForLyricAsync(int lyricId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from lyric_slugs where lyric_id = @lyric_id";
      List<LyricSlugViewModel> lyricSlugs = new List<LyricSlugViewModel>();

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@lyric_id", lyricId);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            LyricSlugViewModel lyricSlug = new LyricSlugViewModel();
            lyricSlug.Id = Convert.ToInt32(reader[0]);
            lyricSlug.Name = Convert.ToString(reader[1]);
            lyricSlug.IsPrimary = Convert.ToBoolean(reader[2]);
            lyricSlug.CreatedAt = Convert.ToDateTime(reader[3]);
            lyricSlug.ModifiedAt = reader[4] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[4]);
            lyricSlug.IsDeleted = Convert.ToBoolean(reader[5]);
            lyricSlug.LyricId = Convert.ToInt32(reader[6]);

            lyricSlugs.Add(lyricSlug);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return lyricSlugs;
    }

    public async Task<int> AddNewLyricSlugAsync(LyricSlugCreateViewModel newLyricSlug)
    {
      string connectionString = _databaseOptions.ConnectionString;

      string sqlStatement = "insert into lyric_slugs (name, is_primary, created_at, is_deleted, lyric_id) values (@name, @is_primary, @created_at, @is_deleted, @lyric_id) returning id";

      int lyricSlugId = 0;

      if (newLyricSlug.IsPrimary)
      {
        await MarkIsPrimaryAsFalseForAllLyricSlugs(newLyricSlug.LyricId);
      }

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        string slug = newLyricSlug.Name.NormalizeStringForUrl();

        command.Parameters.AddWithValue("@name", slug);
        command.Parameters.AddWithValue("@is_primary", newLyricSlug.IsPrimary);
        command.Parameters.AddWithValue("@created_at", newLyricSlug.CreatedAt);
        command.Parameters.AddWithValue("@is_deleted", newLyricSlug.IsDeleted);
        command.Parameters.AddWithValue("@lyric_id", newLyricSlug.LyricId);

        try
        {
          await connection.OpenAsync();

          object lyricSlugIdentity = command.ExecuteScalar();
          lyricSlugId = (int)lyricSlugIdentity;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return lyricSlugId;
    }

    public async Task EditLyricSlugAsync(LyricSlugEditViewModel editedLyricSlug)
    {
      int id = editedLyricSlug.Id;
      string slug = editedLyricSlug.Name.NormalizeStringForUrl();
      bool isPrimary = editedLyricSlug.IsPrimary;
      DateTime modifiedAt = DateTime.UtcNow;
      bool isDeleted = editedLyricSlug.IsDeleted;

      string connectionString = _databaseOptions.ConnectionString;

      string sqlStatement =
        "update lyric_slugs set name = @name, is_primary = @is_primary, modified_at = @modified_at, is_deleted = @is_deleted where id = @id";

      if (editedLyricSlug.IsPrimary)
      {
        await MarkIsPrimaryAsFalseForAllLyricSlugs(editedLyricSlug.LyricId);
      }

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", slug);
        command.Parameters.AddWithValue("@is_primary", isPrimary);
        command.Parameters.AddWithValue("@modified_at", modifiedAt);
        command.Parameters.AddWithValue("@is_deleted", isDeleted);

        try
        {
          await connection.OpenAsync();

          await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }
    }

    public async Task MakeLyricSlugPrimaryAsync(int lyricSlugId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "update lyric_slugs set is_primary = true where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@id", lyricSlugId);

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

    public async Task MarkIsPrimaryAsFalseForAllLyricSlugs(int lyricId)
    {
      string connectionString = _databaseOptions.ConnectionString;
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

          throw;
        }
      }
    }
  }
}
