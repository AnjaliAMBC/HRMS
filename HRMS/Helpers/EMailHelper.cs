using HRMS.Models.Employee;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace HRMS.Helpers
{
    public class EMailHelper
    {
        public static EmailRequest SendEmail(EmailRequest request)
        {
            var model = request;
            try
            {
                string fromEmail = ConfigurationManager.AppSettings["SMTPUserName"]; // Replace with your email
                string fromPassword = ConfigurationManager.AppSettings["SMTPPassword"]; // Replace with your email password
                string smtpHost = ConfigurationManager.AppSettings["SMTPHost"]; // Replace with your SMTP server
                string smtpPort = ConfigurationManager.AppSettings["SMTPPort"]; // Replace with your SMTP port

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(request.ToEmail);
                if (!string.IsNullOrWhiteSpace(request.CCEmail))
                {
                    mail.CC.Add(request.CCEmail);
                }
                mail.Subject = request.Subject;
                mail.Body = request.Body;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(smtpHost, System.Convert.ToInt32(smtpPort));
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtpClient.EnableSsl = true;

                // Attach the file if AttachmentPath is provided and file exists
                if (!string.IsNullOrEmpty(request.AttachmentPath) && System.IO.File.Exists(request.AttachmentPath))
                {
                    var attachment = new Attachment(request.AttachmentPath);
                    mail.Attachments.Add(attachment);
                }

                smtpClient.Send(mail);
                model.JsonResponse.StatusCode = 200;
                model.JsonResponse.Message = "Email Sent Successfully!";
                return model;
            }
            catch (Exception ex)
            {
                model.JsonResponse.StatusCode = 500;
                model.JsonResponse.Message = "Error when sending Email!";
                return model;
            }
        }

        public static string FormatTimeSpan(TimeSpan? time)
        {
            // Check if the TimeSpan is null
            if (!time.HasValue)
            {
                return "Not specified"; // Or whatever default you prefer
            }

            // Extract the TimeSpan value
            TimeSpan t = time.Value;

            // Convert the TimeSpan into a 12-hour format with AM/PM
            int hours = t.Hours;
            int minutes = t.Minutes;

            // Determine AM or PM
            string period = hours >= 12 ? "PM" : "AM";

            // Convert hours to 12-hour format (handle midnight and noon)
            hours = (hours > 12) ? hours - 12 : (hours == 0 ? 12 : hours);

            // Format the time as hh:mm AM/PM
            return $"{hours:D2}:{minutes:D2} {period}";
        }


    }
}