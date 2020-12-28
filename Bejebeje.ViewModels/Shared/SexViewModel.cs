using System.Collections.Generic;

namespace Bejebeje.ViewModels.Shared
{
  using Microsoft.AspNetCore.Mvc.Rendering;

  public class SexViewModel
  {
    public string SelectedSex { get; set; }

    public List<SelectListItem> SexOptions = new List<SelectListItem>
    {
      new SelectListItem { Value = "m", Text = "Male" },
      new SelectListItem { Value = "f", Text = "Female" },
    };
  }
}
