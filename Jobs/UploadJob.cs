using System.Text;
using CliWrap;
using Microsoft.Extensions.Options;
using PGBackup.Helpers;
using Quartz;

namespace PGBackup.Jobs;

[DisallowConcurrentExecution]
public class UploadJob : IJob
{
    private readonly ILogger<UploadJob> _logger;
    private readonly IWebHostEnvironment _host;
    private readonly FileUploader _fileUploader;

    public UploadJob(ILogger<UploadJob> logger, IWebHostEnvironment host, FileUploader fileUploader)
    {
        _logger = logger;
        _host = host;
        _fileUploader = fileUploader;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.MergedJobDataMap;
        string? fileName = dataMap.GetString("fileName");
        _logger.LogInformation("Uploading file {fileName}", fileName);

        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogInformation("No file");
            return;
        }

        string backupPath = Path.Combine(_host.ContentRootPath, "backup");
        string filePath = Path.Combine(backupPath, fileName);
        if (File.Exists(filePath))
        {
            _logger.LogInformation("Uploading file to S3");
            using var fs = new FileStream(filePath, FileMode.Open);
            await _fileUploader.UploadFileAsync(fs, fileName, context.CancellationToken);
            _logger.LogInformation("Finished uploading to S3");
        }
    }
}