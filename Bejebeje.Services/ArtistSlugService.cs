namespace Bejebeje.Services
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Common;
  using Config;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using ViewModels.ArtistSlug;

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

    public async Task<ArtistSlugViewModel> GetArtistSlugByIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from artist_slugs where id = @id";
      ArtistSlugViewModel artistSlug = null;

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
            artistSlug = new ArtistSlugViewModel();
            artistSlug.Id = Convert.ToInt32(reader[0]);
            artistSlug.Name = Convert.ToString(reader[1]);
            artistSlug.IsPrimary = Convert.ToBoolean(reader[2]);
            artistSlug.CreatedAt = Convert.ToDateTime(reader[3]);
            artistSlug.ModifiedAt = reader[4] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[4]);
            artistSlug.IsDeleted = Convert.ToBoolean(reader[5]);
            artistSlug.ArtistId = Convert.ToInt32(reader[6]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }

      return artistSlug;
    }

    public async Task AddArtistSlugAsync(ArtistSlugCreateViewModel artistSlug)
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

        string name = artistSlug.Name.NormalizeStringForUrl();
        DateTime createdAt = DateTime.UtcNow;

        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@is_primary", artistSlug.IsPrimary);
        command.Parameters.AddWithValue("@created_at", createdAt);
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

    public async Task EditArtistSlugAsync(ArtistSlugEditViewModel editedArtistSlug)
    {
      if (editedArtistSlug.IsPrimary)
      {
        await MarkIsPrimaryAsFalseForAllArtistSlugs(editedArtistSlug.ArtistId);
      }

      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "update artist_slugs set name = @name, is_primary = @is_primary, modified_at = @modified_at, is_deleted = @is_deleted where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        string name = editedArtistSlug.Name.NormalizeStringForUrl();
        bool isPrimary = editedArtistSlug.IsPrimary;
        DateTime modifiedAt = DateTime.UtcNow;
        bool isDeleted = editedArtistSlug.IsDeleted;

        command.Parameters.AddWithValue("@id", editedArtistSlug.Id);
        command.Parameters.AddWithValue("@name", name);
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
