namespace Bejebeje.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.ArtistSlug;

  public interface IArtistSlugService
  {
    Task<IEnumerable<ArtistSlugViewModel>> GetSlugsForArtistAsync(int artistId);

    Task<ArtistSlugViewModel> GetArtistSlugByIdAsync(int id);

    Task AddArtistSlugAsync(ArtistSlugCreateViewModel artistSlug);

    Task EditArtistSlugAsync(ArtistSlugEditViewModel editedArtistSlug);

    Task MarkIsPrimaryAsFalseForAllArtistSlugs(int artistId);

    Task MarkArtistSlugAsPrimary(int artistSlugId);
  }
}
