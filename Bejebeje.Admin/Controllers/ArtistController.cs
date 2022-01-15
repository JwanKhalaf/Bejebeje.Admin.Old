﻿namespace Bejebeje.Admin.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Extensions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels.Artist;
  using ViewModels.ArtistSlug;
  using ViewModels.Shared;
  
  public class ArtistController : Controller
  {
    private readonly IArtistService _artistService;

    private readonly IArtistSlugService _artistSlugService;

    public ArtistController(
      IArtistService artistService,
      IArtistSlugService artistSlugService)
    {
      _artistService = artistService;
      _artistSlugService = artistSlugService;
    }

    public async Task<IActionResult> Index()
    {
      ArtistIndexViewModel viewModel = new ArtistIndexViewModel();

      IEnumerable<ArtistListItemViewModel> artists = await _artistService.GetArtistsAsync();

      viewModel.Artists = artists;

      return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
      ArtistViewModel artist = await _artistService.GetArtistByIdAsync(id);
      IEnumerable<ArtistSlugViewModel> slugs = await _artistSlugService.GetSlugsForArtistAsync(id);

      ArtistDetailsViewModel viewModel = new ArtistDetailsViewModel();
      viewModel.Artist = artist;
      viewModel.Slugs = slugs;

      return View(viewModel);
    }

    public IActionResult Create()
    {
      ArtistViewModel viewModel = new ArtistViewModel();

      viewModel.Sex = new SexViewModel();

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArtistViewModel viewModel)
    {
      string userId = User.GetUserId().ToString();
      viewModel.UserId = userId;
      int artistId = await _artistService.AddArtistAsync(viewModel);

      return RedirectToAction("Details", new { id = artistId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      ArtistViewModel artist = await _artistService.GetArtistByIdAsync(id);

      ArtistEditViewModel viewModel = new ArtistEditViewModel();
      viewModel.Id = artist.Id;
      viewModel.FirstName = artist.FirstName;
      viewModel.LastName = artist.LastName;
      viewModel.IsApproved = artist.IsApproved;
      viewModel.IsDeleted = artist.IsDeleted;
      viewModel.Sex = artist.Sex;

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ArtistEditViewModel editedArtist)
    {
      try
      {
        await _artistService.EditArtistAsync(editedArtist);

        return RedirectToAction("Details", "Artist", new { id = editedArtist.Id });
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);

        throw;
      }
    }
  }
}
