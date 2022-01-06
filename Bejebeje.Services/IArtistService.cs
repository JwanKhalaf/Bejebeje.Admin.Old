﻿namespace Bejebeje.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.Artist;

  public interface IArtistService
  {
    Task<IEnumerable<ArtistListItemViewModel>> GetArtistsAsync();

    Task<ArtistViewModel> GetArtistByIdAsync(int id);

    Task<int> AddArtistAsync(ArtistViewModel artist);

    Task EditArtistAsync(ArtistEditViewModel editedArtist);
  }
}
