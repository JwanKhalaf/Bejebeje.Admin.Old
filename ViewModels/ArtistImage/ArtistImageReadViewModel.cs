namespace ViewModels.ArtistImage
{
  using System;

  public class ArtistImageReadViewModel
  {
    public int Id { get; set; }

    public byte[] Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int ArtistId { get; set; }
  }
}
