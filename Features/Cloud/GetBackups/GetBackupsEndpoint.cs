using Amazon.S3;
using Humanizer;

namespace PGBackup.Features.Cloud.GetBackups;

public class GetBackupsEndpoint : EndpointWithoutRequest<List<FileDetail>>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;

    public GetBackupsEndpoint(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    public override void Configure()
    {
        Get("/cloud/backup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var listAll = await _s3Client.ListObjectsAsync(_config["AWS:BucketName"], "backup", cancellationToken);
        List<FileDetail> response = new();
        foreach (var file in listAll.S3Objects)
        {
            response.Add(new FileDetail
            {
                Key = file.Key,
                Name = Path.GetFileName(file.Key),
                Size = file.Size,
                NiceSize = file.Size.Bytes().Humanize(),
                StorageClass = file.StorageClass.Value,
                LastModified = file.LastModified,
                NiceLastModified = file.LastModified.Humanize(),
                BucketName = file.BucketName
            });
        }
        await SendAsync(response, cancellation: cancellationToken);
    }
}