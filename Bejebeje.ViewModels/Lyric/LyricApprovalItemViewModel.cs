using System;

namespace Bejebeje.ViewModels.Lyric;

public class LyricApprovalItemViewModel
{
  public int LyricId { get; set; }

  public string LyricTitle { get; set; }

  public int ArtistId { get; set; }
  
  public string ArtistName { get; set; }
  
  public string SubmittedBy { get; set; }

  public DateTime CreatedAt { get; set; }
  
  public DateTime? ModifiedAt { get; set; }
}