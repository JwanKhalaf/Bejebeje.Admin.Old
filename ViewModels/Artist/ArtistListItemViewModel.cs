namespace ViewModels.Artist
{
  public class ArtistListItemViewModel
  {
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsApproved { get; set; }

    public bool IsDeleted { get; set; }
  }
}
