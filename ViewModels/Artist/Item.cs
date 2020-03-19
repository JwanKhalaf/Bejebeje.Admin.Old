using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Artist
{
  public class Item
  {
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName { get; set; }

    public bool IsApproved { get; set; }

    public string UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
  }
}
