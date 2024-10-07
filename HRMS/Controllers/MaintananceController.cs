using HRMS.Models;
using HRMS.Helpers;
using HRMS.Models.Employee;
using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.IO;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;

    public class MaintananceController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public MaintananceController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }


        public ActionResult MaintananceInfo(int year = 0, int month = 0, string location = "")
        {
            MaintananceModel model = new MaintananceModel();

            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;


            int selectedYear = year != 0 ? year : currentYear;
            int selectedMonth = month != 0 ? month : currentMonth;


            var startOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);


            model.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department == "IT").ToList();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();


            model.monthlyschedules = _dbContext.IT_Maintenance
                .Where(r => r.MaintenanceDate >= startOfMonth && r.MaintenanceDate <= endOfMonth)
                .OrderByDescending(r => r.MaintenanceDate)
                .ToList();


            if (!string.IsNullOrWhiteSpace(location) && location != "All")
            {
                model.monthlyschedules = model.monthlyschedules
                    .Where(s => s.Location == location)
                    .ToList();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("~/Views/Itsupport/_maintenanceinfotable.cshtml", model);
            }

            return View("~/Views/Itsupport/Maintanance.cshtml", model);
        }


        public ActionResult ExportToExcelMaintenance(int year = 0, int month = 0, string location = "")
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            int selectedYear = year != 0 ? year : currentYear;
            int selectedMonth = month != 0 ? month : currentMonth;

            var startOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var data = _dbContext.IT_Maintenance
               .Where(r => r.MaintenanceDate >= startOfMonth && r.MaintenanceDate <= endOfMonth)
               .OrderByDescending(r => r.MaintenanceDate)
               .ToList();


            if (!string.IsNullOrWhiteSpace(location) && location != "All")
            {
                data = data
                    .Where(s => s.Location == location)
                    .ToList();
            }


            // Generate Excel file using EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Maintenance Data");

                // Add Headers

                // Set the headers in the first row
                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "EmployeeName";
                worksheet.Cells[1, 3].Value = "EmailId";
                worksheet.Cells[1, 4].Value = "MaintenanceDate";
                worksheet.Cells[1, 5].Value = "RescheduleDate";
                worksheet.Cells[1, 6].Value = "AgentName";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "Notes";
                worksheet.Cells[1, 9].Value = "Acknowledge";
                worksheet.Cells[1, 10].Value = "Location";
                worksheet.Cells[1, 11].Value = "IssueDate";
                worksheet.Cells[1, 12].Value = "ProblemCategory";
                worksheet.Cells[1, 13].Value = "IssueFacing";
                worksheet.Cells[1, 14].Value = "NewAssetRequirement";

                // Apply styling to the header
                using (var headerRange = worksheet.Cells[1, 1, 1, 14])
                {
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.Font.Bold = true; // Optional: make the header bold
                }

                // Populate data starting from the second row
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeID ?? ""; // EmployeeID
                    worksheet.Cells[i + 2, 2].Value = data[i].EmployeeName ?? ""; // EmployeeName
                    worksheet.Cells[i + 2, 3].Value = data[i].EmailId ?? ""; // EmailId

                    // MaintenanceDate
                    worksheet.Cells[i + 2, 4].Value = data[i].MaintenanceDate.HasValue
                                                       ? data[i].MaintenanceDate.Value.ToString("yyyy-MM-dd")
                                                       : "";

                    // RescheduleDate
                    worksheet.Cells[i + 2, 5].Value = data[i].RescheduleDate.HasValue
                                                       ? data[i].RescheduleDate.Value.ToString("yyyy-MM-dd")
                                                       : "";

                    worksheet.Cells[i + 2, 6].Value = data[i].AgentName ?? ""; // AgentName
                    worksheet.Cells[i + 2, 7].Value = data[i].Status ?? ""; // Status
                    worksheet.Cells[i + 2, 8].Value = data[i].Notes ?? ""; // Notes
                    worksheet.Cells[i + 2, 9].Value = data[i].Acknowledge ?? ""; // Acknowledge
                    worksheet.Cells[i + 2, 10].Value = data[i].Location ?? ""; // Location

                    // IssueDate
                    worksheet.Cells[i + 2, 11].Value = data[i].IssueDate.HasValue
                                                        ? data[i].IssueDate.Value.ToString("yyyy-MM-dd")
                                                        : "";

                    worksheet.Cells[i + 2, 12].Value = data[i].ProblemCategory ?? ""; // ProblemCategory
                    worksheet.Cells[i + 2, 13].Value = data[i].IssueFacing ?? ""; // IssueFacing
                    worksheet.Cells[i + 2, 14].Value = data[i].NewAssetRequirement ?? ""; // NewAssetRequirement
                }


                // Set column widths (optional)
                worksheet.Cells.AutoFitColumns();

                // Save the Excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Return the file for download
                string excelFileName = "MaintenanceData.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }
        }



        public JsonResult GetEmpBasedOnLocation(string Location)
        {
            var emmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Location == Location).ToList();
            return Json(emmployees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddMaintenanceSchedule()
        {
            var response = new JsonResponse();
            MaintananceModel model = new MaintananceModel();
            model.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department == "IT").ToList();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            try
            {
                string location = Request.Form["Location"];
                string agentID = Request.Form["AgentID"];
                string agentName = Request.Form["AgentName"];
                string date = Request.Form["Date"];
                string timeIn = Request.Form["TimeIn"];
                string timeOut = Request.Form["TimeOut"];

                var employeeIDs = Request.Form.GetValues("EmployeeIDs");
                var employeeNames = Request.Form.GetValues("EmployeeNames");
                var employeeEmails = Request.Form.GetValues("EmployeeEmails");


                if (employeeIDs != null && employeeNames != null)
                {
                    for (int i = 0; i < employeeIDs.Length; i++)
                    {
                        var maintenance = new IT_Maintenance
                        {
                            EmployeeID = employeeIDs[i],
                            EmployeeName = employeeNames[i],
                            EmailId = employeeEmails[i],
                            Location = location,
                            AgentID = agentID,
                            AgentName = agentName,
                            MaintenanceDate = !string.IsNullOrWhiteSpace(date) ? DateTime.Parse(date) : (DateTime?)null,
                            TimeIn = !string.IsNullOrWhiteSpace(timeIn) ? TimeSpan.Parse(timeIn) : (TimeSpan?)null,
                            TimeOut = !string.IsNullOrWhiteSpace(timeOut) ? TimeSpan.Parse(timeOut) : (TimeSpan?)null,
                            Status = "Pending",
                        };

                        _dbContext.IT_Maintenance.Add(maintenance);
                        _dbContext.SaveChanges();

                        var emailBody = RenderPartialToString(this, "_MaintenanceEmailNotification", maintenance, ViewData, TempData);

                        var emailRequest = new EmailRequest
                        {
                            Body = emailBody,
                            ToEmail = maintenance.EmailId,
                            Subject = "Maintenance Scheduled Notification"
                        };

                        EMailHelper.SendEmail(emailRequest);
                    }



                    response.StatusCode = 200;
                    response.Message = "Maintenance scheduled successfully for all selected employees!";
                }

                else
                {
                    response.StatusCode = 400;
                    response.Message = "No employees were selected.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"Failed to schedule maintenance: {ex.Message}";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // Method to fetch the email address of an employee by their ID
        private string GetEmployeeEmailById(string employeeId)
        {
            // Assuming you have an `emp_info` table with EmployeeID and Email fields
            var employee = _dbContext.emp_info.FirstOrDefault(x => x.EmployeeID == employeeId);
            return employee != null ? employee.OfficalEmailid : string.Empty;
        }

        public ActionResult EmpMaintananceHistory(string empid)
        {
            MaintananceModel model = new MaintananceModel();

            int currentYear = DateTime.Now.Year;
            DateTime startDate = new DateTime(currentYear, 1, 1);
            DateTime endDate = new DateTime(currentYear, 12, 31);

            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            model.monthlyschedules = _dbContext.IT_Maintenance
           .Where(r => r.MaintenanceDate >= startDate && r.MaintenanceDate <= endDate && r.EmployeeID == empid)
           .OrderByDescending(r => r.MaintenanceDate)
           .ToList();
            model.SelectedEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empid).FirstOrDefault();


            // Create a list to hold the years
            List<int> years = new List<int>();

            // Add the past 10 years
            for (int i = -10; i <= 2; i++)
            {
                years.Add(currentYear + i);
            }

            model.Years = years;


            return View("~/Views/Itsupport/EmpMaintananceHistory.cshtml", model);
        }
        public ActionResult MaintananceApproval(int sno)
        {
            MaintananceModel model = new MaintananceModel();
            var selectedMaintenanceSno = sno;
            var maintenanceItem = _dbContext.IT_Maintenance.Where(x => x.Sno == sno).FirstOrDefault();
            model.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department == "IT").ToList();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.EditableRecord = maintenanceItem;

            return View("~/Views/Itsupport/MaintananceApprovalView.cshtml", model);
        }

        public ActionResult MaintananceReschedule(int sno)
        {
            MaintananceModel model = new MaintananceModel();
            var selectedMaintenanceSno = sno;
            var maintenanceItem = _dbContext.IT_Maintenance.Where(x => x.Sno == sno).FirstOrDefault();
            model.EditableRecord = maintenanceItem;
            return Json(JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMaintenanceStatus(IT_Maintenance maintenanceData)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var maintenanceItem = _dbContext.IT_Maintenance.Where(x => x.Sno == maintenanceData.Sno).FirstOrDefault();

                if (maintenanceItem != null)
                {
                    maintenanceItem.Status = maintenanceData.Status;
                    maintenanceItem.Notes = maintenanceData.Notes;
                    maintenanceItem.RescheduleDate = maintenanceData.RescheduleDate;
                    _dbContext.SaveChanges();
                }

                jsonResponse.StatusCode = 200;
                jsonResponse.Message = "Maintenance status updates successfully!";
            }
            catch (Exception)
            {
                jsonResponse.StatusCode = 500;
                jsonResponse.Message = "Error when updated Maintenace status!";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RescheduleMaintenance(IT_Maintenance maintenanceData)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var maintenanceItem = _dbContext.IT_Maintenance.Where(x => x.Sno == maintenanceData.Sno).FirstOrDefault();

                if (maintenanceItem != null)
                {
                    maintenanceItem.Notes = maintenanceData.Notes;
                    maintenanceItem.RescheduleDate = maintenanceData.RescheduleDate;
                    maintenanceItem.AgentID = maintenanceData.AgentID;
                    maintenanceItem.AgentName = maintenanceData.AgentName;
                    _dbContext.SaveChanges();
                }

                jsonResponse.StatusCode = 200;
                jsonResponse.Message = "Maintenance Rescheduled successfully!";
            }
            catch (Exception)
            {
                jsonResponse.StatusCode = 500;
                jsonResponse.Message = "Error when Rescheduling Maintenace!";
            }
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportToExcelMaintenanceByEmpHistory(int year = 0, string empid = "")
        {
            int currentYear = year != 0 ? year : DateTime.Now.Year;
            DateTime startDate = new DateTime(currentYear, 1, 1);
            DateTime endDate = new DateTime(currentYear, 12, 31);


            var data = _dbContext.IT_Maintenance
               .Where(r => r.MaintenanceDate >= startDate && r.MaintenanceDate <= endDate && r.EmployeeID == empid)
               .OrderByDescending(r => r.MaintenanceDate)
               .ToList();
                      
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Maintenance History Data");

                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "EmployeeName";
                worksheet.Cells[1, 3].Value = "EmailId";
                worksheet.Cells[1, 4].Value = "MaintenanceDate";
                worksheet.Cells[1, 5].Value = "RescheduleDate";
                worksheet.Cells[1, 6].Value = "AgentName";
                worksheet.Cells[1, 7].Value = "Status";
                worksheet.Cells[1, 8].Value = "Notes";
                worksheet.Cells[1, 9].Value = "Acknowledge";
                worksheet.Cells[1, 10].Value = "Location";
                worksheet.Cells[1, 11].Value = "IssueDate";
                worksheet.Cells[1, 12].Value = "ProblemCategory";
                worksheet.Cells[1, 13].Value = "IssueFacing";
                worksheet.Cells[1, 14].Value = "NewAssetRequirement";
               
                using (var headerRange = worksheet.Cells[1, 1, 1, 14])
                {
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.Font.Bold = true;
                }
                               
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeID ?? ""; 
                    worksheet.Cells[i + 2, 2].Value = data[i].EmployeeName ?? ""; 
                    worksheet.Cells[i + 2, 3].Value = data[i].EmailId ?? ""; 

                    // MaintenanceDate
                    worksheet.Cells[i + 2, 4].Value = data[i].MaintenanceDate.HasValue
                                                       ? data[i].MaintenanceDate.Value.ToString("yyyy-MM-dd")
                                                       : "";

                    // RescheduleDate
                    worksheet.Cells[i + 2, 5].Value = data[i].RescheduleDate.HasValue
                                                       ? data[i].RescheduleDate.Value.ToString("yyyy-MM-dd")
                                                       : "";

                    worksheet.Cells[i + 2, 6].Value = data[i].AgentName ?? ""; 
                    worksheet.Cells[i + 2, 7].Value = data[i].Status ?? ""; 
                    worksheet.Cells[i + 2, 8].Value = data[i].Notes ?? ""; 
                    worksheet.Cells[i + 2, 9].Value = data[i].Acknowledge ?? ""; 
                    worksheet.Cells[i + 2, 10].Value = data[i].Location ?? "";

                    // IssueDate
                    worksheet.Cells[i + 2, 11].Value = data[i].IssueDate.HasValue
                                                        ? data[i].IssueDate.Value.ToString("yyyy-MM-dd")
                                                        : "";

                    worksheet.Cells[i + 2, 12].Value = data[i].ProblemCategory ?? "";
                    worksheet.Cells[i + 2, 13].Value = data[i].IssueFacing ?? ""; 
                    worksheet.Cells[i + 2, 14].Value = data[i].NewAssetRequirement ?? ""; 
                }
                              
                worksheet.Cells.AutoFitColumns();
                               
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelFileName = "MaintenanceHistoryData.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }
        }

    }
}
