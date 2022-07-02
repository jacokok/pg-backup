using Humanizer;

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
            FileInfo fileInfo = new(file);
            response.Add(new FileDetail
            {
                Name = fileInfo.Name,
                Path = file,
                Size = fileInfo.Length,
                NiceSize = fileInfo.Length.Bytes().Humanize(),
                LastModified = fileInfo.LastWriteTime,
                NiceLastModified = fileInfo.LastWriteTime.Humanize()
            });
        }
        await SendAsync(response, cancellation: cancellationToken);
    }
}