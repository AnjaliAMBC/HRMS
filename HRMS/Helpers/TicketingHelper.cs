using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public static class TicketingHelper
    {
        public static string GetPriorityColor(string priority)
        {
            switch (priority.ToLower())
            {
                case "high":
                    return "res-emp-ticketlisting-color-red";
                case "mid":
                    return "res-emp-ticketlisting-color-orange";
                case "low":
                    return "res-emp-ticketlisting-color-green";
                default:
                    return ""; // Default or unknown priority
            }
        }                 
        public static string GetTicketStatusEmpClass(string status)
        {
            switch (status.ToLower())
            {
                case "open":
                    return "status-pending";
                case "closed":
                    return "status-resolved";
                case "resolved":
                    return "status-resolved";
                case "re-open":
                    return "status-pending";
                case "cancelled":
                    return "status-pending";
                default:
                    return "";
            }
        }

        public static string GetTicketStatusDashClass(string status)
        {
            switch (status.ToLower())
            {
                case "open":
                    return "status-pending";
                case "closed":
                    return "status-closed";
                case "resolved":
                    return "status-closed";
                case "re-open":
                    return "status-pending";
                case "cancelled":
                    return "status-pending";
                default:
                    return "";
            }
        }

        public static string GetTicketStatusCssClass(string status)
        {
            switch (status.ToLower())
            {
                case "open":
                    return "ithrticket-status-open";
                case "closed":
                    return "ithrticket-status-closed";
                case "resolved":
                    return "ticket-status-pending";
                case "re-open":
                    return "ticket-status-re-open";
                case "cancelled":
                    return "ticket-status-cancelled";
                default:
                    return "";
            }
        }

        public static string GetTicketPriorityDashCssClass(string status)
        {
            switch (status.ToLower())
            {
                case "low":
                    return "badge-low";
                case "mid":
                    return "badge-mid";
                case "high":
                    return "badge-danger";
                default:
                    return "";
            }
        }

        public static string GetMyticketDashPriorityColor(string priority)
        {
            switch (priority.ToLower())
            {
                case "high":
                    return "dash-ticket-priority-high";
                case "mid":
                    return "dash-ticket-priority-mid";
                case "low":
                    return "dash-ticket-priority-low";
                default:
                    return ""; // Default or unknown priority
            }
        }


        public static void SendTicketConfirmationEmail(IT_Ticket ticket)
        {
            var siteURL = ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";

            string body = $@"
<html>
<head>
    <style>
        .email-body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f4f4f4;
        }}
        .email-content {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            position: relative;
        }}
        .email-header {{
            display: flex;
            align-items: flex-start;
        }}
       
 .email-logo img {{
            display: block;
        }}
        .email-header .email-logo {{
            margin-left: auto;
            margin-bottom: 10px;
        }}
        .email-footer {{
            margin-top: 20px;
            text-align: center;
            font-size: 12px;
            color: #888888;
        }}
        .email-table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        .email-table th, .email-table td {{
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
        }}
        .email-table th {{
            background-color: #f2f2f2;
        }}
    </style>
</head>
<body>
    <div class='email-body'>
        <div class='email-content'>
            <div class='email-header'>
                <div style='flex:1;'>
                    <h2>Hi Team,</h2>
                    <p>Employee <strong>{ticket.EmployeeName}</strong> with ID <strong>{ticket.EmployeeID}</strong> has raised a ticket no #<strong>{ticket.TicketNo}</strong>.</p>
                    <p><strong>Category:</strong> {ticket.Category}</p>
                    <p><strong>Subject:</strong> {ticket.Subject}</p>
                    <p><strong>Status:</strong> {ticket.Status}</p>
                    <p><strong>Date:</strong> {System.DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}</p>
                </div>
                <div class='email-logo'>
                    <img src='{logoURL}' alt='Company Logo' width='70'>
                </div>
            </div>
            <table class='email-table'>
                <tr>
                    <th>Field</th>
                    <th>Details</th>
                </tr>
                <tr>
                    <td>Ticket Type</td>
                    <td>{ticket.TicketType}</td>
                </tr>
                <tr>
                    <td>Category</td>
                    <td>{ticket.Category}</td>
                </tr>
                <tr>
                    <td>Subject</td>
                    <td>{ticket.Subject}</td>
                </tr>
                <tr>
                    <td>Description</td>
                    <td>{ticket.Description}</td>
                </tr>
                <tr>
                    <td>Priority</td>
                    <td>{ticket.Priority}</td>
                </tr>                    
                <tr>
                    <td>Location</td>
                    <td>{ticket.Location}</td>
                </tr>
            </table>
        </div>
        <div class='email-footer'>
            <p>This is an automated email, please do not reply.</p>
            <p>Automated mail from <a href='{siteURL}'>{siteURL}</a></p>
        </div>
    </div>
</body>
</html>";


            // Determine the recipient email based on ticket type
            string recipientEmail;
            if (ticket.TicketType.Equals("IT", StringComparison.OrdinalIgnoreCase))
            {
                recipientEmail = ConfigurationManager.AppSettings["ITSupportGroupEmail"];
            }
            else
            {
                recipientEmail = ConfigurationManager.AppSettings["HRSupportGroupEmail"];
            }

            // Create the email request with the determined recipient email
            var emailRequest = new EmailRequest()
            {
                Body = body,
                ToEmail = recipientEmail,
                Subject = "Ticket Notification: #" + ticket.TicketNo + " - " + ticket.Status,
                CCEmail = ticket.OfficialEmailID
            };


            EMailHelper.SendEmail(emailRequest);
        }
    }
}