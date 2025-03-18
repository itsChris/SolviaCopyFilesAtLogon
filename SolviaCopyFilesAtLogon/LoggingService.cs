using System;
using System.IO;
using System.Text;

namespace SolviaCopyFilesAtLogon
{
    public static class LoggingService
    {
        private static readonly string logDirectory;
        private static readonly string logFilePath;

        static LoggingService()
        {
            // Primärer Speicherort: %LOCALAPPDATA%\SolviaCopyFilesAtLogon\
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localAppDataPath))
            {
                logDirectory = Path.Combine(localAppDataPath, "SolviaCopyFilesAtLogon");
            }
            else
            {
                // Fallback: C:\Temp\SolviaCopyFilesAtLogon\
                logDirectory = Path.Combine("C:\\Temp", "SolviaCopyFilesAtLogon");
            }

            logFilePath = Path.Combine(logDirectory, "log.txt");

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error creating log directory: {ex.Message}");
                Console.ResetColor();
            }
        }

        public static void Log(string message, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();

            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error writing to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
