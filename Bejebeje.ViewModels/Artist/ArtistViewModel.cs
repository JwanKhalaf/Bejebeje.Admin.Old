﻿namespace Bejebeje.ViewModels.Artist
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using Shared;

  public class ArtistViewModel
  {
    public int Id { get; set; }

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    [Display(Name = "Last name")]
    public string LastName { get; set; }

    [Display(Name = "Full name")]
    public string FullName { get; set; }

    public SexViewModel Sex { get; set; }

    [Display(Name = "Is approved?")]
    public bool IsApproved { get; set; }

    [Display(Name = "User Id")]
    public string UserId { get; set; }

    [Display(Name = "Created at")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Modified at")]
    public DateTime? ModifiedAt { get; set; }

    [Display(Name = "Is deleted?")]
    public bool IsDeleted { get; set; }

    [Display(Name = "Has an image?")]
    public bool HasImage { get; set; }

    public string ImageUrl { get; set; }
  }
}
