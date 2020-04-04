namespace ViewModels.ArtistImage
{
  using System.ComponentModel.DataAnnotations;
  using Microsoft.AspNetCore.Http;

  public class ArtistImageCreateViewModel
  {
    [Required]
    [Display(Name = "Image file")]
    public IFormFile File { get; set; }

    [Required]
    public int ArtistId { get; set; }
  }
}
