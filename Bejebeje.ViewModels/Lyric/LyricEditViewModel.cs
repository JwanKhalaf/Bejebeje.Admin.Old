namespace Bejebeje.ViewModels.Lyric
{
  using System.ComponentModel.DataAnnotations;

  public class LyricEditViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Body { get; set; }

    [Required]
    [Display(Name = "Is verified?")]
    public bool IsVerified { get; set; }

    [Required]
    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }
  }
}
