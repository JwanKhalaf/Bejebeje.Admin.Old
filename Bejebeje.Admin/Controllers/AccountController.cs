namespace Bejebeje.Admin.Controllers
{
  using Microsoft.AspNetCore.Mvc;

  public class AccountController : Controller
  {
    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}