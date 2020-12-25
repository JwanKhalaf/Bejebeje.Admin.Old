namespace Bejebeje.ViewModels.Lyric
{
  using System.ComponentModel.DataAnnotations;

  public class LyricCreateViewModel
  {
    public string Title { get; set; }

    public string Body { get; set; }

    public string UserId { get; set; }

    public int ArtistId { get; set; }

    [Display(Name = "Is verified?")]
    public bool IsVerified { get; set; }
  }
}
