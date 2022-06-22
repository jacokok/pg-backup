namespace PGBackup.Features.Cloud.GetBackups;

public class Response
{
    public List<FileDetail> Files { get; set; } = new();
}

public class FileDetail
{
    public string Name { get; set; } = "";
    public string Key { get; set; } = "";
    public long Size { get; set; }
    public string StorageClass { get; set; } = "";
    public string BucketName { get; set; } = "";
    public DateTime LastModified { get; set; }
}