namespace Services
{
  using Microsoft.Extensions.Options;
  using Npgsql;
  using Services.Config;
  using System;
  using System.Threading.Tasks;
  using ViewModels;

  public class ArtistSlugService : IArtistSlugService
  {
    private readonly DatabaseOptions databaseOptions;

    public ArtistSlugService(IOptionsMonitor<DatabaseOptions> optionsAccessor)
    {
      databaseOptions = optionsAccessor.CurrentValue;
    }

    public async Task AddNewArtistSlugAsync(ArtistSlugViewModel artistSlug)
    {
      string connectionString = databaseOptions.ConnectionString;
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
  }
}
