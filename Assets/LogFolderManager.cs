using System.IO;
using System;
using System.Diagnostics;

public static class LogFolderManager
{
    private static string folderPath = null;

    public static string GetFolderPath()
    {
        if (folderPath == null)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logsFolderPath = Path.Combine(desktopPath, "Logs");

            if (!Directory.Exists(logsFolderPath))
            {
                Directory.CreateDirectory(logsFolderPath);
            }

            string folderName = DateTime.Now.ToString("dd_MM_yyyy-HH-mm-ss");
            folderPath = Path.Combine(logsFolderPath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        return folderPath;
    }
}
