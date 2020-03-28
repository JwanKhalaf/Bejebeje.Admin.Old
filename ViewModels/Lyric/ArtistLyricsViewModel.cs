namespace ViewModels.Lyric
{
  using System.Collections.Generic;
  using ViewModels.Artist;

  public class ArtistLyricsViewModel
  {
    public ArtistViewModel Artist { get; set; }

    public IEnumerable<LyricViewModel> Lyrics { get; set; }
  }
}
