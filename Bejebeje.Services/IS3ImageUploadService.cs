namespace Bejebeje.Services;

using System.IO;
using System.Threading.Tasks;

public interface IS3ImageUploadService
{
  Task UploadImageToS3Async(string key, Stream stream);
}