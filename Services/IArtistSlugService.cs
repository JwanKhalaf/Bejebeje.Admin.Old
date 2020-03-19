namespace Services
{
  using System.Threading.Tasks;
  using ViewModels;

  public interface IArtistSlugService
  {
    Task AddNewArtistSlugAsync(ArtistSlugViewModel artistSlug);
  }
}
