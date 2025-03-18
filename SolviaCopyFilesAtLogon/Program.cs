using System;

namespace SolviaCopyFilesAtLogon
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggingService.Log("=== Solvia Copy Files At Logon - Start ===");

            try
            {
                var sourceDir = ConfigHelper.GetExpandedSetting("SourceDirectory");
                var targetDir = ConfigHelper.GetExpandedSetting("TargetDirectory");
                bool sendEmail = ConfigHelper.GetBoolSetting("SendEmail");
                bool debugMode = ConfigHelper.GetBoolSetting("DebugMode");

                if (!FileCopyService.CopyFilesRecursive(sourceDir, targetDir))
                {
                    LoggingService.Log("File copy operation encountered errors.", true);
                }

                if (sendEmail)
                {
                    EmailService.SendReport();
                }
            }
            catch (Exception ex)
            {
                LoggingService.Log($"Unexpected error: {ex.Message}", true);
            }

            LoggingService.Log("=== Solvia Copy Files At Logon - Finished ===\n");

            if (ConfigHelper.GetBoolSetting("DebugMode"))
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
