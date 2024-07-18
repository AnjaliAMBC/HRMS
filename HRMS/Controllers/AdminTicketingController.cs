using HRMS.Helpers;
using HRMS.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminTicketingController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;

        public AdminTicketingController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: AdminHrTicketing
        public ActionResult Index()
        {
            var tickets = _dbContext.IT_Ticket.ToList();
            return View(tickets);
        }

        public ActionResult EmpTicketList()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();


            var employeeId = cuserContext.EmpInfo.EmployeeID; // Retrieve this from session or authentication context
            var employeeTickets = _dbContext.IT_Ticket.Where(t => t.EmployeeID == employeeId).ToList();
            return View(employeeTickets);
        }

        public ActionResult HrTicketing()
        {
            var model = new TicketingModel();

            var employeeTickets = _dbContext.IT_Ticket.Where(x => x.TicketType == "HR").OrderByDescending(x => x.Created_date);
            model.empTickets = employeeTickets.ToList();

            model.itEmployees = _dbContext.emp_info.Where(x => x.Department == "HR").ToList();
            return View("~/Views/AdminDashboard/AdminHrTicketing.cshtml", model);
        }


        public ActionResult GetHrTicketFilter(string fromDate, string toDate, string status, string location, string closedBy)
        {
            List<IT_Ticket> ticketsList = GetTicketsFilter(fromDate, toDate, status, location, closedBy, "HR");
            var json = JsonConvert.SerializeObject(ticketsList);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetHRTicketDetails(int ticketNo)
        {
            var model = new TicketDetailsViewModel();
            var ticket = _dbContext.IT_Ticket.FirstOrDefault(t => t.TicketNo == ticketNo);
            if (ticket != null)
            {
                model.TicketViewModel = ticket;
            }

            var hrEMployees = _dbContext.emp_info.Where(x => x.Department == "HR").ToList();
            model.ITEmployees = hrEMployees;

            return View("~/Views/AdminDashboard/AdminHrTicketOpenCloseView.cshtml", model);
        }

        public ActionResult GetITTicketFilter(string fromDate, string toDate, string status, string location, string closedBy)
        {
            List<IT_Ticket> ticketsList = GetTicketsFilter(fromDate, toDate, status, location, closedBy, "IT");
            var json = JsonConvert.SerializeObject(ticketsList);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItTicketing()
        {
            var model = new TicketingModel();

            var employeeTickets = _dbContext.IT_Ticket.Where(x => x.TicketType == "IT").OrderByDescending(x => x.Created_date);
            model.empTickets = employeeTickets.ToList();

            model.itEmployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();

            return View("~/Views/AdminDashboard/AdminTicketingView.cshtml", model);
        }
        public ActionResult GetItTicketDetails(int ticketNo)
        {
            var model = new TicketDetailsViewModel();
            var ticket = _dbContext.IT_Ticket.FirstOrDefault(t => t.TicketNo == ticketNo);
            if (ticket != null)
            {
                model.TicketViewModel = ticket;

            }

            var itEMployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();
            model.ITEmployees = itEMployees;

            return View("~/Views/AdminDashboard/AdminItTicketOpenClose.cshtml", model);
        }



        public ActionResult HRExportToExcel(string fromDate, string toDate, string status, string location, string closedBy)
        {
            List<IT_Ticket> ticketsList = GetTicketsFilter(fromDate, toDate, status, location, closedBy, "HR");

            // Create Excel package
            ExcelPackage excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Tickets");

            // Header row
            worksheet.Cells["A1"].Value = "Employee ID";
            worksheet.Cells["B1"].Value = "Subject";
            worksheet.Cells["C1"].Value = "Priority";
            worksheet.Cells["D1"].Value = "Location";
            worksheet.Cells["E1"].Value = "Status";
            worksheet.Cells["F1"].Value = "Created Date";

            // Data rows
            int row = 2;
            foreach (var ticket in ticketsList)
            {
                worksheet.Cells[string.Format("A{0}", row)].Value = ticket.EmployeeID;
                worksheet.Cells[string.Format("B{0}", row)].Value = ticket.Subject;
                worksheet.Cells[string.Format("C{0}", row)].Value = ticket.Priority;
                worksheet.Cells[string.Format("D{0}", row)].Value = ticket.Location;
                worksheet.Cells[string.Format("E{0}", row)].Value = ticket.Status;
                worksheet.Cells[string.Format("F{0}", row)].Value = ticket.Created_date.HasValue ? ticket.Created_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A";

                row++;
            }

            // Auto fit columns
            worksheet.Cells.AutoFitColumns();

            // Prepare the response
            var memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);
            memoryStream.Position = 0;

            string excelName = $"HR-TickcketHistory-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private List<IT_Ticket> GetTicketsFilter(string fromDate, string toDate, string status, string location, string closedBy, string type)
        {
            DateTime dateStart = System.DateTime.Parse(fromDate);
            DateTime dateEnd = System.DateTime.Parse(toDate);

            var employeeTickets = _dbContext.IT_Ticket.Where(x => x.TicketType == type);

            if (fromDate != null && !string.IsNullOrEmpty(fromDate))
            {
                employeeTickets = employeeTickets.Where(x => x.Created_date >= dateStart);
            }

            if (toDate != null && !string.IsNullOrEmpty(toDate))
            {
                employeeTickets = employeeTickets.Where(x => x.Created_date <= dateEnd);
            }

            if (status != "null" && status != "All" && !string.IsNullOrEmpty(status))
            {
                employeeTickets = employeeTickets.Where(x => x.Status == status);
            }

            if (location != "null" && location != "All" && !string.IsNullOrEmpty(location))
            {
                employeeTickets = employeeTickets.Where(x => x.Location == location);
            }

            if (closedBy != "null" && closedBy != "All" && !string.IsNullOrEmpty(closedBy))
            {
                employeeTickets = employeeTickets.Where(x => x.Closedby == closedBy);
            }

            var ticketsList = employeeTickets.ToList();
            return ticketsList;
        }

        [HttpPost]
        public JsonResult UpdateTicketStatus(IT_Ticket ticketModel)
        {
            try
            {
                var ticket = _dbContext.IT_Ticket.FirstOrDefault(x => x.TicketNo == ticketModel.TicketNo);
                if (ticket != null)
                {
                    ticket.Status = ticketModel.Status;

                    if (ticket.Status == "Resolved")
                    {
                        ticket.Resolved_by = ticketModel.Resolved_by;
                        ticket.ResolvedDate = DateTime.Now;

                        if (ticket.Created_date.HasValue)
                        {
                            TimeSpan timeDifference = ticket.ResolvedDate.Value - ticket.Created_date.Value;
                            ticket.ResponseTime = (long)timeDifference.TotalSeconds;
                        }

                    }
                    else if (ticket.Status == "Re-Open")
                    {
                        ticket.ReopenedDate = DateTime.Now;
                        ticket.ReopenedComments = ticketModel.ReopenedComments;
                    }
                    else if (ticket.Status == "Closed")
                    {
                        ticket.Closedby = ticketModel.Closedby;
                        ticket.AcknowledgeComments = ticketModel.AcknowledgeComments;
                        ticket.Closed_date = DateTime.Now;
                    }

                    else if (ticket.isacknowledge == "true")
                    {
                        ticket.Closedby = ticketModel.Closedby;
                        ticket.AcknowledgeComments = ticketModel.AcknowledgeComments;
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

        //Admin IT Ticketing export and filter 
        public ActionResult ITExportToExcel(string fromDate, string toDate, string status, string location, string closedBy)
        {
            List<IT_Ticket> ticketsList = GetTicketsFilter(fromDate, toDate, status, location, closedBy, "IT");

            // Create Excel package
            ExcelPackage excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Tickets");

            // Header row
            worksheet.Cells["A1"].Value = "Employee ID";
            worksheet.Cells["B1"].Value = "Subject";
            worksheet.Cells["C1"].Value = "Priority";
            worksheet.Cells["D1"].Value = "Location";
            worksheet.Cells["E1"].Value = "Status";
            worksheet.Cells["F1"].Value = "Created Date";

            // Data rows
            int row = 2;
            foreach (var ticket in ticketsList)
            {
                worksheet.Cells[string.Format("A{0}", row)].Value = ticket.EmployeeID;
                worksheet.Cells[string.Format("B{0}", row)].Value = ticket.Subject;
                worksheet.Cells[string.Format("C{0}", row)].Value = ticket.Priority;
                worksheet.Cells[string.Format("D{0}", row)].Value = ticket.Location;
                worksheet.Cells[string.Format("E{0}", row)].Value = ticket.Status;
                worksheet.Cells[string.Format("F{0}", row)].Value = ticket.Created_date.HasValue ? ticket.Created_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A";

                row++;
            }

            // Auto fit columns
            worksheet.Cells.AutoFitColumns();

            // Prepare the response
            var memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);
            memoryStream.Position = 0;

            string excelName = $"IT-TickcketHistory-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public ActionResult GetTicketDetailsByNumber(int ticketNo)
        {
            var ticket = _dbContext.IT_Ticket.FirstOrDefault(t => t.TicketNo == ticketNo);
            var json = JsonConvert.SerializeObject(ticket);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}