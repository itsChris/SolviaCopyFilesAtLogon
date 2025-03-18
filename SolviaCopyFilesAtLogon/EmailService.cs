using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SolviaCopyFilesAtLogon
{
    public static class EmailService
    {
        public static void SendReport()
        {
            try
            {
                string smtpServer = ConfigHelper.GetSetting("SmtpServer");
                int smtpPort = ConfigHelper.GetIntSetting("SmtpPort");
                string senderEmail = ConfigHelper.GetSetting("SenderEmail");
                string recipientEmail = ConfigHelper.GetSetting("RecipientEmail");

                string hostname = Environment.MachineName;
                string username = Environment.UserName;
                string subject = $"Solvia - Copy File Report [{hostname} - {username}]";

                string body = GenerateEmailBody();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail, "Solvia - Copy File Tool");
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                        smtpClient.Send(mail);
                    }
                }

                LoggingService.Log("Email sent successfully.");
            }
            catch (Exception ex)
            {
                LoggingService.Log($"Error sending email: {ex}", true);
            }
        }

        private static string GenerateEmailBody()
        {
            StringBuilder emailBody = new StringBuilder();
            string hostname = Environment.MachineName;
            string username = Environment.UserName;

            emailBody.AppendLine("<html><body>");
            emailBody.AppendLine("<h2 style='color: #2E86C1;'>Solvia - Copy File Report</h2>");
            emailBody.AppendLine($"<p><strong>Hostname:</strong> {hostname}</p>");
            emailBody.AppendLine($"<p><strong>Username:</strong> {username}</p>");
            emailBody.AppendLine("<hr>");
            emailBody.AppendLine("<h3>Copied Files</h3>");
            emailBody.AppendLine("<table border='1' cellspacing='0' cellpadding='5' style='border-collapse: collapse;'>");
            emailBody.AppendLine("<tr style='background-color: #D6EAF8;'><th>Source</th><th>Destination</th><th>Size (bytes)</th><th>Result</th></tr>");

            foreach (var file in FileCopyService.CopiedFiles)
            {
                string color = file.CopyResult == "Success" ? "#28B463" : "#E74C3C";
                emailBody.AppendLine($"<tr><td>{file.Source}</td><td>{file.Destination}</td><td>{file.FileSize}</td><td style='color:{color};'>{file.CopyResult}</td></tr>");
            }

            emailBody.AppendLine("</table>");
            emailBody.AppendLine("<p style='color: #7D3C98;'><em>Generated automatically by Solvia - Copy File Tool</em></p>");
            emailBody.AppendLine("</body></html>");

            return emailBody.ToString();
        }
    }
}
