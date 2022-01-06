namespace Bejebeje.ViewModels.LyricSlug
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class LyricSlugCreateViewModel
  {
    [Required]
    public string Name { get; set; }

    [Display(Name = "Is primary?")]
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [Required]
    public int LyricId { get; set; }
  }
}
