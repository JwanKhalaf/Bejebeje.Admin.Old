namespace Client.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System.Threading.Tasks;
  using ViewModels.Artist;
  using ViewModels.Lyric;

  public class LyricController : Controller
  {
    private readonly IArtistService artistService;

    private readonly ILyricService lyricService;

    public LyricController(
      IArtistService artistService,
      ILyricService lyricService)
    {
      this.artistService = artistService;
      this.lyricService = lyricService;
    }

    public async Task<IActionResult> Index([FromQuery] int artistId)
    {
      ArtistLyricsViewModel viewModel = await lyricService.GetLyricsForArtistAsync(artistId);

      return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
      LyricViewModel lyric = await lyricService.GetLyricByIdAsync(id);

      ArtistViewModel artist = await artistService.GetArtistByIdAsync(lyric.ArtistId);

      LyricDetailsViewModel viewModel = new LyricDetailsViewModel();
      viewModel.Lyric = lyric;
      viewModel.Artist = artist;

      return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      LyricViewModel lyric = await lyricService.GetLyricByIdAsync(id);

      LyricUpdateViewModel viewModel = new LyricUpdateViewModel();
      viewModel.Id = lyric.Id;
      viewModel.Title = lyric.Title;
      viewModel.Body = lyric.Body;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LyricUpdateViewModel updatedLyric)
    {
      await lyricService.UpdateLyricAsync(updatedLyric);

      return RedirectToAction("Details", new { id = updatedLyric.Id });
    }
  }
}