using System;
using System.Diagnostics;
using System.IO;

namespace TagSystemViewer.Models;

public static class FileProcess
{
    private static void StartProcess(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch
        {
            Console.WriteLine("Can't start process: " +path);
        }
    }

    public static void StartFile(string fileName)
    {
        if (!File.Exists(fileName)) return;
        StartProcess(fileName);
    }

    public static void StartDirectory(string fileName)
    {
        if (!File.Exists(fileName)) return;
        var folder = Path.GetDirectoryName(fileName);
        if (folder is null) return;
        StartProcess(folder);
    }
}