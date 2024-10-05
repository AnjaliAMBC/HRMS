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


        public ActionResult MaintananceInfo()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            MaintananceModel model = new MaintananceModel();

            model.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department == "IT").ToList();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            var startOfMonth = new DateTime(currentYear, currentMonth, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            model.monthlyschedules = _dbContext.IT_Maintenance
                .Where(r => r.MaintenanceDate >= startOfMonth && r.MaintenanceDate <= endOfMonth).OrderByDescending(r => r.MaintenanceDate)
                .ToList();

            return View("~/Views/Itsupport/Maintanance.cshtml", model);
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
                            MaintenanceDate = !string.IsNullOrWhiteSpace(date) ? DateTime.Parse(date) : DateTime.MinValue,
                            TimeIn = TimeSpan.Parse(timeIn),
                            TimeOut = TimeSpan.Parse(timeOut),
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

        public ActionResult EmpMaintananceHistory()
        {
            return View("~/Views/Itsupport/EmpMaintananceHistory.cshtml");
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
    }
}
