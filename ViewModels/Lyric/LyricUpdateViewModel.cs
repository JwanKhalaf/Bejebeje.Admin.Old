using System.ComponentModel.DataAnnotations;

namespace ViewModels.Lyric
{
  public class LyricUpdateViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Body { get; set; }
  }
}
