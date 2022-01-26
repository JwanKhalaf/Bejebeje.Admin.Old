namespace Bejebeje.ViewModels.Validations;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ByteSizeLib;

public class MaxFileSizeInMegabytesAttribute : ValidationAttribute
{
  private readonly ByteSize _maxFileSize;

  public MaxFileSizeInMegabytesAttribute(int maxFileSize)
  {
    _maxFileSize = ByteSize.FromMegaBytes(maxFileSize);
  }

  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    IFormFile file = value as IFormFile;

    if (file == null) return ValidationResult.Success;
    
    ByteSize fileSize = ByteSize.FromBytes(file.Length);
    
    return fileSize.MegaBytes > _maxFileSize.MegaBytes ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
  }

  public string GetErrorMessage()
  {
    return $"Maximum allowed file size is {_maxFileSize.MegaBytes} megabytes.";
  }
}