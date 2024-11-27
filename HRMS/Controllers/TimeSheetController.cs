using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;
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

        // GET: Timesheet/EnterTimeSheet
        public ActionResult EnterTimesheet(string client, string date = "")
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var empID = cuserContext.EmpInfo.EmployeeID;
            Timesheet model = CurrentWeekTimeSheetDetails(client, empID, "", "", true);
            return View("~/Views/Timesheet/EmpTimesheetSubmit.cshtml", model);
        }

        private Timesheet CurrentWeekTimeSheetDetails(string client, string empID, string startDate, string endDate, bool isWeek)
        {
            var model = new Timesheet();
            DateTime today = string.IsNullOrWhiteSpace(startDate) ? DateTime.Today : DateTime.Parse(startDate);
            if (isWeek)
            {
                // Determine the start and end of the week
                DayOfWeek startOfWeek = DayOfWeek.Monday;
                int diff = (7 + (today.DayOfWeek - startOfWeek)) % 7;

                DateTime weekStartDate = today.AddDays(-diff);
                DateTime weekEndDate = weekStartDate.AddDays(6);

                model.WeekStartDate = weekStartDate;
                model.WeekEndDate = weekEndDate;

                model.Weeknumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, startOfWeek);
            }
            else
            {
                // Month calculation logic
                DateTime start = string.IsNullOrWhiteSpace(startDate) ? DateTime.Today : DateTime.Parse(startDate);
                DateTime monthStartDate = new DateTime(start.Year, start.Month, 1);

                DateTime end = string.IsNullOrWhiteSpace(endDate) ? DateTime.Today : DateTime.Parse(endDate);
                DateTime monthEndDate = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

                model.WeekStartDate = monthStartDate;
                model.WeekEndDate = monthEndDate;
            }

            var selectedEmployee = _dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault();
            model.SelectedDate = today;
            model.SelectedEmployee = selectedEmployee;
            model.WeekInfo = DaysInfo(model.WeekStartDate.ToString("dd MMMM yyyy"), model.WeekEndDate.ToString("dd MMMM yyyy"), model.Weeknumber, empID, client, selectedEmployee);
            model.SiteContext = SiteContext.GetCurrentUserContext();
            model.Client = client;

            return model;
        }


        private List<DaySpecifcData> DaysInfo(string weekstart, string weekend, int weeknumber, string empID, string client, emp_info selecteEmployee)
        {
            //var cuserContext = SiteContext.GetCurrentUserContext();

            // Parse weekstart and weekend parameters to DateTime
            DateTime weekStartDate = DateTime.Parse(weekstart);
            DateTime weekEndDate = DateTime.Parse(weekend);

            var loginInfo = _dbContext.tbld_ambclogininformation;
            var compoOffInfo = _dbContext.Compoffs.Where(x => x.addStatus == "Approved");
            var holidaysInfo = _dbContext.tblambcholidays;
            var leavesInfo = _dbContext.con_leaveupdate.Where(x => x.LeaveStatus != "Cancelled" && x.LeaveStatus != "Rejected");
            var timeSheetInfo = _dbContext.TimeSheets;

            var categories = _dbContext.TimeSheetCategories.ToList();
            var clients = _dbContext.Clients.ToList();

            var empInfo = selecteEmployee;

            var fullDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FullDayMaxWorkingHours"]);
            var halfDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HalfDayMaxWorkingHours"]);

            decimal standardWorkingHours = 8;

            var model = new List<DaySpecifcData>();

            for (DateTime date = weekStartDate; date <= weekEndDate; date = date.AddDays(1))
            {
                int allowedHours = fullDayWorkingHours;

                var daySpecificTimesheets = timeSheetInfo
                    .Where(x => x.Date == date && x.EmployeeID == empInfo.EmployeeID && x.Client == client)
                    .OrderByDescending(x => x.Date).ToList();

                var TimeSheetSubmitted = daySpecificTimesheets.Any(x => x.submissionstatus == "Submitted");

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
                            EmployeeID = empID
                        });
                    }
                }

                // Example data from your sources

                var leaveInfo = leavesInfo.Where(x => x.employee_id == empInfo.EmployeeID && x.leavedate == date).ToList();
                bool isLeave = leaveInfo != null && leaveInfo.Count() > 0 ? true : false;

                var holidayInfo = holidaysInfo.Where(x => x.region.Contains(empInfo.Location) && x.holiday_date == date).FirstOrDefault();
                bool isHoliday = holidayInfo != null ? true : false;

                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;


                var timeSheetForDate = timeSheetInfo.Where(x => x.Date == date && x.EmployeeID == empInfo.EmployeeID).ToList();
                decimal hoursSpent = timeSheetForDate.Sum(x => x.HoursSpent ?? 0);
                decimal overtime = hoursSpent > standardWorkingHours ? hoursSpent - standardWorkingHours : 0;

                var checkInRecord = loginInfo.FirstOrDefault(x => x.Login_date == date);

                var indexLabelLeave = "";
                decimal leaveY = 0;
                if (checkInRecord != null)
                {
                    if (isLeave)
                    {
                        if (leaveInfo[0].DayType == "halfDay")
                        {
                            allowedHours = halfDayWorkingHours;
                            indexLabelLeave = "Half Day Leave + Work";
                            leaveY = System.Convert.ToDecimal(0.2);
                        }
                        else
                        {
                            allowedHours = 0;
                            indexLabelLeave = "Leave";
                            leaveY = System.Convert.ToDecimal(0.2);
                        }
                    }
                    else
                    {
                        indexLabelLeave = "";
                        allowedHours = fullDayWorkingHours;
                    }
                }

                // Leave data points
                var dataPoints1 = new Graph
                {
                    label = date.ToString("d-M-yyyy"),
                    y = leaveY,
                    indexLabel = indexLabelLeave
                };


                var indexLabelHoliday = "";
                decimal holidayY = 0;
                if (checkInRecord != null)
                {
                    if (isHoliday)
                    {
                        allowedHours = fullDayWorkingHours;
                        indexLabelHoliday = "";
                    }
                }
                else
                {
                    if (isHoliday)
                    {
                        holidayY = System.Convert.ToDecimal(0.2);
                        indexLabelHoliday = "Holiday";
                    }
                }

                // Holiday data points
                var dataPoints2 = new Graph
                {
                    label = date.ToString("d-M-yyyy"),
                    y = holidayY,
                    indexLabel = indexLabelHoliday
                };

                // Weekend data points
                var dataPoints3 = new Graph
                {
                    label = date.ToString("d-M-yyyy"),
                    y = isWeekend ? System.Convert.ToDecimal(0.2) : 0,
                    indexLabel = isWeekend ? "Weekend" : ""
                };

                // Hours Spent data points
                var dataPoints4 = new Graph
                {
                    label = date.ToString("d-M-yyyy"),
                    y = hoursSpent - overtime,
                    indexLabel = hoursSpent - overtime > 0 ? "Hours Spent" : ""
                };

                // Overtime data points
                var dataPoints5 = new Graph
                {
                    label = date.ToString("d-M-yyyy"),
                    y = overtime,
                    indexLabel = overtime > 0 ? "Overtime" : ""
                };

                if (isWeekend)
                {
                    allowedHours = 0;
                }

                var DateValidated = allowedHours == 0 ? true : (allowedHours != 0 && hoursSpent > 0) ? true : false;

                if (allowedHours == 0 && DateValidated)
                {
                    TimeSheetSubmitted = true;
                }

                // Add to model
                model.Add(new DaySpecifcData()
                {
                    CheckInInfo = loginInfo.Where(x => x.Employee_Code == empID && x.Login_date == date).FirstOrDefault(),
                    Date = date,
                    IsWeekend = isWeekend,
                    CompoffInfo = compoOffInfo.Where(x => x.addStatus == "Approved" && x.EmployeeID == empInfo.EmployeeID && x.CampOffDate == date).FirstOrDefault(),
                    HolidayInfo = holidaysInfo.Where(x => x.region.Contains(empInfo.Location) && x.holiday_date == date).FirstOrDefault(),
                    Leaves = leaveInfo,
                    TimeSheets = daySpecificTimesheets,
                    TimeSheetSubmitted = TimeSheetSubmitted,
                    Categories = categories,
                    Clients = clients,
                    FullDayeWorkingHours = fullDayWorkingHours,
                    HalfDayeWorkingHours = halfDayWorkingHours,

                    DataPoints1 = new List<Graph> { dataPoints1 },
                    DataPoints2 = new List<Graph> { dataPoints2 },
                    DataPoints3 = new List<Graph> { dataPoints3 },
                    DataPoints4 = new List<Graph> { dataPoints4 },
                    DataPoints5 = new List<Graph> { dataPoints5 },

                    //Based on this value when submitting timesheet will check hurs enetred for specifc date or not
                    AllowedHours = allowedHours,

                    HoursSpent = hoursSpent - overtime,
                    OverTime = overtime,
                    DateValidated = allowedHours == 0 ? true : (allowedHours != 0 && hoursSpent > 0) ? true : false
                });
            }

            return model;
        }


        //private List<DaySpecifcData> DaysInfo(string weekstart, string weekend, int weeknumber, string empID, string location, string client)
        //{
        //    var cuserContext = SiteContext.GetCurrentUserContext();

        //    // Parse weekstart and weekend parameters to DateTime
        //    DateTime weekStartDate = DateTime.Parse(weekstart);
        //    DateTime weekEndDate = DateTime.Parse(weekend);

        //    var loginInfo = _dbContext.tbld_ambclogininformation;
        //    var compoOffInfo = _dbContext.Compoffs.Where(x => x.addStatus == "Approved");
        //    var holidayInfo = _dbContext.tblambcholidays;
        //    var leavesInfo = _dbContext.con_leaveupdate.Where(x => x.LeaveStatus != "Cancelled" && x.LeaveStatus != "Rejected");
        //    var timeSheetInfo = _dbContext.TimeSheets;

        //    var categories = _dbContext.TimeSheetCategories.ToList();
        //    var clients = _dbContext.Clients.ToList();

        //    var empInfo = _dbContext.emp_info.Where(x => x.EmployeeID == empID && x.Location == location).FirstOrDefault();

        //    var fullDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FullDayMaxWorkingHours"]);
        //    var halfDayWorkingHours = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HalfDayMaxWorkingHours"]);


        //    var dataPoints1 = new List<dynamic>();
        //    var dataPoints2 = new List<dynamic>();
        //    var dataPoints3 = new List<dynamic>();
        //    var dataPoints4 = new List<dynamic>();
        //    var dataPoints5 = new List<dynamic>();

        //    decimal standardWorkingHours = 8;


        //    var model = new List<DaySpecifcData>();
        //    for (DateTime date = weekStartDate; date <= weekEndDate; date = date.AddDays(1))
        //    {
        //        var daySpecificTimesheets = timeSheetInfo.Where(x => x.Date == date && x.EmployeeID == empInfo.EmployeeID && x.Client == client).OrderByDescending(x => x.Date).ToList();
        //        var TimeSheetSubmitted = daySpecificTimesheets.Where(x => x.submissionstatus == "Submitted").FirstOrDefault() != null ? true : false;

        //        if (daySpecificTimesheets != null && daySpecificTimesheets.Count() < 5 && !TimeSheetSubmitted)
        //        {
        //            int rowsToAdd = 5 - daySpecificTimesheets.Count();
        //            for (int i = 0; i < rowsToAdd; i++)
        //            {
        //                daySpecificTimesheets.Add(new TimeSheet
        //                {
        //                    Category = string.Empty,
        //                    IncidentTaskName = string.Empty,
        //                    IncidentTaskDescription = string.Empty,
        //                    Requester = string.Empty,
        //                    HoursSpent = null,
        //                    Priority = string.Empty,
        //                    Status = string.Empty,
        //                    Date = date,
        //                    EmployeeID = empID
        //                });
        //            }
        //        }


        //        // Example data from your sources
        //        bool isLeave = leavesInfo.Any(x => x.employee_id == empInfo.EmployeeID && x.leavedate == date);
        //        bool isHoliday = holidayInfo.Any(x => x.region.Contains(empInfo.Location) && x.holiday_date == date);
        //        bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        //        var timeSheetForDate = timeSheetInfo.Where(x => x.Date == date && x.EmployeeID == empInfo.EmployeeID).ToList();
        //        decimal hoursSpent = timeSheetForDate.Sum(x => x.HoursSpent ?? 0);
        //        // Calculate overtime (hoursSpent - standardWorkingHours, if hoursSpent > standardWorkingHours)
        //        decimal overtime = hoursSpent > standardWorkingHours ? hoursSpent - standardWorkingHours : 0;


        //        model.Add(new DaySpecifcData()
        //        {
        //            CheckInInfo = loginInfo.Where(x => x.Employee_Code == empID && x.Login_date == date).FirstOrDefault(),
        //            Date = date,
        //            IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? true : false,
        //            CompoffInfo = compoOffInfo.Where(x => x.addStatus == "Approved" && x.EmployeeID == empInfo.EmployeeID && x.CampOffDate == date).FirstOrDefault(),
        //            HolidayInfo = holidayInfo.Where(x => x.region.Contains(empInfo.Location) && x.holiday_date == date).FirstOrDefault(),
        //            Leaves = leavesInfo.Where(x => x.employee_id == empInfo.EmployeeID && x.leavedate == date).ToList(),
        //            TimeSheets = daySpecificTimesheets,
        //            TimeSheetSubmitted = TimeSheetSubmitted,
        //            Categories = categories,
        //            Clients = clients,
        //            FullDayeWorkingHours = fullDayWorkingHours,
        //            HalfDayeWorkingHours = halfDayWorkingHours,
        //        });



        //        // Leave data points
        //        dataPoints1.Add(new
        //        {
        //            label = date.ToString("d-M-yyyy"),
        //            y = isLeave ? 0.2m : 0,
        //            indexLabel = isLeave ? "Leave" : ""
        //        });

        //        // Holiday data points
        //        dataPoints2.Add(new
        //        {
        //            label = date.ToString("d-M-yyyy"),
        //            y = isHoliday ? 0.2m : 0,
        //            indexLabel = isHoliday ? "Holiday" : ""
        //        });

        //        // Weekend data points
        //        dataPoints3.Add(new
        //        {
        //            label = date.ToString("d-M-yyyy"),
        //            y = isWeekend ? 0.2m : 0,
        //            indexLabel = isWeekend ? "Weekend" : ""
        //        });

        //        // Hours Spent data points
        //        dataPoints4.Add(new
        //        {
        //            label = date.ToString("d-M-yyyy"),
        //            y = hoursSpent,
        //            indexLabel = hoursSpent > 0 ? "Hours Spent" : ""
        //        });

        //        // Overtime data points
        //        dataPoints5.Add(new
        //        {
        //            label = date.ToString("d-M-yyyy"),
        //            y = overtime,
        //            indexLabel = overtime > 0 ? "Overtime" : ""
        //        });

        //    }
        //    return model;
        //}

        public ActionResult PreviousWeekTimeSheets(string weekstart, string weekend, int weeknumber, string client, string empID)
        {
            Timesheet model = TimesheetsByWeek(weekstart, weekend, weeknumber, client, empID);

            return PartialView("~/Views/Timesheet/_EmployeeTimeSheetMain.cshtml", model);
        }

        private Timesheet TimesheetsByWeek(string weekstart, string weekend, int weeknumber, string client, string empID)
        {
            var model = new Timesheet();
            var selectedEmployee = _dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault();

            var cuserContext = SiteContext.GetCurrentUserContext();
            DateTime weekStartDate = DateTime.Parse(weekstart);
            DateTime weekEndDate = DateTime.Parse(weekend);
            model.Weeknumber = weeknumber;
            model.WeekStartDate = weekStartDate;
            model.WeekEndDate = weekEndDate;
            model.SelectedEmployee = selectedEmployee;
            model.WeekInfo = DaysInfo(weekstart, weekend, weeknumber, empID, client, selectedEmployee);
            model.Client = client;
            return model;
        }

        public ActionResult ViewPreviousWeekTimeSheets(string weekstart, string weekend, int weeknumber, string client, string empID)
        {
            Timesheet model = TimesheetsByWeek(weekstart, weekend, weeknumber, client, empID);

            return PartialView("~/Views/Timesheet/_EmpViewTimeSheetrows.cshtml", model);
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

            return PartialView("~/Views/Timesheet/_Timesheetaddnewrow.cshtml", model);
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
            var cuserContext = SiteContext.GetCurrentUserContext();
            Timesheet model = CurrentWeekTimeSheetDetails(cuserContext.EmpInfo.Client, cuserContext.EmpInfo.EmployeeID, "", "", true);
            return View("~/Views/Timesheet/EmpTimesheetView.cshtml", model);
        }


        public ActionResult SubmitTimeSheet(string weekstart, string weekend, int weeknumber, string client, string empID)
        {
            try
            {
                var cuserContext = SiteContext.GetCurrentUserContext();
                var timeSheets = _dbContext.TimeSheets.Where(x => x.EmployeeID == empID && x.WeekEnd == weeknumber).ToList();

                foreach (var timeSheet in timeSheets)
                {
                    timeSheet.submissionstatus = "Submitted";
                    timeSheet.UpdatedDate = DateTime.Now;
                }
                _dbContext.SaveChanges();

                var emailBody = RenderPartialToString(this, "_TimesheetReminderEmail", timeSheets, ViewData, TempData);

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["TimesheetSubmitEmail"],
                    Subject = $"Submission of Timesheet - {cuserContext.EmpInfo.EmployeeName} - {cuserContext.EmpInfo.EmployeeID}" // Adjust subject as needed
                };

                // Send email
                EMailHelper.SendEmail(emailRequest);


                return Json(new { success = true, message = "TimeSheet submitted Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = "Error when submitted TimeSheet!" });
            }
        }

        public ActionResult DeleteTimeSheet(int timesheetid)
        {
            try
            {
                var cuserContext = SiteContext.GetCurrentUserContext();
                var timeSheets = _dbContext.TimeSheets.Where(x => x.TimeSheetID == timesheetid).FirstOrDefault();

                _dbContext.TimeSheets.Remove(timeSheets);
                _dbContext.SaveChanges();

                return Json(new { success = true, message = "TimeSheet deleted Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = "Error when deleting TimeSheet!" });
            }
        }


        public ActionResult AdminTimesheet()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            Timesheet model = CurrentWeekTimeSheetDetails(cuserContext.EmpInfo.Client, cuserContext.EmpInfo.EmployeeID, "", "", true);
            model.Employees = _dbContext.emp_info.ToList();
            return View("~/Views/Timesheet/AdminTimesheetView.cshtml", model);
        }


        public JsonResult GetEmployeeTimesheet(string client, string employeeId, string startdate, string endDate, bool isweek)
        {
            Timesheet model = CurrentWeekTimeSheetDetails(client, employeeId, startdate, endDate, isweek);

            // Render the graph block partial view
            string graphBlockHtml = RenderPartialViewToString("~/Views/Timesheet/_AdminTimesheetCharts.cshtml", model);

            // Render the table block partial view
            string tableBlockHtml = RenderPartialViewToString("~/Views/Timesheet/_AdminTimesheetrows.cshtml", model);

            return Json(new
            {
                graphBlockHtml,
                tableBlockHtml
            }, JsonRequestBehavior.AllowGet);

        }

        // Helper method to render a partial view to string
        private string RenderPartialViewToString(string viewPath, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, viewPath, null);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        public ActionResult TimesheetWeekReport()
        {
            return View("~/Views/Timesheet/TimesheetWeeklyTemplate.cshtml");
        }
        public ActionResult TimesheetMonthReport()
        {
            return View("~/Views/Timesheet/TimesheetMonthlyTemplate.cshtml");
        }
    }
}
