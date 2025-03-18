using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SolviaCopyFilesAtLogon
{
    public static class FileCopyService
    {
        private const int WM_FONTCHANGE = 0x001D;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static List<FileObj> CopiedFiles { get; private set; } = new List<FileObj>();

        public static bool CopyFilesRecursive(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                LoggingService.Log($"Source directory does not exist: {sourceDir}", true);
                return false;
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            try
            {
                foreach (string file in Directory.GetFiles(sourceDir))
                {
                    string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                    try
                    {
                        File.Copy(file, targetFile, true);
                        var fileInfo = new FileInfo(targetFile);
                        CopiedFiles.Add(new FileObj(file, targetFile, fileInfo.Length, "Success"));
                        LoggingService.Log($"Copied: {file} -> {targetFile}");

                        // Check if file is a font and register it
                        if (IsFontFile(file))
                        {
                            RegisterFont(targetFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        CopiedFiles.Add(new FileObj(file, targetFile, 0, $"Error: {ex.Message}"));
                        LoggingService.Log($"Failed: {file} -> {targetFile}, Error: {ex.Message}", true);
                    }
                }

                foreach (string dir in Directory.GetDirectories(sourceDir))
                {
                    string subTargetDir = Path.Combine(targetDir, Path.GetFileName(dir));
                    CopyFilesRecursive(dir, subTargetDir);
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggingService.Log($"Error during file copy: {ex.Message}", true);
                return false;
            }
        }

        private static bool IsFontFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".ttf" || extension == ".otf" || extension == ".woff" || extension == ".woff2";
        }

        private static void RegisterFont(string fontPath)
        {
            try
            {
                string fontName = Path.GetFileName(fontPath);
                string localFontDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts");
                string destinationPath = Path.Combine(localFontDir, fontName);

                // Ensure font directory exists
                Directory.CreateDirectory(localFontDir);

                // Move font file if necessary
                if (!File.Exists(destinationPath))
                {
                    File.Copy(fontPath, destinationPath, true);
                }

                // Register font in registry
                string fontRegistryName = Path.GetFileNameWithoutExtension(fontName);
                string fontRegistryPath = @"Software\Microsoft\Windows NT\CurrentVersion\Fonts";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(fontRegistryPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue(fontRegistryName, destinationPath, RegistryValueKind.String);
                    }
                }

                // Notify Windows about font change
                SendMessage(HWND_BROADCAST, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);

                LoggingService.Log($"Font '{fontName}' registered successfully.");
            }
            catch (Exception ex)
            {
                LoggingService.Log($"Error registering font: {ex.Message}", true);
            }
        }
    }

    public class FileObj
    {
        public string Source { get; }
        public string Destination { get; }
        public long FileSize { get; }
        public string CopyResult { get; }

        public FileObj(string source, string destination, long fileSize, string copyResult)
        {
            Source = source;
            Destination = destination;
            FileSize = fileSize;
            CopyResult = copyResult;
        }
    }
}
