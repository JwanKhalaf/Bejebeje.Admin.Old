namespace Bejebeje.Services
{
  using System.Threading.Tasks;
  using ViewModels.Lyric;

  public interface ILyricService
  {
    Task<LyricViewModel> GetLyricByIdAsync(int id);

    Task<ArtistLyricsViewModel> GetLyricsForArtistAsync(int artistId);

    Task<int> AddLyricAsync(LyricCreateViewModel newLyric);

    Task EditLyricAsync(LyricEditViewModel editedLyric);
  }
}
