using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Transfer;
using PGBackup.Helpers;

namespace PGBackup.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureAWSS3(this IServiceCollection services, IConfiguration configuration)
    {
        string accessKey = configuration["AWS:AccessKey"];
        string secretKey = configuration["AWS:SecretKey"];

        var options = new CredentialProfileOptions
        {
            AccessKey = accessKey,
            SecretKey = secretKey
        };
        var profile = new CredentialProfile("default", options)
        {
            Region = RegionEndpoint.EUWest1
        };
        var sharedFile = new SharedCredentialsFile();
        sharedFile.RegisterProfile(profile);

        RegionEndpoint bucketRegion = RegionEndpoint.AFSouth1;
        IAmazonS3 s3Client = new AmazonS3Client(bucketRegion);
        TransferUtility transferUtility = new(s3Client);
        FileUploader fileUploader = new(transferUtility, configuration);
        services.AddSingleton(fileUploader);
        services.AddSingleton(s3Client);
    }
}