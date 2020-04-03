using System.Collections.Generic;
using ViewModels.Artist;
using ViewModels.LyricSlug;

namespace ViewModels.Lyric
{
  public class LyricDetailsViewModel
  {
    public LyricViewModel Lyric { get; set; }

    public ArtistViewModel Artist { get; set; }

    public IEnumerable<LyricSlugViewModel> Slugs { get; set; }
  }
}
