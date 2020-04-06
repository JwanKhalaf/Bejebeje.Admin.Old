namespace Bejebeje.ViewModels.Artist
{
  using System.ComponentModel.DataAnnotations;

  public class ArtistEditViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    [Display (Name = "First name")]
    public string FirstName { get; set; }

    [Display(Name = "Last name")]
    public string LastName { get; set; }

    [Display(Name = "Is approved?")]
    public bool IsApproved { get; set; }

    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }
  }
}
