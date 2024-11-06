using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmpTicketingController : BaseController
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

            if (employeeTickets.Any())
            {
                model.ticketinfo = employeeTickets.First();
            }

            return View("~/Views/EmployeeDashboard/EmpTicketing.cshtml", model);
        }

        public ActionResult EmpTicketRaise(int ticketnumber = 0)
        {
            var model = new TicketingModel();
            var cuserContext = SiteContext.GetCurrentUserContext();

            if (ticketnumber != 0)
            {
                model.IsEditRecord = true;
                model.ticketinfo = _dbContext.IT_Ticket.Where(t => t.TicketNo == ticketnumber).FirstOrDefault();
            }

            return View("~/Views/EmployeeDashboard/EmpRaiseTicket.cshtml", model);
        }

        public ActionResult EmpTicketView()
        {
            var model = new TicketingModel(); // Instantiate the model
            var cuserContext = SiteContext.GetCurrentUserContext();
            var employeeId = cuserContext.EmpInfo.EmployeeID;

            var employeeTickets = _dbContext.IT_Ticket
                                    .Where(t => t.EmployeeID == employeeId)
                                    .OrderByDescending(x => x.Created_date)
                                    .ToList();
            model.empTickets = employeeTickets;
            model.ticketinfo = employeeTickets.FirstOrDefault() ?? new IT_Ticket();


            return View("~/Views/EmployeeDashboard/EmpTicketView.cshtml", model);
        }

        [HttpPost]
        public JsonResult RaiseTicket()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

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

                string isEdiatbleRecord = Request.Form["IsEditRecord"].ToString().ToLowerInvariant();
                string isEdiatbleRecordTicketNumber = Request.Form["EdiatbleRecordNumber"].ToString().ToLowerInvariant();

                // Get file from FormData
                HttpPostedFileBase file = Request.Files["File"];

                string fileName = "";

                // Validate file
                if (file != null)
                {
                    if ((file.ContentType == "image/jpeg" || file.ContentType == "image/png") && file.ContentLength <= 2097152)
                    {
                        // Save the file in a virtual folder
                        fileName = Path.GetFileName(file.FileName);
                        var TicketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];

                        TicketingFolderPath = TicketingFolderPath + "/" + ticketType;

                        string filePath = Path.Combine(TicketingFolderPath, fileName);
                        file.SaveAs(filePath);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid file type or size." });
                    }
                }

                var raisedTicket = new IT_Ticket();

                var Message = "Ticket has been raised successfully.";

                if (isEdiatbleRecord == "false")
                {
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
                        AttatchimageFile = fileName // Save file path to model
                    };

                    // Save the model to the database
                    raisedTicket = _dbContext.IT_Ticket.Add(ticketModel);
                    Message = "Ticket has been raised successfully.";
                }
                else
                {
                    var ticketNum = System.Convert.ToInt32(isEdiatbleRecordTicketNumber);
                    raisedTicket = _dbContext.IT_Ticket.Where(x => x.TicketNo == ticketNum).FirstOrDefault();

                    if (raisedTicket != null)
                    {
                        raisedTicket.TicketType = ticketType;
                        raisedTicket.Category = category;
                        raisedTicket.Subject = subject;
                        raisedTicket.Description = description;
                        raisedTicket.Priority = priority;
                        raisedTicket.EmployeeID = employeeId;
                        raisedTicket.EmployeeName = employeeName;
                        raisedTicket.OfficialEmailID = officialEmail;
                        raisedTicket.Status = status;
                        raisedTicket.Location = location;
                        raisedTicket.Created_date = DateTime.Now;
                        if (file != null)
                        {
                            raisedTicket.AttatchimageFile = fileName;
                        }

                        Message = "Ticket has been updated successfully.";
                    }
                }


                _dbContext.SaveChanges();


                var newNotification = new Notification
                {
                    NotificationDate = DateTime.Now,
                    NotificationFromName = cuserContext.EmpInfo.EmployeeName,
                    NotificationFromID = cuserContext.EmpInfo.EmployeeID,
                    NotificationToName = employeeName,
                    NotificationToID = employeeId,
                    NotificationType = "Ticket",
                    Status = "Submitted",
                    ReferenceNumber = raisedTicket.TicketNo.ToString(),
                    Comments = "",
                    CreatedDate = DateTime.Now
                };

                _dbContext.Notifications.Add(newNotification);
                _dbContext.SaveChanges();


                TicketingHelper.SendTicketConfirmationEmail(raisedTicket);
                return Json(new { success = true, message = Message });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public JsonResult CancelTicket(int ticketName, string status)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            try
            {
                var cancellTicket = _dbContext.IT_Ticket.Where(x => x.TicketNo == ticketName).FirstOrDefault();
                if (cancellTicket != null)
                {
                    cancellTicket.Status = status;
                    _dbContext.SaveChanges();

                    var newNotification = new Notification
                    {
                        NotificationDate = DateTime.Now,
                        NotificationFromName = cuserContext.EmpInfo.EmployeeName,
                        NotificationFromID = cuserContext.EmpInfo.EmployeeID,
                        NotificationToName = cancellTicket.EmployeeName,
                        NotificationToID = cancellTicket.EmployeeID,
                        NotificationType = "Ticket",
                        Status = status,
                        ReferenceNumber = cancellTicket.TicketNo.ToString(),
                        Comments = "",
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.Notifications.Add(newNotification);
                    _dbContext.SaveChanges();


                    TicketingHelper.SendTicketConfirmationEmail(cancellTicket);
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
            var cuserContext = SiteContext.GetCurrentUserContext();


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

                    var newNotification = new Notification
                    {
                        NotificationDate = DateTime.Now,
                        NotificationFromName = cuserContext.EmpInfo.EmployeeName,
                        NotificationFromID = cuserContext.EmpInfo.EmployeeID,
                        NotificationToName = ticket.EmployeeName,
                        NotificationToID = ticket.EmployeeID,
                        NotificationType = "Ticket",
                        Status = status,
                        ReferenceNumber = ticket.TicketNo.ToString(),
                        Comments = "",
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.Notifications.Add(newNotification);
                    _dbContext.SaveChanges();

                    TicketingHelper.SendTicketConfirmationEmail(ticket);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}