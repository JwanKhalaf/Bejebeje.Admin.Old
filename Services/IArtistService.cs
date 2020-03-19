namespace Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.Artist;

  public interface IArtistService
  {
    Task<IEnumerable<Item>> GetArtistsAsync();
    
    Task<Item> GetArtistByIdAsync(int id);

    Task<int> AddNewArtistAsync(Item artist);
  }
}
