namespace Bejebeje.Services;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IS3ImageUploadService
{
  Task UploadImageToS3Async(IFormFile file);
}