namespace ViewModels.Lyric
{
  using System.Collections.Generic;
  using Artist;

  public class ArtistLyricsViewModel
  {
    public ArtistViewModel Artist { get; set; }

    public IEnumerable<LyricViewModel> Lyrics { get; set; }
  }
}
