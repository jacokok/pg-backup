using Amazon.S3;
using Amazon.S3.Model;
using PGBackup.Features.Backup;

namespace PGBackup.Features.Cloud.DeleteBackup;

public class DeleteBackup : Endpoint<Request>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;
    private readonly ILogger<DeleteBackup> _logger;


    public DeleteBackup(IAmazonS3 s3Client, IConfiguration config, ILogger<DeleteBackup> logger)
    {
        _s3Client = s3Client;
        _config = config;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/cloud/{FileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _config["AWS:BucketName"],
                Key = Path.Combine("backup", request.FileName)
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
            await SendNoContentAsync(cancellationToken);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError("Error encountered on server. Message:'{Message}' when deleting an object", e.Message);
            await SendNotFoundAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown encountered on server. Message:'{Message}' when deleting an object", e.Message);
            await SendNotFoundAsync(cancellationToken);
        }
    }
}