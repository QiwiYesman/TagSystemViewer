using System;
using System.Diagnostics;
using System.IO;

namespace TagSystemViewer.Utility;

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

    public static bool MoveFile(string fromFileName, string toFileName)
    {
        if (!File.Exists(fromFileName)) return false;
        try
        {
            File.Move(fromFileName, toFileName, true);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
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