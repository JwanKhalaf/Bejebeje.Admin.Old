namespace Bejebeje.ViewModels.Validations;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

public class MinimumDimensionsAttribute : ValidationAttribute
{
  private readonly int _width;

  private readonly int _height;

  /// <summary>
  /// Set up the minimum dimensions validation attribute.
  /// </summary>
  /// <param name="width">Minimum width in pixels</param>
  /// <param name="height">Minimum height in pixels</param>
  public MinimumDimensionsAttribute(int width, int height)
  {
    _width = width;
    _height = height;
  }

  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    IFormFile file = value as IFormFile;

    if (file == null) return ValidationResult.Success;

    Image image = Image.Load(file.OpenReadStream());

    return (image.Height < _height || image.Width < _width)
      ? new ValidationResult(GetErrorMessage())
      : ValidationResult.Success;
  }

  public string GetErrorMessage()
  {
    return _height == _width
      ? $"Height and Width of image cannot be less than {_height}px"
      : $"Height of image cannot be less than {_height}px and Width image cannot be less than {_width}px";
  }
}