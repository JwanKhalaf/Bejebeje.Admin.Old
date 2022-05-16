using System.Collections.Generic;

namespace Bejebeje.ViewModels.Lyric;

public class LyricsAwaitingApprovalViewModel
{
  public IEnumerable<LyricApprovalItemViewModel> Lyrics { get; set; }
}