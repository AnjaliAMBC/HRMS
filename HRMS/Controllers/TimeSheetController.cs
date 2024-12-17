using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Net;
using System.Text;

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
        public ActionResult EnterTimesheet(string client, string date, string startdate = "", string enddate = "")
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var empID = cuserContext.EmpInfo.EmployeeID;
            Timesheet model = CurrentWeekTimeSheetDetails(client, empID, startdate, enddate, true);

            DateTime today = string.IsNullOrWhiteSpace(date) && string.IsNullOrWhiteSpace(startdate)
           ? DateTime.Now
           : DateTime.Parse(string.IsNullOrWhiteSpace(date) ? startdate : date);

            model.SelectedDate = today;

            return View("~/Views/Timesheet/EmpTimesheetSubmit.cshtml", model);
        }

        private Timesheet CurrentWeekTimeSheetDetails(string client, string empID, string startDate, string endDate, bool isWeek, bool isAdminReport = false)
        {
            var model = new Timesheet();
            DateTime today = string.IsNullOrWhiteSpace(startDate) ? DateTime.Today : DateTime.Parse(startDate);
            if (isWeek)
            {
                DayOfWeek startOfWeek = DayOfWeek.Monday;
                int diff = (7 + (today.DayOfWeek - startOfWeek)) % 7;

                DateTime weekStartDate = today.AddDays(-diff);
                DateTime weekEndDate = weekStartDate.AddDays(6);

                model.WeekStartDate = weekStartDate;
                model.WeekEndDate = weekEndDate;

                model.Weeknumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, startOfWeek);
                model.WeeklyReport = true;
            }
            else
            {
                DateTime start = string.IsNullOrWhiteSpace(startDate) ? DateTime.Today : DateTime.Parse(startDate);
                DateTime monthStartDate = new DateTime(start.Year, start.Month, 1);

                DateTime end = string.IsNullOrWhiteSpace(endDate) ? DateTime.Today : DateTime.Parse(endDate);
                DateTime monthEndDate = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

                model.WeekStartDate = monthStartDate;
                model.WeekEndDate = monthEndDate;
                model.WeeklyReport = false;
            }

            var selectedEmployee = _dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault();
            model.SelectedDate = today;
            model.SelectedEmployee = selectedEmployee;
            model.SelectedClient = _dbContext.EmployeeBasedClients.Where(x => x.EmployeeID == empID && x.Client == client).FirstOrDefault();
            model.WeekInfo = DaysInfo(model.WeekStartDate.ToString("dd MMMM yyyy"), model.WeekEndDate.ToString("dd MMMM yyyy"), empID, client, selectedEmployee, isAdminReport);
            model.SiteContext = SiteContext.GetCurrentUserContext();
            model.Client = client;

            return model;
        }


        private List<DaySpecifcData> DaysInfo(string weekstart, string weekend, string empID, string client, emp_info selecteEmployee, bool isAdminReport = false)
        {
            //var cuserContext = SiteContext.GetCurrentUserContext();

            // Parse weekstart and weekend parameters to DateTime
            DateTime weekStartDate = DateTime.Parse(weekstart);
            DateTime weekEndDate = DateTime.Parse(weekend);

            var loginInfo = _dbContext.tbld_ambclogininformation;
            var compoOffInfo = _dbContext.Compoffs.Where(x => x.addStatus == "Approved");
            var holidaysInfo = _dbContext.tblambcholidays;
            var leavesInfo = _dbContext.con_leaveupdate.Where(x => x.LeaveStatus != "Cancelled" && x.LeaveStatus != "Rejected");
            var timeSheetInfo = !isAdminReport ? _dbContext.TimeSheets : _dbContext.TimeSheets.Where(x => x.submissionstatus == "Submitted");

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
                            HoursSpent = 0,
                            Priority = string.Empty,
                            Status = string.Empty,
                            Date = date,
                            EmployeeID = empID,
                            submissionstatus = ""
                        });
                    }
                }

                // Example data from your sources

                var leaveInfo = leavesInfo.Where(x => x.employee_id == empInfo.EmployeeID && x.leavedate == date).ToList();
                bool isLeave = leaveInfo != null && leaveInfo.Count() > 0 ? true : false;

                var holidayInfo = holidaysInfo.Where(x => x.region.Contains(empInfo.Location) && x.holiday_date == date).FirstOrDefault();
                bool isHoliday = holidayInfo != null ? true : false;

                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;


                var timeSheetForDate = timeSheetInfo.Where(x => x.Date == date && x.EmployeeID == empInfo.EmployeeID && x.Client == client).ToList();
                //decimal hoursSpent = timeSheetForDate.Sum(x => x.HoursSpent ?? 0);
                //decimal overtime = hoursSpent > standardWorkingHours ? hoursSpent - standardWorkingHours : 0;


                decimal toatlMinutes = 0;

                foreach (var timesheet in timeSheetForDate)
                {
                    // Check if HoursSpent has a value
                    if (timesheet.HoursSpent.HasValue && timesheet.HoursSpent.Value > 0)
                    {
                        // Read the value from HoursSpent (which is in hours)
                        decimal currentTask = timesheet.HoursSpent.Value;

                        // Split the value into hours and minutes
                        int hours1 = (int)currentTask; // Get the whole number part (hours)
                        int minutes1 = (int)((currentTask - hours1) * 100); // Get the fractional part and convert it to minutes

                        // If the value is in the form of 0.30 (meaning 30 minutes), we need to handle it properly.
                        if (minutes1 >= 60)
                        {
                            minutes1 = 0; // Reset minutes
                            hours1 += 1; // Add 1 hour if minutes exceed 60
                        }

                        // Convert everything to minutes (hours * 60 + minutes)
                        toatlMinutes += (hours1 * 60) + minutes1;
                    }
                }

                decimal totalMinutes = toatlMinutes;  // Example total minutes

                // Calculate hours and remaining minutes
                decimal hours = totalMinutes / 60;   // This gives the whole number of hours
                decimal minutes = totalMinutes % 60;  // This gives the remaining minutes

                // Take the first two digits of the minutes (round down if necessary)
                int minutesTwoDigits = (int)minutes;  // Casting to int will give the whole part of the minutes

                // Format the result
                string timeFormatted = $"{(int)hours}.{minutesTwoDigits:D2}";
                decimal hoursSpent = System.Convert.ToDecimal(timeFormatted);



                //decimal totalOverTimeMinutes = totalMinutes - 480;  // Example total minutes

                //// Calculate hours and remaining minutes
                //decimal hoursovertime = totalOverTimeMinutes / 60;   // This gives the whole number of hours
                //decimal minutesovertime = totalOverTimeMinutes % 60;  // This gives the remaining minutes

                //// Take the first two digits of the minutes (round down if necessary)
                //int minutesTwoDigitsOverTime = (int)minutesovertime;  // Casting to int will give the whole part of the minutes

                //// Format the result
                //string timeFormattedOverTime = $"{(int)hoursovertime}.{minutesTwoDigitsOverTime:D2}";
                //decimal overtime = System.Convert.ToDecimal(timeFormattedOverTime);

                decimal overtime = hoursSpent > standardWorkingHours ? hoursSpent - standardWorkingHours : 0;


                var abc = 0;



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
                            indexLabelLeave = "Halfday Leave";
                            leaveY = System.Convert.ToDecimal(0.2);
                        }
                        else if (leaveInfo[0].DayType == "fullDay")
                        {
                            allowedHours = 0;
                            indexLabelLeave = "Leave";
                            leaveY = System.Convert.ToDecimal(0.2);
                        }
                        else
                        {
                            allowedHours = fullDayWorkingHours;
                            indexLabelLeave = "";
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
                        indexLabelHoliday = "Holiday";
                        holidayY = System.Convert.ToDecimal(0.2);
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

                if (isWeekend || isHoliday)
                {
                    allowedHours = 0;
                }

                var DateValidated = allowedHours == 0 ? true : (allowedHours != 0 && hoursSpent > 0) ? true : false;

                if (allowedHours == 0 && DateValidated)
                {
                    TimeSheetSubmitted = true;
                }

                //Add to model
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
            model.SiteContext = cuserContext;
            model.WeekInfo = DaysInfo(weekstart, weekend, empID, client, selectedEmployee);
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

                    // Get the list of Timesheet IDs from the input data
                    var timesheetIdsFromClient = filteredData
                        .Where(ts => ts.TimeSheetID > 0)
                        .Select(ts => ts.TimeSheetID)
                        .ToList();

                    DateTime? timesheetDate = data.FirstOrDefault()?.Date;

                    // Fetch all existing timesheets from the database for the current employee and context
                    var existingTimesheets = _dbContext.TimeSheets
                        .Where(ts => ts.EmployeeID == cuserContext.EmpInfo.EmployeeID && ts.Date == timesheetDate)
                        .ToList();

                    // Identify records to delete (existing records not in the updated list)
                    var recordsToDelete = existingTimesheets
                        .Where(ts => !timesheetIdsFromClient.Contains(ts.TimeSheetID))
                        .ToList();

                    // Delete these records from the database
                    foreach (var record in recordsToDelete)
                    {
                        _dbContext.TimeSheets.Remove(record);
                    }

                    // Handle add/update logic for the filtered input data
                    foreach (var record in filteredData)
                    {
                        if (record.TimeSheetID > 0) // Update existing record
                        {
                            var existingRecord = existingTimesheets.FirstOrDefault(ts => ts.TimeSheetID == record.TimeSheetID);
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
                        else // Add a new record
                        {
                            var newRecord = new TimeSheet
                            {
                                EmployeeID = cuserContext.EmpInfo.EmployeeID,
                                Date = record.Date,
                                Category = record.Category,
                                IncidentTaskName = record.IncidentTaskName,
                                IncidentTaskDescription = record.IncidentTaskDescription,
                                Requester = record.Requester,
                                HoursSpent = record.HoursSpent,
                                Priority = record.Priority,
                                Status = record.Status,
                                CreatedDate = DateTime.Now,
                                CreateddBy = cuserContext.EmpInfo.EmployeeName,
                                UpdatedDate = DateTime.Now,
                                UpdatedBy = cuserContext.EmpInfo.EmployeeName,
                                Client = record.Client,
                                EmployeeEmail = cuserContext.EmpInfo.OfficalEmailid,
                                EmployeeName = cuserContext.EmpInfo.EmployeeName,
                                submissionstatus = record.submissionstatus,
                                WeekNo = record.WeekNo
                            };

                            _dbContext.TimeSheets.Add(newRecord);
                        }
                    }

                    // Save changes to the database
                    _dbContext.SaveChanges();
                }

                return Json(new
                {
                    success = true,
                    message = "Timesheet data saved successfully!"
                });
            }
            catch (DbEntityValidationException ex)
            {
                var validationErrors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"Property: {e.PropertyName}, Error: {e.ErrorMessage}")
                    .ToList();

                var errorDetails = string.Join(Environment.NewLine, validationErrors);
                Console.WriteLine("Validation errors occurred:\n" + errorDetails);

                return Json(new { success = false, message = "Validation errors occurred.", errors = validationErrors });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while saving timesheet data.", error = ex.Message });
            }
        }




        // GET: Timesheet/TimesheetView
        public ActionResult TimesheetView()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            Timesheet model = CurrentWeekTimeSheetDetails(cuserContext.EmpBasedClients[0].Client, cuserContext.EmpInfo.EmployeeID, "", "", true);
            return View("~/Views/Timesheet/EmpTimesheetView.cshtml", model);
        }


        public ActionResult SubmitTimeSheet(string weekstart, string weekend, int weeknumber, string client, string empID)
        {
            var model = new TimesheetSubmitEmailModel();
            try
            {
                model.WeekStartDate = DateTime.Parse(weekstart);
                model.WeekEndDate = DateTime.Parse(weekend);

                var cuserContext = SiteContext.GetCurrentUserContext();
                var timeSheets = _dbContext.TimeSheets.Where(x => x.EmployeeID == empID && x.WeekNo == weeknumber && x.Client == client).ToList();

                foreach (var timeSheet in timeSheets)
                {
                    timeSheet.submissionstatus = "Submitted";
                    timeSheet.UpdatedDate = DateTime.Now;
                }
                _dbContext.SaveChanges();

                model.UserContext = cuserContext;
                model.TimeSheets = timeSheets;

                var emailBody = RenderPartialToString(this, "_TimesheetSubmittedEmail", model, ViewData, TempData);

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = cuserContext.EmpInfo.OfficalEmailid,
                    Subject = $"Submission of Timesheet - {cuserContext.EmpInfo.EmployeeName} - {cuserContext.EmpInfo.EmployeeID}"
                };

                // Send email
                EMailHelper.SendEmail(emailRequest);


                return Json(new { success = true, message = "Timesheet submitted successfully!" });
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

                return Json(new { success = true, message = "Timesheet deleted Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = "Error when deleting TimeSheet!" });
            }
        }

        public ActionResult AdminTimesheet()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            Timesheet model = CurrentWeekTimeSheetDetails("", cuserContext.EmpInfo.EmployeeID, "", "", true, true);
            model.Employees = _dbContext.emp_info.ToList();
            model.WeeklyReport = true;
            return View("~/Views/Timesheet/AdminTimesheetView.cshtml", model);
        }


        public JsonResult GetEmployeeTimesheet(string client, string employeeId, string startdate, string endDate, bool isweek)
        {
            Timesheet model = CurrentWeekTimeSheetDetails(client, employeeId, startdate, endDate, isweek, true);
            model.WeeklyReport = isweek;

            string graphBlockHtml = RenderPartialViewToString("~/Views/Timesheet/_AdminTimesheetCharts.cshtml", model);
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

        public ActionResult excelexport()
        {
            var model = CurrentWeekTimeSheetDetails("AMBC", "1311", "25 November 2024", "01 December 2024", true);
            return View("~/Views/Timesheet/_TimeSheetWeeklyreportTemplate.cshtml", model);
        }

        public string RenderPartialViewToStringWithoutMainLayout(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                // Pass null for layout
                ViewData["Layout"] = null;
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult ExportSelectedEmployeesToZip(string[] selectedEmployeeIDs, string weekstart, string weekend, string client, bool weeklyreport)
        {
            if (selectedEmployeeIDs == null || selectedEmployeeIDs.Length == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No employees selected.");

            List<SourceFile> sourceFiles = new List<SourceFile>();

            var requiredZIPFileName = client + "-" + weekstart + "to" + weekend;

            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var empID in selectedEmployeeIDs)
                    {
                        var model = CurrentWeekTimeSheetDetails(client, empID, weekstart, weekend, weeklyreport, true);
                        string empSpecificReport = RenderPartialViewToStringWithoutMainLayout("~/Views/Timesheet/_TimeSheetWeeklyreportTemplate.cshtml", model);

                        string wrappedHtmlContent = "<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">" +
                                   "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head>" +
                                   "<body>" + empSpecificReport + "</body></html>";


                        byte[] byteArray = Encoding.ASCII.GetBytes(wrappedHtmlContent);

                        sourceFiles.Add(new SourceFile()
                        {
                            FileBytes = byteArray,
                            Extension = ".xls",
                            Name = model.SelectedEmployee.EmployeeName + "-TimeSheet- " + weekstart + " to " + weekend
                        });
                    }
                }

                byte[] fileBytes = null;

                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    using (System.IO.Compression.ZipArchive zip = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        foreach (SourceFile f in sourceFiles)
                        {

                            System.IO.Compression.ZipArchiveEntry zipItem = zip.CreateEntry(f.Name + "." + f.Extension);
                            using (System.IO.MemoryStream originalFileMemoryStream = new System.IO.MemoryStream(f.FileBytes))
                            {
                                using (System.IO.Stream entryStream = zipItem.Open())
                                {
                                    originalFileMemoryStream.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                    fileBytes = memoryStream.ToArray();
                }

                // download the constructed zip

                Response.AddHeader("Content-Disposition", "attachment; filename=" + requiredZIPFileName + ".zip");
                return File(fileBytes, "application/zip");
            }
        }


        private MemoryStream GenerateExcelFromRenderedView(string htmlContent, string empID)
        {
            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Timesheet");
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var table = htmlDoc.DocumentNode.SelectSingleNode("//table");
                if (table != null)
                {
                    int row = 1;
                    foreach (var tr in table.SelectNodes(".//tr"))
                    {
                        int col = 1;
                        foreach (var td in tr.SelectNodes(".//td|.//th"))
                        {
                            worksheet.Cells[row, col].Value = td.InnerText.Trim();
                            col++;
                        }
                        row++;
                    }
                }
                else
                {
                    worksheet.Cells[1, 1].Value = "Generated Timesheet Report";
                    worksheet.Cells[2, 1].Value = htmlContent;
                }


                worksheet.Cells.AutoFitColumns();
                package.SaveAs(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public ActionResult TimesheetRemainders(string client, int weekNumber)
        {
            int remainderWeek = weekNumber;
            var distinctEmployeeIDs = _dbContext.TimeSheets
                                         .Where(x => x.Client == client && x.WeekNo == remainderWeek && (x.submissionstatus == "Submitted"))
                                         .Select(x => x.EmployeeID)
                                         .Distinct()
                                         .ToList();

            var clientSpecificEmployees = _dbContext.EmployeeBasedClients
                                                    .Where(x => x.Client == client && x.EmployeeStatus == "Active")
                                                    .ToList();
            var missingEmployees = clientSpecificEmployees
                .Where(emp => !distinctEmployeeIDs.Contains(emp.EmployeeID))
                .ToList();

            return PartialView("~/Views/Timesheet/_TimesheerRemainders.cshtml", missingEmployees);
        }


        [HttpPost]
        public ActionResult SendReminderEmail(List<EmployeeReminderInfo> selectedEmployees)
        {
            if (selectedEmployees == null || !selectedEmployees.Any())
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No employees selected.");

            foreach (var employee in selectedEmployees)
            {
                string empId = employee.EmployeeID;
                string empName = employee.EmployeeName;
                string empEmail = employee.EmployeeEmail;

                var emailBody = RenderPartialViewToString("~/Views/Timesheet/_TimesheetReminderEmail.cshtml", employee);
                var emailSubject = $"Reminder: Please Submit Your Timesheet for [{employee.WeekStartDate}] - [{employee.WeekEndDate}]";
                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = employee.EmployeeEmail,
                    CCEmail = ConfigurationManager.AppSettings["TimesheetRemaindersCC"],
                    Subject = emailSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);
            }

            return Json(new { success = true, message = "Reminder emails sent successfully!" });
        }

    }
}
