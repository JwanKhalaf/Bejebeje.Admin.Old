using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels.LyricSlug;

namespace Services
{
  public interface ILyricSlugService
  {
    Task<IEnumerable<LyricSlugViewModel>> GetSlugsForLyricAsync(int lyricId);

    Task<int> AddNewLyricSlugAsync(LyricSlugCreateViewModel newLyricSlug);

    Task MakeLyricSlugPrimaryAsync(int lyricSlugId);
  }
}
