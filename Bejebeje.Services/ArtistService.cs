namespace Bejebeje.Services
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using Common;
  using Config;
  using Microsoft.Extensions.Options;
  using Npgsql;
  using SixLabors.ImageSharp;
  using SixLabors.ImageSharp.Processing;
  using ViewModels.Artist;
  using ViewModels.ArtistSlug;
  using ViewModels.Shared;

  public class ArtistService : IArtistService
  {
    private readonly DatabaseOptions _databaseOptions;

    private readonly IArtistSlugService _artistSlugService;

    private readonly IS3ImageUploadService _s3ImageUploadService;

    public ArtistService(
      IOptionsMonitor<DatabaseOptions> optionsAccessor,
      IArtistSlugService artistSlugService,
      IS3ImageUploadService s3ImageUploadService)
    {
      _databaseOptions = optionsAccessor.CurrentValue;
      _artistSlugService = artistSlugService;
      _s3ImageUploadService = s3ImageUploadService;
    }

    public async Task<IEnumerable<ArtistListItemViewModel>> GetArtistsAsync()
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from artists order by first_name";
      List<ArtistListItemViewModel> artists = new List<ArtistListItemViewModel>();

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);

        try
        {
          await connection.OpenAsync();

          NpgsqlDataReader reader = await command.ExecuteReaderAsync();

          while (reader.Read())
          {
            ArtistListItemViewModel artist = new ArtistListItemViewModel();
            artist.Id = Convert.ToInt32(reader[0]);
            artist.FirstName = Convert.ToString(reader[1]);
            artist.LastName = Convert.ToString(reader[2]);
            artist.IsApproved = Convert.ToBoolean(reader[4]);
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

    public async Task<ArtistViewModel> GetArtistByIdAsync(int id)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement = "select * from artists where id = @artist_id";
      ArtistViewModel artist = null;

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
            artist = new ArtistViewModel();
            artist.Id = Convert.ToInt32(reader[0]);
            artist.FirstName = Convert.ToString(reader[1]);
            artist.LastName = Convert.ToString(reader[2]);
            artist.FullName = Convert.ToString(reader[3]);
            artist.IsApproved = Convert.ToBoolean(reader[4]);
            artist.UserId = Convert.ToString(reader[5]);
            artist.CreatedAt = Convert.ToDateTime(reader[6]);
            artist.ModifiedAt = reader[7] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[7]);
            artist.IsDeleted = Convert.ToBoolean(reader[8]);
            artist.HasImage = Convert.ToBoolean(reader[9]);
            artist.Sex = new SexViewModel();
            artist.Sex.SelectedSex = Convert.ToString(reader[10]);

            if (artist.HasImage)
            {
              artist.ImageUrl =
                $"https://s3.eu-west-2.amazonaws.com/bejebeje.com/artist-images/small/{artist.FullName.NormalizeStringForUrl()}-{artist.Id}.jpg";
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);

          throw;
        }
      }

      return artist;
    }

    public async Task<int> AddArtistAsync(ArtistViewModel artist)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatement =
        "insert into artists (first_name, last_name, full_name, sex, is_approved, user_id, created_at, is_deleted, has_image) values (@first_name, @last_name, @full_name, @sex, @is_approved, @user_id, @created_at, @is_deleted, @has_image) returning id";
      int artistId = 0;

      string firstName = artist.FirstName.Standardize();
      string lastName = artist.LastName.Standardize();
      string fullName = string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";
      string sex = artist.Sex.SelectedSex;
      bool isApproved = true;
      DateTime createdAt = DateTime.UtcNow;
      string userId = artist.UserId;
      bool isDeleted = false;
      bool hasImage = false;

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatement, connection);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@full_name", fullName);
        command.Parameters.AddWithValue("@sex", sex);
        command.Parameters.AddWithValue("@is_approved", isApproved);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@created_at", createdAt);
        command.Parameters.AddWithValue("@is_deleted", isDeleted);
        command.Parameters.AddWithValue("@has_image", hasImage);

        try
        {
          await connection.OpenAsync();

          object artistIdentity = command.ExecuteScalar();
          artistId = (int)artistIdentity;

          ArtistSlugCreateViewModel artistSlug = new ArtistSlugCreateViewModel();
          artistSlug.Name = fullName.NormalizeStringForUrl();
          artistSlug.IsPrimary = true;
          artistSlug.CreatedAt = createdAt;
          artistSlug.ArtistId = artistId;

          await _artistSlugService.AddArtistSlugAsync(artistSlug);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }

      return artistId;
    }

    public async Task EditArtistAsync(ArtistEditViewModel editedArtist)
    {
      string connectionString = _databaseOptions.ConnectionString;
      string sqlStatementToUpdateLyric =
        "update artists set first_name = @first_name, last_name = @last_name, full_name = @full_name, sex = @sex, is_approved = @is_approved, modified_at = @modified_at, is_deleted = @is_deleted, has_image = @has_image where id = @id";

      int artistId = editedArtist.Id;
      string firstName = editedArtist.FirstName.Standardize();
      string lastName = editedArtist.LastName.Standardize();
      string fullName = string.IsNullOrEmpty(lastName)
        ? firstName
        : $"{firstName} {lastName}";
      string sex = editedArtist.Sex.SelectedSex;
      bool isApproved = editedArtist.IsApproved;
      DateTime modifiedAt = DateTime.UtcNow;
      bool isDeleted = editedArtist.IsDeleted;
      bool hasImage = editedArtist.Image is not null || editedArtist.HasImage;

      IEnumerable<ArtistSlugViewModel> artistSlugs = await _artistSlugService.GetSlugsForArtistAsync(editedArtist.Id);

      string updatedSlug = fullName.NormalizeStringForUrl();

      bool slugDoesNotExistAlready = artistSlugs.All(s => s.Name != updatedSlug);

      if (slugDoesNotExistAlready)
      {
        ArtistSlugCreateViewModel newArtistSlug = new ArtistSlugCreateViewModel();
        newArtistSlug.Name = fullName.NormalizeStringForUrl();
        newArtistSlug.IsDeleted = false;
        newArtistSlug.CreatedAt = DateTime.UtcNow;
        newArtistSlug.IsPrimary = true;
        newArtistSlug.ArtistId = editedArtist.Id;

        await _artistSlugService.AddArtistSlugAsync(newArtistSlug);
      }
      else
      {
        await _artistSlugService.MarkIsPrimaryAsFalseForAllArtistSlugs(editedArtist.Id);

        ArtistSlugViewModel existingSlug = artistSlugs.Single(s => s.Name == updatedSlug);

        await _artistSlugService.MarkArtistSlugAsPrimary(existingSlug.Id);
      }

      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        NpgsqlCommand command = new NpgsqlCommand(sqlStatementToUpdateLyric, connection);
        command.Parameters.AddWithValue("@id", artistId);
        command.Parameters.AddWithValue("@first_name", firstName);
        command.Parameters.AddWithValue("@last_name", lastName);
        command.Parameters.AddWithValue("@full_name", fullName);
        command.Parameters.AddWithValue("@sex", sex);
        command.Parameters.AddWithValue("@is_approved", isApproved);
        command.Parameters.AddWithValue("@modified_at", modifiedAt);
        command.Parameters.AddWithValue("@is_deleted", isDeleted);
        command.Parameters.AddWithValue("@has_image", hasImage);

        try
        {
          await connection.OpenAsync();

          await command.ExecuteNonQueryAsync();
          
          Image standardImage = await Image.LoadAsync(editedArtist.Image.OpenReadStream());

          ArtistImage artistImage = new ArtistImage(editedArtist.Id, standardImage);

          await _s3ImageUploadService.UploadImageToS3Async(
            artistImage.GetKey(ImageSize.Standard, ImageType.Jpeg),
            await artistImage.GetStreamAsync(ImageSize.Standard, ImageType.Jpeg));
      
          await _s3ImageUploadService.UploadImageToS3Async(
            artistImage.GetKey(ImageSize.Standard, ImageType.WebP),
            await artistImage.GetStreamAsync(ImageSize.Standard, ImageType.WebP));
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