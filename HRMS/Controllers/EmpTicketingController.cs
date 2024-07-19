using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmpTicketingController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;

        public EmpTicketingController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        // GET: EmpTicketing
        public ActionResult Index()
        {
            var model = new TicketingModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            var employeeId = cuserContext.EmpInfo.EmployeeID;

            var employeeTickets = _dbContext.IT_Ticket.Where(t => t.EmployeeID == employeeId).OrderByDescending(x => x.Created_date).ToList();
            model.empTickets = employeeTickets;
            return View("~/Views/EmployeeDashboard/EmpTicketing.cshtml", model);
        }
        public ActionResult EmpTicketRaise()
        {

            return PartialView("~/Views/EmployeeDashboard/EmpRaiseTicket.cshtml");
        }

        [HttpPost]
        public JsonResult RaiseTicket()
        {
            try
            {
                // Get individual form values from FormData
                string ticketType = Request.Form["TicketType"];
                string category = Request.Form["Category"];
                string subject = Request.Form["Subject"];
                string description = Request.Form["Description"];
                string priority = Request.Form["Priority"];
                string employeeId = Request.Form["EmployeeID"];
                string employeeName = Request.Form["EmployeeName"];
                string officialEmail = Request.Form["OfficialEmailID"];
                string status = Request.Form["Status"];
                string location = Request.Form["Location"];

                // Get file from FormData
                HttpPostedFileBase file = Request.Files["File"];

                // Validate file
                if (file != null && (file.ContentType == "image/jpeg" || file.ContentType == "image/png") && file.ContentLength <= 2097152)
                {
                    // Save the file in a virtual folder
                    var fileName = Path.GetFileName(file.FileName);
                    var TicketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];

                    TicketingFolderPath = TicketingFolderPath + "/" + ticketType;

                    var path = Path.Combine(TicketingFolderPath, fileName);
                    file.SaveAs(path);

                    // Create and populate the IT_Ticket model
                    var ticketModel = new IT_Ticket
                    {
                        TicketType = ticketType,
                        Category = category,
                        Subject = subject,
                        Description = description,
                        Priority = priority,
                        EmployeeID = employeeId,
                        EmployeeName = employeeName,
                        OfficialEmailID = officialEmail,
                        Status = status,
                        Location = location,
                        Created_date = DateTime.Now,
                        AttatchimageFile = path // Save file path to model
                    };

                    // Save the model to the database
                    _dbContext.IT_Ticket.Add(ticketModel);
                    _dbContext.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid file type or size." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult CancelTicket(int ticketName, string status)
        {
            try
            {
                var cancellTicket = _dbContext.IT_Ticket.Where(x => x.TicketNo == ticketName).FirstOrDefault();
                if (cancellTicket != null)
                {
                    cancellTicket.Status = status;
                    _dbContext.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult TicketStatusChangeByEmp(int ticketName, string status, string comments, string updateby, string updatebyID)
        {
            try
            {
                var ticket = _dbContext.IT_Ticket.Where(x => x.TicketNo == ticketName).FirstOrDefault();
                if (ticket != null)
                {
                    if (status == "Re-Open")
                    {
                        ticket.Status = "Re-Open";
                        ticket.ReopenedComments = comments;
                        ticket.ReopenedDate = DateTime.Now;
                        ticket.ResolvedByName = updateby;
                    }

                    if (status == "Closed")
                    {
                        ticket.Status = "Closed";
                        ticket.AcknowledgeComments = comments;
                        ticket.isacknowledge = "true";
                        ticket.Closed_date = DateTime.Now;
                        ticket.ClosedByName = updateby;
                        ticket.Closedby = updatebyID;
                    }

                    _dbContext.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void SendTicketConfirmationEmail(IT_Ticket ticket)
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
            }}
            .email-header {{
                display: flex;
                justify-content: space-between;
                align-items: center;
            }}
            .email-logo img {{
                max-width: 100px;
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
                    <div>
                        <h2>Hi Team,</h2>
                        <p>Employee <strong>{ticket.EmployeeName}</strong> with ID <strong>{ticket.EmployeeID}</strong> has raised a ticket no #<strong>{ticket.TicketNo}</strong>.</p>
                        <p><strong>Category:</strong> {ticket.Category}</p>
                        <p><strong>Subject:</strong> {ticket.Subject}</p>
                        <p><strong>Status:</strong> {ticket.Status}</p>
                        <p><strong>Created Date:</strong> {ticket.Created_date}</p>
                    </div>
                    <div class='email-logo'>
                        <img src='{logoURL}' alt='Company Logo'>
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

            var emailRequest = new EmailRequest()
            {
                Body = body,
                ToEmail = ticket.OfficialEmailID,
                Subject = "Ticket Raised Notification",
            };

            EMailHelper.SendEmail(emailRequest);
        }
    }
}