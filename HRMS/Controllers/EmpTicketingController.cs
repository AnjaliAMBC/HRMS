using HRMS.Helpers;
using HRMS.Models;
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

            var employeeTickets = _dbContext.IT_Ticket.Where(t => t.EmployeeID == employeeId).ToList();
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

        public JsonResult TicketStatusChangeByEmp(int ticketName, string status, string comments, string updateby)
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
    }
}