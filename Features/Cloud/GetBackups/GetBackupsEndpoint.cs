using Amazon.S3;

namespace PGBackup.Features.Cloud.GetBackups;

public class GetBackupsEndpoint : EndpointWithoutRequest<Response>
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
        Get("/cloud/backups");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var listAll = await _s3Client.ListObjectsAsync(_config["AWS:BucketName"], "backup", cancellationToken);
        Response response = new();
        foreach (var file in listAll.S3Objects)
        {
            response.Files.Add(new FileDetail
            {
                Key = file.Key,
                Name = Path.GetFileName(file.Key),
                Size = file.Size,
                StorageClass = file.StorageClass.Value,
                LastModified = file.LastModified,
                BucketName = file.BucketName
            });
        }
        await SendAsync(response, cancellation: cancellationToken);
    }
}