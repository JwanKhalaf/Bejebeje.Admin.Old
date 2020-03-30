namespace ViewModels.Artist
{
  using System.Collections.Generic;
  using ArtistSlug;

  public class ArtistDetailsViewModel
  {
    public ArtistViewModel Artist { get; set; }

    public IEnumerable<ArtistSlugViewModel> Slugs { get; set; }
  }
}
