namespace Bejebeje.Admin.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Options;
  using Bejebeje.Services.Config;
  using ViewModels.Shared;

  public class HomeController : Controller
  {
    private readonly IdentityServerConfigurationOptions _identityServerConfigurationOptions;

    public HomeController(IOptionsMonitor<IdentityServerConfigurationOptions> optionsAccessor)
    {
      _identityServerConfigurationOptions = optionsAccessor.CurrentValue;
    }

    public async Task<IActionResult> Index()
    {
      HomeViewModel viewModel = new HomeViewModel();
      viewModel.Authority = _identityServerConfigurationOptions.Authority;
      viewModel.ClientId = _identityServerConfigurationOptions.ClientId;
      viewModel.ClientSecret = _identityServerConfigurationOptions.ClientSecret;

      return View(viewModel);
    }
  }
}
