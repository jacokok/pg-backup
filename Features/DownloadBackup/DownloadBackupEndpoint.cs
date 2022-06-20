namespace PGBackup.Features.DeleteBackup;

public class DownloadBackupEndpoint : Endpoint<Request>
{
    private readonly IWebHostEnvironment _host;

    public DownloadBackupEndpoint(IWebHostEnvironment host)
    {
        _host = host;
    }

    public override void Configure()
    {
        Get("/backup/{FileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(_host.ContentRootPath, "backup");
        string filePath = Path.Combine(backupPath, request.FileName);
        if (File.Exists(filePath))
        {
            var fi = new FileInfo(filePath);
            await SendFileAsync(fi, cancellation: cancellationToken);
        }
        else
        {
            await SendNotFoundAsync(cancellationToken);
        }
    }
}