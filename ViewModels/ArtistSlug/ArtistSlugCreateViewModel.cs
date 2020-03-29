using System.ComponentModel.DataAnnotations;

namespace ViewModels.ArtistSlug
{
  using System;

  public class ArtistSlugCreateViewModel
  {
    public string Name { get; set; }

    [Display (Name = "Is primary?")]
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public int ArtistId { get; set; }
  }
}
