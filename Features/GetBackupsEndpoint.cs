using Quartz;

namespace PGBackup.Features;

public class GetBackups : EndpointWithoutRequest<object>
{
    private readonly IWebHostEnvironment _host;

    public GetBackups(IWebHostEnvironment host)
    {
        _host = host;
    }
    public override void Configure()
    {
        Get("/backups");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(_host.ContentRootPath, "backup");
        string[] filePaths = Directory.GetFiles(backupPath, "*.dump", SearchOption.TopDirectoryOnly);
        await SendStringAsync(filePaths[0], cancellation: cancellationToken);
    }
}