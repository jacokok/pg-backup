namespace PGBackup.Helpers;

public static class FileHelper
{
    public static long GetFileSize(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }
            return 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static DateTime? GetLastModifiedTime(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).LastWriteTime;
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}