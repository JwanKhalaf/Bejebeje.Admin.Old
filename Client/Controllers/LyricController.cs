namespace Client.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System.Threading.Tasks;
  using ViewModels.Artist;
  using ViewModels.Lyric;

  public class LyricController : Controller
  {
    private readonly IArtistService _artistService;

    private readonly ILyricService _lyricService;

    public LyricController(
      IArtistService artistService,
      ILyricService lyricService)
    {
      _artistService = artistService;
      _lyricService = lyricService;
    }

    public async Task<IActionResult> Index([FromQuery] int artistId)
    {
      ArtistLyricsViewModel viewModel = await _lyricService.GetLyricsForArtistAsync(artistId);

      return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
      LyricViewModel lyric = await _lyricService.GetLyricByIdAsync(id);

      ArtistViewModel artist = await _artistService.GetArtistByIdAsync(lyric.ArtistId);

      LyricDetailsViewModel viewModel = new LyricDetailsViewModel();
      viewModel.Lyric = lyric;
      viewModel.Artist = artist;

      return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      LyricViewModel lyric = await _lyricService.GetLyricByIdAsync(id);

      LyricUpdateViewModel viewModel = new LyricUpdateViewModel();
      viewModel.Id = lyric.Id;
      viewModel.Title = lyric.Title;
      viewModel.Body = lyric.Body;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LyricUpdateViewModel updatedLyric)
    {
      await _lyricService.UpdateLyricAsync(updatedLyric);

      return RedirectToAction("Details", new { id = updatedLyric.Id });
    }
  }
}