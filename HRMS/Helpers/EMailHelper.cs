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
                mail.Subject = request.Subject;
                mail.Body = request.Body;

                SmtpClient smtpClient = new SmtpClient(smtpHost, System.Convert.ToInt32(smtpPort));
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtpClient.EnableSsl = true;

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
    }
}