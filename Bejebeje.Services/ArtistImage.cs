namespace Bejebeje.Services;

using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

public class ArtistImage
{
  private const string ArtistImagesFolderInS3 = "artist-images/";
  
  private const string StandardImageSizeSuffix = "s";
  
  private const string SmallImageSizeSuffix = "sm";
  
  private const string ExtraSmallImageSizeSuffix = "xsm";
  
  private const string JpegExtension = ".jpg";

  private const string WebpExtension = ".webp";

  private int ArtistId { get; }

  private Image Image { get; }

  public ArtistImage(int artistId, Image image)
  {
    ArtistId = artistId;
    Image = image;
  }

  public string GetKey(ImageSize imageSize, ImageType imageType)
  {
    if (imageType == ImageType.Jpeg)
    {
      return imageSize switch
      {
        ImageSize.Standard => $"{ArtistImagesFolderInS3}{ArtistId}-{StandardImageSizeSuffix}{JpegExtension}",
        ImageSize.Small => $"{ArtistImagesFolderInS3}{ArtistId}-{SmallImageSizeSuffix}{JpegExtension}",
        _ => $"{ArtistImagesFolderInS3}{ArtistId}-{ExtraSmallImageSizeSuffix}{JpegExtension}"
      };
    }

    return imageSize switch
    {
      ImageSize.Standard => $"{ArtistImagesFolderInS3}{ArtistId}-{StandardImageSizeSuffix}{WebpExtension}",
      ImageSize.Small => $"{ArtistImagesFolderInS3}{ArtistId}-{SmallImageSizeSuffix}{WebpExtension}",
      _ => $"{ArtistImagesFolderInS3}{ArtistId}-{ExtraSmallImageSizeSuffix}{WebpExtension}"
    };
  }

  public async Task<Stream> GetStreamAsync(ImageSize imageSize, ImageType imageType)
  {
    MemoryStream memoryStream = new MemoryStream();

    if (imageType == ImageType.Jpeg)
    {
      switch (imageSize)
      {
        case ImageSize.Standard:
          Image.Mutate(x => x.Resize(300, 300));
          break;
        case ImageSize.Small:
          Image.Mutate(x => x.Resize(80, 80));
          break;
        case ImageSize.ExtraSmall:
        default:
          Image.Mutate(x => x.Resize(60, 60));
          break;
      }

      await Image.SaveAsJpegAsync(memoryStream);
    }
    else
    {
      switch (imageSize)
      {
        case ImageSize.Standard:
          Image.Mutate(x => x.Resize(300, 300));
          break;
        case ImageSize.Small:
          Image.Mutate(x => x.Resize(80, 80));
          break;
        case ImageSize.ExtraSmall:
        default:
          Image.Mutate(x => x.Resize(60, 60));
          break;
      }

      await Image.SaveAsWebpAsync(memoryStream, new WebpEncoder { FileFormat = WebpFileFormatType.Lossy, Quality = 50 });
    }

    return memoryStream;
  }
}