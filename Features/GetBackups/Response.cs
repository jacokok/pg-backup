namespace PGBackup.Features.GetBackups;

public class Response
{
    public List<FileDetail> Files { get; set; } = new();
}

public class FileDetail
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public long Size { get; set; }
}