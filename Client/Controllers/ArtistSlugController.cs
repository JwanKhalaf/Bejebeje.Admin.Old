using System;

namespace Client.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System.Threading.Tasks;
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
        await _artistSlugService.AddNewArtistSlugAsync(artistSlug);

        return RedirectToAction("Details", "Artist", new {id = artistSlug.ArtistId});
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
        throw;
      }
    }
  }
}