namespace Bejebeje.Admin.Controllers
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels.ArtistSlug;

  public class ArtistSlugController : Controller
  {
    private readonly IArtistSlugService _artistSlugService;

    public ArtistSlugController(IArtistSlugService artistSlugService)
    {
      _artistSlugService = artistSlugService;
    }

    public IActionResult Create(int artistId)
    {
      ArtistSlugCreateViewModel viewModel = new ArtistSlugCreateViewModel();
      viewModel.ArtistId = artistId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArtistSlugCreateViewModel artistSlug)
    {
      try
      {
        await _artistSlugService.AddArtistSlugAsync(artistSlug);

        return RedirectToAction("Details", "Artist", new { id = artistSlug.ArtistId });
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
        throw;
      }
    }

    public async Task<IActionResult> Edit(int id)
    {
      ArtistSlugViewModel artistSlug = await _artistSlugService.GetArtistSlugByIdAsync(id);

      ArtistSlugEditViewModel viewModel = new ArtistSlugEditViewModel();
      viewModel.Id = artistSlug.Id;
      viewModel.Name = artistSlug.Name;
      viewModel.IsPrimary = artistSlug.IsPrimary;
      viewModel.IsDeleted = artistSlug.IsDeleted;
      viewModel.ArtistId = artistSlug.ArtistId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ArtistSlugEditViewModel editedArtistSlug)
    {
      await _artistSlugService.EditArtistSlugAsync(editedArtistSlug);

      return RedirectToAction("Details", "Artist", new { id = editedArtistSlug.ArtistId });
    }
  }
}