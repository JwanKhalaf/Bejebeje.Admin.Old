namespace Bejebeje.Admin.Controllers
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Extensions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels.Artist;
  using ViewModels.Lyric;
  using ViewModels.LyricSlug;

  [Authorize(Roles = "moderator,administrator")]
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

    public IActionResult Create(int artistId)
    {
      LyricCreateViewModel viewModel = new LyricCreateViewModel();
      viewModel.ArtistId = artistId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LyricCreateViewModel newLyric)
    {
      string userId = User.GetUserId().ToString();
      newLyric.UserId = userId;

      int lyricId = await _lyricService.AddLyricAsync(newLyric);

      return RedirectToAction("Details", new { id = lyricId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      LyricViewModel lyric = await _lyricService.GetLyricByIdAsync(id);

      LyricEditViewModel viewModel = new LyricEditViewModel();
      viewModel.Id = lyric.Id;
      viewModel.Title = lyric.Title;
      viewModel.Body = lyric.Body;
      viewModel.IsVerified = lyric.IsVerified;
      viewModel.IsDeleted = lyric.IsDeleted;

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
