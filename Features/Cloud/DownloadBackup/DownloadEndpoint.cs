using Amazon.S3;
using Amazon.S3.Transfer;
using PGBackup.Features.Backup;

namespace PGBackup.Features.Cloud.DownloadBackup;

public class DownloadEndpoint : Endpoint<Request>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;


    public DownloadEndpoint(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    public override void Configure()
    {
        Get("/cloud/{FileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        TransferUtility transferUtility = new(_s3Client);
        using var result = await transferUtility.OpenStreamAsync(_config["AWS:BucketName"], Path.Combine("backup", request.FileName), cancellationToken);
        if (result.Length <= 0)
        {
            await SendNotFoundAsync(cancellationToken);
        }
        await SendStreamAsync(result, request.FileName, cancellation: cancellationToken);
    }
}