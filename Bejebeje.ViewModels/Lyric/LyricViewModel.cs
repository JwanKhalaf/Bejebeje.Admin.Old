namespace Bejebeje.ViewModels.Lyric
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class LyricViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public string UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }

    [Display(Name = "Is approved?")]
    public bool IsApproved { get; set; }

    public int ArtistId { get; set; }

    public int? AuthorId { get; set; }

    [Display (Name = "Is verified?")]
    public bool IsVerified { get; set; }
  }
}
