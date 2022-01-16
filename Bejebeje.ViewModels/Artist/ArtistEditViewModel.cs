namespace Bejebeje.ViewModels.Artist
{
  using System.ComponentModel.DataAnnotations;
  using Microsoft.AspNetCore.Http;
  using Shared;
  using Validations;

  public class ArtistEditViewModel
  {
    [Required] public int Id { get; set; }

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    [Display(Name = "Last name")] public string LastName { get; set; }

    [Display(Name = "Is approved?")] public bool IsApproved { get; set; }

    [Display(Name = "Is deleted?")] public bool IsDeleted { get; set; }

    public SexViewModel Sex { get; set; }

    [AllowedExtensions(new[] { ".jpg", ".png" })]
    [MaxFileSizeInMegabytes(2)]
    [MinimumDimensions(300, 300)]
    [Display(Name = "Artist image")]
    public IFormFile Image { get; set; }
  }
}