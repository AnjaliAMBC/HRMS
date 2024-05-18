using HRMS.Helpers;
using HRMS.Models.Admin;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace HRMS.Controllers
{
    public class AdminAttendanceController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminAttendanceController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }


        // GET: Attendance
        public ActionResult Attendance(string selectedDate)
        {
            var model = new AdminAttendanceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedDate = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(selectedDate))
            {
                model.SelectedDate = DateTime.ParseExact(selectedDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
            }

            var selectedDateAttendence = _dbContext.EmployeeCheckins.Where(x => x.Login_date == model.SelectedDate).ToList();

            if (selectedDateAttendence != null && selectedDateAttendence.Any())
            {
                model.EmpCheckInList = selectedDateAttendence;
            }

            model.AllEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            return PartialView("~/Views/AdminDashboard/AdminAttendance.cshtml", model);
        }

        public ActionResult EmpAttendance(string selectedDate, string selectedEmpID)
        {
            var model = new AdminEmpIndividualAttendanceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedEndDate = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(selectedDate))
            {
                model.SelectedEndDate = DateTime.ParseExact(selectedDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
            }

            model.SelectedStartDate = model.SelectedEndDate.AddDays(-10);

            var selectedDateAttendence = _dbContext.EmployeeCheckins.Where(x => x.Login_date >= model.SelectedStartDate && x.Login_date <= model.SelectedEndDate && x.EmployeeID == selectedEmpID).ToList();

            if (selectedDateAttendence != null && selectedDateAttendence.Any())
            {
                model.EmpCheckInList = selectedDateAttendence;
            }

            // Get all dates between start and end date
            List<DateTime> allDates = DateHelper.GetAllDates(model.SelectedStartDate, model.SelectedEndDate);
            model.AllDates = allDates;

            var selectedEmployeeInfo = _dbContext.emp_info.Where(x => x.EmployeeID == selectedEmpID).FirstOrDefault();

            return PartialView("~/Views/AdminDashboard/AdminEmpIndividualAttendance.cshtml", model);
        }

        [HttpPost]
        public ActionResult ExportAttendence(string fromDate, string toDate)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrWhiteSpace(fromDate))
            {
                startDate = DateTime.Parse(fromDate).Date;
            }

            if (!string.IsNullOrWhiteSpace(toDate))
            {
                endDate = DateTime.Parse(toDate).Date;
            }


            var selectedEmployees = _dbContext.EmployeeCheckins.Where(e => e.Login_date >= startDate && e.Login_date <= endDate).ToList();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeInfo");

                var properties = typeof(EmployeeCheckin).GetProperties();
                int columnIndex = 1;

                // Add header row
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name != "imagepath")
                    {
                        worksheet.Cells[1, columnIndex].Value = properties[i].Name;
                        columnIndex++;
                    }
                }

                int row = 2;

                // Add data rows
                foreach (var employee in selectedEmployees)
                {
                    DateTime? signInTime = null;
                    DateTime? signOutTime = null;

                    columnIndex = 1;
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].Name != "imagepath")
                        {
                            var value = properties[i].GetValue(employee);

                            if (value is DateTime dateTimeValue)
                            {
                                if (dateTimeValue.TimeOfDay != TimeSpan.Zero)
                                {
                                    worksheet.Cells[row, columnIndex].Value = dateTimeValue.ToString("h:mm tt");

                                    if (properties[i].Name == "Signin_Time")
                                    {
                                        signInTime = dateTimeValue;
                                    }

                                    if (properties[i].Name == "Signout_Time")
                                    {
                                        signOutTime = dateTimeValue;
                                    }

                                }
                                else
                                {
                                    worksheet.Cells[row, columnIndex].Value = dateTimeValue.ToString("yyyy-MM-dd");
                                }
                            }

                            else
                            {
                                if (properties[i].Name == "Working_Hours")
                                {
                                    TimeSpan? timeDifference = null;

                                    if (signInTime.HasValue && signOutTime.HasValue)
                                    {
                                        timeDifference = signOutTime.Value - signInTime.Value;
                                    }
                                    if (timeDifference != null)
                                    {
                                        string hours = timeDifference.Value.Hours.ToString();
                                        string minutes = timeDifference.Value.Minutes.ToString();
                                        worksheet.Cells[row, columnIndex].Value = hours + "h:" + minutes + "m";
                                    }
                                    else
                                    {
                                        worksheet.Cells[row, columnIndex].Value = "";
                                    }

                                }
                                else
                                {
                                    worksheet.Cells[row, columnIndex].Value = value?.ToString() ?? string.Empty;
                                }

                            }
                            columnIndex++;
                        }
                    }
                    row++;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    excelPackage.SaveAs(memoryStream);
                    byte[] fileContents = memoryStream.ToArray();
                    return Json(Convert.ToBase64String(fileContents));
                }
            }
        }

        public ActionResult AddShift()
        {
            return PartialView("~/Views/AdminDashboard/AddShift.cshtml");
        }
    }
}
