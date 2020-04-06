namespace Bejebeje.ViewModels.ArtistImage
{
  using Microsoft.AspNetCore.Http;

  public class ArtistImageEditViewModel
  {
    public int Id { get; set; }

    public IFormFile File { get; set; }

    public int ArtistId { get; set; }
  }
}
