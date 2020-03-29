namespace Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.ArtistSlug;

  public interface IArtistSlugService
  {
    Task<IEnumerable<ArtistSlugViewModel>> GetSlugsForArtistAsync(int artistId);

    Task AddNewArtistSlugAsync(ArtistSlugCreateViewModel artistSlug);

    Task MarkIsPrimaryAsFalseForAllArtistSlugs(int artistId);

    Task MarkArtistSlugAsPrimary(int artistSlugId);
  }
}
