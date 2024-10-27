﻿namespace Not.Filesystem;

public class FileHelper
{
    public static async Task WriteAsync(string content, string path)
    {
        CreateDirectoryIfDoesNotExist(path);
        await File.WriteAllTextAsync(path, content);
    }

    public static void Write(string content, string path)
    {
        CreateDirectoryIfDoesNotExist(path);
        File.WriteAllText(path, content);
    }

    public static async Task<string?> SafeReadStringAsync(string path)
    {
        try
        {
            return await File.ReadAllTextAsync(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string? SafeReadString(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void CreateDirectoryIfDoesNotExist(string path)
    {
        if (!File.Exists(path))
        {
            var forwardSlashIndex = path.LastIndexOf('/');
            var backwardsSlashIndex = path.LastIndexOf('\\');
            var lastSeparator = Math.Max(forwardSlashIndex, backwardsSlashIndex);
            var directoryPath = path[..lastSeparator];
            Directory.CreateDirectory(directoryPath);
        }
    }
}
