namespace PGBackup.Features.Backup.DeleteBackup;

public class DeleteBackup : Endpoint<Request>
{
    private readonly IWebHostEnvironment _host;

    public DeleteBackup(IWebHostEnvironment host)
    {
        _host = host;
    }

    public override void Configure()
    {
        Delete("/backup/{FileName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(_host.ContentRootPath, "backup");
        string filePath = Path.Combine(backupPath, request.FileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            await SendNoContentAsync(cancellationToken);
        }
        else
        {
            await SendNotFoundAsync(cancellationToken);
        }
    }
}