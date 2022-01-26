namespace Bejebeje.ViewModels.Validations;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

public class AllowedExtensionsAttribute : ValidationAttribute
{
  private readonly IList<string> _extensions;

  public AllowedExtensionsAttribute(string[] extensions)
  {
    _extensions = new List<string>(extensions);
  }

  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    IFormFile file = value as IFormFile;

    if (file == null) return ValidationResult.Success;

    string extension = Path.GetExtension(file.FileName);

    return !_extensions.Contains(extension.ToLower())
      ? new ValidationResult(GetErrorMessage())
      : ValidationResult.Success;
  }

  public string GetErrorMessage()
  {
    return "Only .jpg and .png files are allowed";
  }
}