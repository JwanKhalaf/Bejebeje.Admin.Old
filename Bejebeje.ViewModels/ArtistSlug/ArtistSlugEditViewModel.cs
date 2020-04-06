namespace Bejebeje.ViewModels.ArtistSlug
{
  using System.ComponentModel.DataAnnotations;

  public class ArtistSlugEditViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    [Display (Name = "Is primary?")]
    public bool IsPrimary { get; set; }

    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }

    public int ArtistId { get; set; }
  }
}
