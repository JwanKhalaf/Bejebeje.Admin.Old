namespace Bejebeje.Admin.Controllers
{
  using System.IO;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels.ArtistImage;

  [Authorize(Roles = "moderator,administrator")]
  public class ArtistImageController : Controller
  {
    private readonly IArtistImageService _artistImageService;

    public ArtistImageController(IArtistImageService artistImageService)
    {
      _artistImageService = artistImageService;
    }

    public async Task<FileResult> Artist(int id)
    {
      ArtistImageReadViewModel image = await _artistImageService.GetImageByArtistIdAsync(id);

      byte[] data = null;

      if (image != null)
      {
        data = image.Data;
      }
      else
      {
        string filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\Images\\bejebeje-logo.jpg";
        data = System.IO.File.ReadAllBytes(filePath);
      }

      string contentType = "image/jpg";

      return File(data, contentType);
    }

    public IActionResult Create(int id)
    {
      ArtistImageCreateViewModel viewModel = new ArtistImageCreateViewModel();
      viewModel.ArtistId = id;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArtistImageCreateViewModel image)
    {
      await _artistImageService.AddArtistImageAsync(image);

      return RedirectToAction("Details", "Artist", new { id = image.ArtistId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      ArtistImageReadViewModel image = await _artistImageService.GetImageByArtistIdAsync(id);

      ArtistImageEditViewModel viewModel = new ArtistImageEditViewModel();
      viewModel.Id = image.Id;
      viewModel.ArtistId = image.ArtistId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ArtistImageEditViewModel image)
    {
      await _artistImageService.EditArtistImageAsync(image);

      return RedirectToAction("Details", "Artist", new { id = image.ArtistId });
    }

    public async Task<IActionResult> Delete(int id)
    {
      ArtistImageReadViewModel image = await _artistImageService.GetImageByArtistIdAsync(id);

      ArtistImageDeleteViewModel viewModel = new ArtistImageDeleteViewModel();
      viewModel.Id = image.Id;
      viewModel.ArtistId = image.ArtistId;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(ArtistImageDeleteViewModel image)
    {
      await _artistImageService.RemoveArtistImageAsync(image.Id);

      return RedirectToAction("Details", "Artist", new { id = image.ArtistId });
    }
  }
}
