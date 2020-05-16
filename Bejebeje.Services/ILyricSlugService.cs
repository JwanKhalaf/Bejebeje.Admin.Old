namespace Bejebeje.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.LyricSlug;

  public interface ILyricSlugService
  {
    Task<LyricSlugViewModel> GetSlugByIdAsync(int id);

    Task<IEnumerable<LyricSlugViewModel>> GetSlugsForLyricAsync(int lyricId);

    Task<int> AddLyricSlugAsync(LyricSlugCreateViewModel newLyricSlug);

    Task EditLyricSlugAsync(LyricSlugEditViewModel editedLyricSlug);

    Task MakeLyricSlugPrimaryAsync(int lyricSlugId);

    Task MarkIsPrimaryAsFalseForAllLyricSlugs(int lyricId);
  }
}
