namespace ViewModels
{
  using System;

  public class ArtistSlugViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public int ArtistId { get; set; }
  }
}
