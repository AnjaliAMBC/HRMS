using HRMS.Helpers;
using HRMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class TimeSheetController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;

        public TimeSheetController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        // GET: Timesheet/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Timesheet/SubmitTimesheet
        public ActionResult SubmitTimesheet()
        {
            var model = new Timesheet();
            DateTime today = DateTime.Today;

            DayOfWeek startOfWeek = DayOfWeek.Monday;
            int diff = (7 + (today.DayOfWeek - startOfWeek)) % 7;

            DateTime weekStartDate = today.AddDays(-diff);
            DateTime weekEndDate = weekStartDate.AddDays(6);

            model.Weeknumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, startOfWeek);
            model.WeekStartDate = weekStartDate;
            model.WeekEndDate = weekEndDate;
            model.WeekInfo = DaysInfo(model.WeekStartDate.ToString("dd MMMM yyyy"), model.WeekEndDate.ToString("dd MMMM yyyy"), model.Weeknumber);



            return View("~/Views/EmployeeDashboard/EmpTimesheetSubmit.cshtml", model);
        }

        private List<DaySpecifcData> DaysInfo(string weekstart, string weekend, int weeknumber)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            // Parse weekstart and weekend parameters to DateTime
            DateTime weekStartDate = DateTime.Parse(weekstart);
            DateTime weekEndDate = DateTime.Parse(weekend);

            var loginInfo = _dbContext.tbld_ambclogininformation;
            var compoOffInfo = _dbContext.Compoffs;
            var holidayInfo = _dbContext.tblambcholidays;
            var leavesInfo = _dbContext.con_leaveupdate;
            var timeSheetInfo = _dbContext.TimeSheets;

            var categories = _dbContext.TimeSheetCategories.ToList();
            var clients = _dbContext.Clients.ToList();

            var fullDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FullDayMaxWorkingHours"]);
            var halfDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HalfDayMaxWorkingHours"]);

            var model = new List<DaySpecifcData>();
            for (DateTime date = weekStartDate; date <= weekEndDate; date = date.AddDays(1))
            {
                var daySpecificTimesheets = timeSheetInfo.Where(x => x.Date == date && x.EmployeeID == cuserContext.EmpInfo.EmployeeID).OrderByDescending(x => x.Date).ToList();
                var TimeSheetSubmitted = daySpecificTimesheets.Where(x => x.submissionstatus == "Submitted").FirstOrDefault() != null ? true : false;

                if (daySpecificTimesheets != null && daySpecificTimesheets.Count() < 5 && !TimeSheetSubmitted)
                {
                    int rowsToAdd = 5 - daySpecificTimesheets.Count();
                    for (int i = 0; i < rowsToAdd; i++)
                    {
                        daySpecificTimesheets.Add(new TimeSheet
                        {
                            Category = string.Empty,
                            IncidentTaskName = string.Empty,
                            IncidentTaskDescription = string.Empty,
                            Requester = string.Empty,
                            HoursSpent = null,
                            Priority = string.Empty,
                            Status = string.Empty,
                            Date = date,
                            EmployeeID = cuserContext.EmpInfo.EmployeeID
                        });
                    }
                }

                model.Add(new DaySpecifcData()
                {
                    CheckInInfo = loginInfo.Where(x => x.Employee_Code == cuserContext.EmpInfo.EmployeeID && x.Login_date == date).FirstOrDefault(),
                    Date = date,
                    IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? true : false,
                    CompoffInfo = compoOffInfo.Where(x => x.addStatus == "Approved" && x.EmployeeID == cuserContext.EmpInfo.EmployeeID && x.CampOffDate == date).FirstOrDefault(),
                    HolidayInfo = holidayInfo.Where(x => x.region.Contains(cuserContext.EmpInfo.Location) && x.holiday_date == date).FirstOrDefault(),
                    Leaves = leavesInfo.Where(x => x.employee_id == cuserContext.EmpInfo.EmployeeID && x.leavedate == date).ToList(),
                    TimeSheets = daySpecificTimesheets,
                    TimeSheetSubmitted = TimeSheetSubmitted,
                    Categories = categories,
                    Clients = clients,
                    FullDayeWorkingHours = fullDayWorkingHours,
                    HalfDayeWorkingHours = halfDayWorkingHours
                }); ;
            }


            return model;
        }

        public ActionResult PreviousWeekTimeSheets(string weekstart, string weekend, int weeknumber)
        {
            var model = new Timesheet();

            var cuserContext = SiteContext.GetCurrentUserContext();
            DateTime weekStartDate = DateTime.Parse(weekstart);
            DateTime weekEndDate = DateTime.Parse(weekend);
            model.Weeknumber = weeknumber;
            model.WeekStartDate = weekStartDate;
            model.WeekEndDate = weekEndDate;
            model.WeekInfo = DaysInfo(weekstart, weekend, weeknumber);

            return PartialView("~/Views/EmployeeDashboard/_EmployeeTimeSheetMain.cshtml", model);
        }


        public ActionResult AddNewRowByDate(string date, int rownmber, int dayindexnumber, string blocknumber)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            var model = new TimesheetDayModel();

            model.StartIndexNumber = rownmber;
            model.EndIndexNumber = rownmber;
            model.IndexNumber = dayindexnumber;
            model.BlockNumber = blocknumber;
            model.Date = DateTime.Parse(date);
            model.AddNewRow = true;

            var categories = _dbContext.TimeSheetCategories.ToList();
            var clients = _dbContext.Clients.ToList();

            model.DaySpecifcInfo.Categories = categories;
            model.DaySpecifcInfo.Clients = clients;

            model.DaySpecifcInfo.TimeSheets.Add(new TimeSheet
            {
                Category = string.Empty,
                IncidentTaskName = string.Empty,
                IncidentTaskDescription = string.Empty,
                Requester = string.Empty,
                HoursSpent = null,
                Priority = string.Empty,
                Status = string.Empty,
                Date = System.DateTime.Parse(date),
                EmployeeID = cuserContext.EmpInfo.EmployeeID
            });

            return PartialView("~/Views/EmployeeDashboard/_Timesheetaddnewrow.cshtml", model);
        }

        [HttpPost]
        public JsonResult SaveTimesheet(List<TimeSheet> data)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            try
            {
                if (data == null)
                {
                    return Json(new { success = false, message = "No data received." });
                }

                using (var _dbContext = new HRMS_EntityFramework())
                {
                    // Filter valid data (non-empty rows)
                    var filteredData = data.Where(ts =>
                        !string.IsNullOrWhiteSpace(ts.Category) ||
                        !string.IsNullOrWhiteSpace(ts.IncidentTaskName) ||
                        !string.IsNullOrWhiteSpace(ts.IncidentTaskDescription) ||
                        !string.IsNullOrWhiteSpace(ts.Requester) ||
                        ts.HoursSpent.HasValue ||
                        !string.IsNullOrWhiteSpace(ts.Priority) ||
                        !string.IsNullOrWhiteSpace(ts.Status)
                    ).ToList();

                    if (!filteredData.Any())
                    {
                        return Json(new { success = false, message = "No valid timesheet data to save." });
                    }

                    foreach (var record in filteredData)
                    {
                        if (record.TimeSheetID > 0) // Assuming TimeSheetId is the primary key
                        {
                            // Fetch the existing record from the database
                            var existingRecord = _dbContext.TimeSheets.FirstOrDefault(ts => ts.TimeSheetID == record.TimeSheetID);
                            if (existingRecord != null)
                            {
                                // Update the existing record
                                existingRecord.Category = record.Category;
                                existingRecord.IncidentTaskName = record.IncidentTaskName;
                                existingRecord.IncidentTaskDescription = record.IncidentTaskDescription;
                                existingRecord.Requester = record.Requester;
                                existingRecord.HoursSpent = record.HoursSpent;
                                existingRecord.Priority = record.Priority;
                                existingRecord.Status = record.Status;
                                existingRecord.UpdatedDate = DateTime.Now;
                                existingRecord.UpdatedBy = cuserContext.EmpInfo.EmployeeName;
                            }
                        }
                        else
                        {
                            // Add a new record
                            record.CreatedDate = DateTime.Now;
                            record.CreateddBy = cuserContext.EmpInfo.EmployeeName;
                            record.UpdatedDate = DateTime.Now;
                            _dbContext.TimeSheets.Add(record);
                        }
                    }

                    // Save changes to the database
                    _dbContext.SaveChanges();
                }

                return Json(new { success = true, message = "Timesheet data saved successfully!" });
            }
            catch (DbEntityValidationException ex)
            {
                // Extract detailed validation errors
                var validationErrors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"Property: {e.PropertyName}, Error: {e.ErrorMessage}")
                    .ToList();

                // Log and return the error response
                var errorDetails = string.Join(Environment.NewLine, validationErrors);
                Console.WriteLine("Validation errors occurred:\n" + errorDetails);

                return Json(new { success = false, message = "Validation errors occurred.", errors = validationErrors });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while saving timesheet data.", error = ex.Message });
            }
        }


        // GET: Timesheet/TimesheetView
        public ActionResult TimesheetView()
        {
            return View("~/Views/EmployeeDashboard/EmpTimesheetView.cshtml");
        }

        public ActionResult AdminTimesheet()
        {
            return View("~/Views/AdminDashboard/AdminTimesheetView.cshtml");
        }

        public ActionResult TimesheetWeekReport()
        {
            return View("~/Views/AdminDashboard/TimesheetWeeklyTemplate.cshtml");
        }
        public ActionResult TimesheetMonthReport()
        {
            return View("~/Views/AdminDashboard/TimesheetMonthlyTemplate.cshtml");
        }
    }
}
