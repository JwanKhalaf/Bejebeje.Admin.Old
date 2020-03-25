namespace ViewModels.Lyric
{
  using System;

  public class LyricViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public string UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsApproved { get; set; }

    public int ArtistId { get; set; }

    public int? AuthorId { get; set; }
  }
}
