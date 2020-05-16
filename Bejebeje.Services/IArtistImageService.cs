namespace Bejebeje.Services
{
  using System.Threading.Tasks;
  using ViewModels.ArtistImage;

  public interface IArtistImageService
  {
    Task<ArtistImageReadViewModel> GetImageByIdAsync(int id);

    Task<ArtistImageReadViewModel> GetImageByArtistIdAsync(int id);

    Task AddArtistImageAsync(ArtistImageCreateViewModel artistImage);

    Task EditArtistImageAsync(ArtistImageEditViewModel artistImage);

    Task RemoveArtistImageAsync(int id);
  }
}
