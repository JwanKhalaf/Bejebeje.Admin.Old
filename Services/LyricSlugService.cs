using Common;
using Microsoft.Extensions.Options;
using Npgsql;
using Services.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels.LyricSlug;

namespace Services
{
  public class LyricSlugService : ILyricSlugService
  {
    private readonly DatabaseOptions _databaseOptions;

    public LyricSlugService(IOptionsMonitor<DatabaseOptions> optionsAccessor)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
    }

    public async Task<int> AddNewLyricSlugAsync(LyricSlugCreateViewModel newLyricSlug)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "insert into lyric_slugs (name, is_primary, created_at, is_deleted, lyric_id) values (@name, @is_primary, @created_at, @is_deleted, @lyric_id) returning id";
      int lyricSlugId = 0;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@name", newLyricSlug.Name.NormalizeStringForUrl());
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
  }
}
