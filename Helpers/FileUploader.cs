using Amazon.S3.Transfer;

namespace PGBackup.Helpers;

public class FileUploader
{
    private readonly TransferUtility _transferUtility;
    private readonly IConfiguration _config;

    public FileUploader(TransferUtility transferUtility, IConfiguration config)
    {
        _transferUtility = transferUtility;
        _config = config;
    }

    public async Task UploadFileAsync(Stream fileStream, string keyName, CancellationToken ct = default)
    {
        await _transferUtility.UploadAsync(fileStream, _config["AWS:BucketName"], keyName, ct);
    }
}