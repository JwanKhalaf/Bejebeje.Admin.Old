namespace Bejebeje.Services
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Config;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using ViewModels.ArtistImage;

  public class ArtistImageService : IArtistImageService
  {
    private readonly DatabaseOptions _databaseOptions;

    public ArtistImageService(IOptionsMonitor<DatabaseOptions> optionsAccessor)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
    }

    public async Task<ArtistImageReadViewModel> GetImageByIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;

      string sqlStatement = "select * from artist_images where id = @id";

      ArtistImageReadViewModel image = null;

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
            image = new ArtistImageReadViewModel();
            image.Id = Convert.ToInt32(reader[0]);
            image.Data = (byte[])reader[1];
            image.CreatedAt = Convert.ToDateTime(reader[2]);
            image.ModifiedAt = reader[3] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[3]);
            image.ArtistId = Convert.ToInt32(reader[4]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }

      return image;
    }

    public async Task<ArtistImageReadViewModel> GetImageByArtistIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;

      string sqlStatement = "select * from artist_images where artist_id = @artist_id";

      ArtistImageReadViewModel image = null;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        command.Parameters.AddWithValue("@artist_id", id);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            image = new ArtistImageReadViewModel();
            image.Id = Convert.ToInt32(reader[0]);
            image.Data = (byte[])reader[1];
            image.CreatedAt = Convert.ToDateTime(reader[2]);
            image.ModifiedAt = reader[3] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[3]);
            image.ArtistId = Convert.ToInt32(reader[4]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }

      return image;
    }

    public async Task AddNewArtistImageAsync(ArtistImageCreateViewModel artistImage)
    {
      if (artistImage.File.Length > 0)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          artistImage.File.CopyTo(memoryStream);

          byte[] data = memoryStream.ToArray();
          DateTime createdAt = DateTime.UtcNow;
          int artistId = artistImage.ArtistId;

          string connectionString = _databaseOptions.ConnectionString;

          string sqlStatement = "insert into artist_images (data, created_at, artist_id) values (@data, @created_at, @artist_id)";

          using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
          {
            NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

            command.Parameters.AddWithValue("@data", data);
            command.Parameters.AddWithValue("@created_at", createdAt);
            command.Parameters.AddWithValue("@artist_id", artistId);

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
      }
    }

    public async Task EditArtistImageAsync(ArtistImageEditViewModel artistImage)
    {
      if (artistImage.File.Length > 0)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          artistImage.File.CopyTo(memoryStream);

          int id = artistImage.Id;
          byte[] data = memoryStream.ToArray();
          DateTime modifiedAt = DateTime.UtcNow;

          string connectionString = _databaseOptions.ConnectionString;

          string sqlStatement = "update artist_images set data = @data, modified_at = @modified_at where id = @id";

          using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
          {
            NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@data", data);
            command.Parameters.AddWithValue("@modified_at", modifiedAt);

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
      }
    }

    public async Task RemoveArtistImageAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;

      string sqlStatement = "delete from artist_images where id = @id";

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        command.Parameters.AddWithValue("@id", id);

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
  }
}
