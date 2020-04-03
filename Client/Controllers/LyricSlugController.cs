namespace Client.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System;
  using System.Threading.Tasks;
  using ViewModels.LyricSlug;

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
      catch (Exception exception)
      {
        Console.WriteLine(exception.Message);

        throw;
      }
    }

    public async Task<IActionResult> Edit(int id)
    {
      LyricSlugViewModel slug = await _lyricSlugService.GetSlugByIdAsync(id);

      LyricSlugEditViewModel viewModel = new LyricSlugEditViewModel();
      viewModel.Id = id;
      viewModel.LyricId = slug.LyricId;
      viewModel.IsPrimary = slug.IsPrimary;
      viewModel.IsDeleted = slug.IsDeleted;
      viewModel.Name = slug.Name;
      
      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LyricSlugEditViewModel editedLyricSlug)
    {
      await _lyricSlugService.EditLyricSlugAsync(editedLyricSlug);

      return RedirectToAction("Details", "Lyric", new { id = editedLyricSlug.LyricId });
    }
  }
}