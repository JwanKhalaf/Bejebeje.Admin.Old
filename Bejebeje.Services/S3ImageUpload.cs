namespace Bejebeje.Services;

using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class S3ImageUploadService : IS3ImageUploadService
{
  private readonly AWSOptions _awsOptions;

  public S3ImageUploadService(IOptionsMonitor<AWSOptions> optionsAccessor)
  {
    _awsOptions = optionsAccessor.CurrentValue;
  }
  
  public async Task UploadImageToS3Async(IFormFile file)
  {
    try
    {
      BasicAWSCredentials credentials = new BasicAWSCredentials(_awsOptions.AccessKey, _awsOptions.AccessSecret);
    
      AmazonS3Config config = new AmazonS3Config
      {
        RegionEndpoint = Amazon.RegionEndpoint.EUWest2
      };
    
      using AmazonS3Client client = new AmazonS3Client(credentials, config);
    
      await using MemoryStream newMemoryStream = new MemoryStream();
      file.CopyTo(newMemoryStream);

      TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
      {
        InputStream = newMemoryStream,
        Key = file.FileName,
        BucketName = _awsOptions.BucketName,
        CannedACL = S3CannedACL.PublicRead
      };

      TransferUtility fileTransferUtility = new TransferUtility(client);
    
      await fileTransferUtility.UploadAsync(uploadRequest);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }
}