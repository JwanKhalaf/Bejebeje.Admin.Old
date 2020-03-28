namespace Client.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels.Artist;

  public class ArtistController : Controller
  {
    private readonly IArtistService artistService;

    public ArtistController(IArtistService artistService)
    {
      this.artistService = artistService;
    }

    public async Task<IActionResult> Index()
    {
      ArtistIndexViewModel viewModel = new ArtistIndexViewModel();

      IEnumerable<ArtistListItemViewModel> artists = await artistService.GetArtistsAsync();

      viewModel.Artists = artists;

      return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
      ArtistViewModel artist = await artistService.GetArtistByIdAsync(id);

      return View(artist);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArtistViewModel viewModel)
    {
      int artistId = await artistService.AddNewArtistAsync(viewModel);

      return RedirectToAction("Details", new { id = artistId });
    }
  }
}