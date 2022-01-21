namespace Bejebeje.Services;

using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

public class ArtistImage
{
  private const string JpegExtension = ".jpg";

  private const string WebpExtension = ".webp";

  private string PrimarySlug { get; set; }

  private int ArtistId { get; set; }

  public Image Image { get; set; }

  public ArtistImage(string primarySlug, int artistId, Image image)
  {
    PrimarySlug = primarySlug;
    ArtistId = artistId;
    Image = image;
  }

  public string GetKey(ImageSize imageSize, ImageType imageType)
  {
    if (imageType == ImageType.Jpeg)
    {
      return imageSize switch
      {
        ImageSize.Standard => $"artist-images/standard/{PrimarySlug}-{ArtistId}{JpegExtension}",
        ImageSize.Small => $"artist-images/small/{PrimarySlug}-{ArtistId}{JpegExtension}",
        _ => $"artist-images/extra-small/{PrimarySlug}-{ArtistId}{JpegExtension}"
      };
    }

    return imageSize switch
    {
      ImageSize.Standard => $"artist-images/standard/{PrimarySlug}-{ArtistId}{WebpExtension}",
      ImageSize.Small => $"artist-images/small/{PrimarySlug}-{ArtistId}{WebpExtension}",
      _ => $"artist-images/extra-small/{PrimarySlug}-{ArtistId}{WebpExtension}"
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

      await Image.SaveAsWebpAsync(memoryStream, new WebpEncoder { Quality = 50 });
    }

    return memoryStream;
  }
}