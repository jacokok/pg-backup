using Humanizer;
using PGBackup.Helpers;

namespace PGBackup.Features.Backup.GetBackups;

public class GetBackups : EndpointWithoutRequest<Response>
{
    private readonly IWebHostEnvironment _host;

    public GetBackups(IWebHostEnvironment host)
    {
        _host = host;
    }
    public override void Configure()
    {
        Get("/backup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(_host.ContentRootPath, "backup");
        string[] filePaths = Directory.GetFiles(backupPath, "*.dump", SearchOption.TopDirectoryOnly);
        Response response = new();
        foreach (string file in filePaths)
        {
            long fileSize = FileHelper.GetFileSize(file);
            response.Files.Add(new FileDetail
            {
                Name = Path.GetFileName(file),
                Path = file,
                Size = fileSize,
                NiceSize = fileSize.Bytes().Humanize()
            });
        }

        await SendAsync(response, cancellation: cancellationToken);
    }
}