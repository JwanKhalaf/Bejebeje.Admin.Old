namespace Bejebeje.ViewModels.Lyric
{
  using System.Collections.Generic;
  using Artist;
  using LyricSlug;

  public class LyricDetailsViewModel
  {
    public LyricViewModel Lyric { get; set; }

    public ArtistViewModel Artist { get; set; }

    public IEnumerable<LyricSlugViewModel> Slugs { get; set; }
  }
}
