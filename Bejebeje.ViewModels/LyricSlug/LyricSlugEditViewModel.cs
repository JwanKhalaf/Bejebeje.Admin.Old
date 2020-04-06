namespace Bejebeje.ViewModels.LyricSlug
{
  using System.ComponentModel.DataAnnotations;

  public class LyricSlugEditViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Display (Name = "Is primary?")]
    public bool IsPrimary { get; set; }

    [Required]
    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }

    [Required]
    public int LyricId { get; set; }
  }
}
