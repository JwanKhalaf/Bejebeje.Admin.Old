﻿namespace Services
{
  using System.Threading.Tasks;
  using ViewModels.Lyric;

  public interface ILyricService
  {
    Task<LyricViewModel> GetLyricByIdAsync(int id);

    Task<ArtistLyricsViewModel> GetLyricsForArtistAsync(int artistId);

    Task UpdateLyricAsync(LyricUpdateViewModel updatedLyric);
  }
}