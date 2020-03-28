using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.LyricSlug
{
  public class LyricSlugCreateViewModel
  {
    [Required]
    public string Name { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [Required]
    public int LyricId { get; set; }
  }
}
