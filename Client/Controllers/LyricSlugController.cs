using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading.Tasks;
using ViewModels.LyricSlug;

namespace Client.Controllers
{
  public class LyricSlugController : Controller
  {
    private readonly ILyricSlugService _lyricSlugService;

    public LyricSlugController(ILyricSlugService lyricSlugService)
    {
      _lyricSlugService = lyricSlugService;
    }

    public IActionResult Create([FromQuery] int lyricId)
    {
      LyricSlugCreateViewModel viewModel = new LyricSlugCreateViewModel();
      viewModel.LyricId = lyricId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LyricSlugCreateViewModel lyricSlug)
    {
      try
      {
        lyricSlug.CreatedAt = DateTime.UtcNow;
        lyricSlug.IsDeleted = false;

        await _lyricSlugService.AddNewLyricSlugAsync(lyricSlug);

        return RedirectToAction("Details", "Lyric", new { id = lyricSlug.LyricId });
      }
      catch (Exception)
      {

        throw;
      }
    }
  }
}