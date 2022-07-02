using Humanizer;
using PGBackup.Helpers;

namespace PGBackup.Features.Backup.GetBackups;

public class GetBackups : EndpointWithoutRequest<List<FileDetail>>
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
        List<FileDetail> response = new();
        foreach (string file in filePaths)
        {
            long fileSize = FileHelper.GetFileSize(file);
            DateTime? lastModified = FileHelper.GetLastModifiedTime(file);
            response.Add(new FileDetail
            {
                Name = Path.GetFileName(file),
                Path = file,
                Size = fileSize,
                NiceSize = fileSize.Bytes().Humanize(),
                LastModified = lastModified,
                NiceLastModified = lastModified.Humanize()
            });
        }
        await SendAsync(response, cancellation: cancellationToken);
    }
}