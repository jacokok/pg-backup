namespace PGBackup.Features.Backup.GetBackups;
public class FileDetail
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public long Size { get; set; }
    public string NiceSize { get; set; } = "";
    public DateTime? LastModified { get; set; }
    public string NiceLastModified { get; set; } = "";
}