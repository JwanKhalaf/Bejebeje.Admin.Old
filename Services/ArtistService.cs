namespace Services
{
  using Common;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using Services.Config;
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels;
  using ViewModels.Artist;

  public class ArtistService : IArtistService
  {
    private readonly DatabaseOptions databaseOptions;

    private readonly IArtistSlugService artistSlugService;

    public ArtistService(
      IOptionsMonitor<DatabaseOptions> optionsAccessor,
      IArtistSlugService artistSlugService)
    {
      databaseOptions = optionsAccessor.CurrentValue;
      this.artistSlugService = artistSlugService;
    }

    public async Task<IEnumerable<Item>> GetArtistsAsync()
    {
      string connectionString = databaseOptions.ConnectionString;
      string sqlStatement = "select * from artists order by first_name";
      List<Item> artists = new List<Item>();

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            Item artist = new Item();
            artist.Id = Convert.ToInt32(reader[0]);
            artist.FirstName = Convert.ToString(reader[1]);
            artist.LastName = Convert.ToString(reader[2]);
            artist.FullName = Convert.ToString(reader[3]);
            artist.IsApproved = Convert.ToBoolean(reader[4]);
            artist.UserId = Convert.ToString(reader[5]);
            artist.CreatedAt = Convert.ToDateTime(reader[6]);
            artist.ModifiedAt = reader[7] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[7]);
            artist.IsDeleted = Convert.ToBoolean(reader[8]);

            artists.Add(artist);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return artists;
    }

    public async Task<Item> GetArtistByIdAsync(int id)
    {
      string connectionString = databaseOptions.ConnectionString;
      string sqlStatement = "select * from artists where id = @artist_id";
      Item artist = null;

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
            artist = new Item();
            artist.Id = Convert.ToInt32(reader[0]);
            artist.FirstName = Convert.ToString(reader[1]);
            artist.LastName = Convert.ToString(reader[2]);
            artist.FullName = Convert.ToString(reader[3]);
            artist.IsApproved = Convert.ToBoolean(reader[4]);
            artist.UserId = Convert.ToString(reader[5]);
            artist.CreatedAt = Convert.ToDateTime(reader[6]);
            artist.ModifiedAt = reader[7] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[7]);
            artist.IsDeleted = Convert.ToBoolean(reader[8]);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return artist;
    }

    public async Task<int> AddNewArtistAsync(Item artist)
    {
      string connectionString = databaseOptions.ConnectionString;
      string sqlStatement = "insert into artists (first_name, last_name, full_name, is_approved, user_id, created_at, is_deleted) values (@first_name, @last_name, @full_name, @is_approved, @user_id, @created_at, @is_deleted) returning id";
      int artistId = 0;

      string firstName = artist.FirstName.Standardize();
      string lastName = artist.LastName.Standardize();
      string fullName = string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";
      bool isApproved = true;
      DateTime createdAt = DateTime.UtcNow;
      string userId = databaseOptions.UserId;
      bool isDeleted = false;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@full_name", fullName);
        command.Parameters.AddWithValue("@is_approved", isApproved);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@created_at", createdAt);
        command.Parameters.AddWithValue("@is_deleted", isDeleted);

        try
        {
          await connection.OpenAsync();

          object artistIdentity = command.ExecuteScalar();
          artistId = (int)artistIdentity;

          ArtistSlugViewModel artistSlug = new ArtistSlugViewModel();
          artistSlug.Name = fullName.NormalizeStringForUrl();
          artistSlug.IsPrimary = true;
          artistSlug.CreatedAt = createdAt;
          artistSlug.ArtistId = artistId;

          await artistSlugService.AddNewArtistSlugAsync(artistSlug);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return artistId;
    }
  }
}
