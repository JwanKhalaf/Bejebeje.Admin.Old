namespace Services
{
  using Microsoft.Extensions.Options;
  using Npgsql;
  using Config;
  using System;
  using System.Threading.Tasks;
  using ViewModels.ArtistSlug;
  using System.Collections.Generic;

  public class ArtistSlugService : IArtistSlugService
  {
    private readonly DatabaseOptions _databaseOptions;

    public ArtistSlugService(IOptionsMonitor<DatabaseOptions> optionsAccessor)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
    }

    public async Task<IEnumerable<ArtistSlugViewModel>> GetSlugsForArtistAsync(int artistId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from artist_slugs where artist_id = @artist_id";
      List<ArtistSlugViewModel> artistSlugs = new List<ArtistSlugViewModel>();

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
            ArtistSlugViewModel artistSlug = new ArtistSlugViewModel();
            artistSlug.Id = Convert.ToInt32(reader[0]);
            artistSlug.Name = Convert.ToString(reader[1]);
            artistSlug.IsPrimary = Convert.ToBoolean(reader[2]);
            artistSlug.CreatedAt = Convert.ToDateTime(reader[3]);
            artistSlug.ModifiedAt = reader[4] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[4]);
            artistSlug.IsDeleted = Convert.ToBoolean(reader[5]);
            artistSlug.ArtistId = Convert.ToInt32(reader[6]);

            artistSlugs.Add(artistSlug);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }

      return artistSlugs;
    }

    public async Task AddNewArtistSlugAsync(ArtistSlugCreateViewModel artistSlug)
    {
      if (artistSlug.IsPrimary)
      {
        await MarkIsPrimaryAsFalseForAllArtistSlugs(artistSlug.ArtistId);
      }

      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "insert into artist_slugs (name, is_primary, created_at, is_deleted, artist_id) values (@name, @is_primary, @created_at, @is_deleted, @artist_id)";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@name", artistSlug.Name);
        command.Parameters.AddWithValue("@is_primary", artistSlug.IsPrimary);
        command.Parameters.AddWithValue("@created_at", artistSlug.CreatedAt);
        command.Parameters.AddWithValue("@is_deleted", artistSlug.IsDeleted);
        command.Parameters.AddWithValue("@artist_id", artistSlug.ArtistId);

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

    public async Task MarkIsPrimaryAsFalseForAllArtistSlugs(int artistId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update artist_slugs set is_primary = false where artist_id = @artist_id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@artist_id", artistId);

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

    public async Task MarkArtistSlugAsPrimary(int artistSlugId)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric = "update artist_slugs set is_primary = true where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@id", artistSlugId);

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
