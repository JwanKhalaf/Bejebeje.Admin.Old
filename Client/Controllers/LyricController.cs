using System.Collections.Generic;
using ViewModels.LyricSlug;

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

    private readonly ILyricSlugService _lyricSlugService;

    public LyricController(
      IArtistService artistService,
      ILyricService lyricService,
      ILyricSlugService lyricSlugService)
    {
      _artistService = artistService;
      _lyricService = lyricService;
      _lyricSlugService = lyricSlugService;
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

      IEnumerable<LyricSlugViewModel> slugs = await _lyricSlugService.GetSlugsForLyricAsync(lyric.Id);

      LyricDetailsViewModel viewModel = new LyricDetailsViewModel();
      viewModel.Lyric = lyric;
      viewModel.Artist = artist;
      viewModel.Slugs = slugs;

      return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      LyricViewModel lyric = await _lyricService.GetLyricByIdAsync(id);

      LyricEditViewModel viewModel = new LyricEditViewModel();
      viewModel.Id = lyric.Id;
      viewModel.Title = lyric.Title;
      viewModel.Body = lyric.Body;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LyricEditViewModel updatedLyric)
    {
      await _lyricService.EditLyricAsync(updatedLyric);

      return RedirectToAction("Details", new { id = updatedLyric.Id });
    }
  }
}