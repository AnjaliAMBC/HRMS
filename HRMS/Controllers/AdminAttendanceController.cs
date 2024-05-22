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
using System.Drawing;
using OfficeOpenXml.Style;

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

        public ActionResult EmpAttendance(string selectedStartDate, string selectedEndDate, string selectedEmpID)
        {
            var model = new AdminEmpIndividualAttendanceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedEndDate = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(selectedStartDate))
            {
                model.SelectedStartDate = DateTime.Parse(selectedStartDate);
            }
            else
            {
                model.SelectedStartDate = model.SelectedEndDate.AddDays(-10);
            }

            if (!string.IsNullOrWhiteSpace(selectedEndDate))
            {
                model.SelectedEndDate = DateTime.Parse(selectedEndDate);
            }

            var selectedDateAttendence = _dbContext.EmployeeCheckins.Where(x => x.Login_date >= model.SelectedStartDate && x.Login_date <= model.SelectedEndDate && x.EmployeeID == selectedEmpID).ToList();

            if (selectedDateAttendence != null && selectedDateAttendence.Any())
            {
                model.EmpCheckInList = selectedDateAttendence;
            }

            // Get all dates between start and end date
            List<DateTime> allDates = DateHelper.GetAllDates(model.SelectedStartDate, model.SelectedEndDate);
            model.AllDates = allDates;

            model.SelectedEmployee = _dbContext.emp_info.Where(x => x.EmployeeID == selectedEmpID).FirstOrDefault();

            return PartialView("~/Views/AdminDashboard/AdminEmpIndividualAttendance.cshtml", model);
        }

        [HttpPost]
        public ActionResult ExportAttendence(string fromDate, string toDate, string empID)
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
            var selectedEmployeeCheckins = new List<EmployeeCheckin>();

            var allSelectedDates = DateHelper.GetAllDates(DateTime.Parse(fromDate).Date, DateTime.Parse(toDate).Date);

            var requiredEmployeeList = new List<emp_info>();

            if (!string.IsNullOrWhiteSpace(empID))
            {
                var selectedEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault();
                requiredEmployeeList.Add(selectedEmp);
                selectedEmployeeCheckins = _dbContext.EmployeeCheckins.Where(e => e.Login_date >= startDate && e.Login_date <= endDate && e.EmployeeID == empID).ToList();
            }
            else
            {
                var selectedEmps = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
                requiredEmployeeList.AddRange(selectedEmps);
                selectedEmployeeCheckins = _dbContext.EmployeeCheckins.Where(e => e.Login_date >= startDate && e.Login_date <= endDate).ToList();
            }

            // Define your color constants
            var headerBackgroundColor = Color.LightBlue;
            var headerFontColor = Color.DarkBlue;
            var headerFontSize = 12; //

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
                        var cell = worksheet.Cells[1, columnIndex];

                        // Set background color
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(headerBackgroundColor);

                        // Set font color
                        cell.Style.Font.Color.SetColor(headerFontColor);

                        // Set font size
                        cell.Style.Font.Size = headerFontSize;


                        // Set cell value
                        cell.Value = properties[i].Name;

                        columnIndex++;
                    }
                }

                int row = 2;

                foreach (DateTime allSelectedDate in allSelectedDates)
                {
                    foreach (emp_info emp in requiredEmployeeList)
                    {
                        var IsEmpCheckedonSelectedDate = selectedEmployeeCheckins.Where(x => x.EmployeeID == emp.EmployeeID && x.Login_date == allSelectedDate).FirstOrDefault();

                        if (IsEmpCheckedonSelectedDate != null)
                        {
                            DateTime? signInTime = null;
                            DateTime? signOutTime = null;
                            string hours = "0";
                            string minutes = "0";

                            columnIndex = 1;
                            for (int i = 0; i < properties.Length; i++)
                            {
                                if (properties[i].Name != "imagepath")
                                {
                                    var value = properties[i].GetValue(IsEmpCheckedonSelectedDate);

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
                                            worksheet.Cells[row, columnIndex].Value = dateTimeValue.ToString("dd-MM-yyyy");
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
                                                hours = timeDifference.Value.Hours.ToString();
                                                minutes = timeDifference.Value.Minutes.ToString();
                                                worksheet.Cells[row, columnIndex].Value = hours + "h:" + minutes + "m";
                                            }
                                            else
                                            {
                                                worksheet.Cells[row, columnIndex].Value = "";
                                            }

                                        }
                                        else if (properties[i].Name == "EmployeeStatus")
                                        {
                                            DateTime? signInTime1 = null;
                                            DateTime? signOutTime1 = null;

                                            var signInValue = properties[4].GetValue(IsEmpCheckedonSelectedDate);
                                            if (signInValue is DateTime dateTimeValue1)
                                            {
                                                if (dateTimeValue1.TimeOfDay != TimeSpan.Zero)
                                                {
                                                    signInTime1 = dateTimeValue1;

                                                }
                                            }

                                            var signOutValue = properties[5].GetValue(IsEmpCheckedonSelectedDate);
                                            if (signOutValue is DateTime dateTimeValue2)
                                            {
                                                if (dateTimeValue2.TimeOfDay != TimeSpan.Zero)
                                                {
                                                    signOutTime1 = dateTimeValue2;
                                                }
                                            }

                                            TimeSpan? timeDifference1 = null;

                                            if (signInTime1.HasValue && signOutTime1.HasValue)
                                            {
                                                timeDifference1 = signOutTime1.Value - signInTime1.Value;
                                            }
                                            if (timeDifference1 != null)
                                            {
                                                hours = timeDifference1.Value.Hours.ToString();
                                                minutes = timeDifference1.Value.Minutes.ToString();

                                                if (System.Convert.ToInt16(hours) >= 9)
                                                {
                                                    worksheet.Cells[row, columnIndex].Value = "Present";
                                                    var cell = worksheet.Cells[row, columnIndex];

                                                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    cell.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
                                                }
                                                else
                                                {
                                                    worksheet.Cells[row, columnIndex].Value = "Permission";
                                                    var cell = worksheet.Cells[row, columnIndex];
                                                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    cell.Style.Fill.BackgroundColor.SetColor(Color.MediumOrchid);
                                                }
                                            }
                                            else
                                            {
                                                worksheet.Cells[row, columnIndex].Value = "Present";
                                                var cell = worksheet.Cells[row, columnIndex];

                                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                cell.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
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
                        else
                        {
                            worksheet.Cells[row, 1].Value = emp.EmployeeID;
                            worksheet.Cells[row, 2].Value = emp.EmployeeName;
                            worksheet.Cells[row, 3].Value = "Leave";
                            var cell = worksheet.Cells[row, 3];
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(Color.OrangeRed);

                            worksheet.Cells[row, 4].Value = emp.OfficalEmailid;
                            worksheet.Cells[row, 5].Value = "";
                            worksheet.Cells[row, 6].Value = "";
                            worksheet.Cells[row, 7].Value = "";
                            worksheet.Cells[row, 8].Value = emp.ShiftTimings;
                            worksheet.Cells[row, 9].Value = allSelectedDate.ToString("dd-MM-yyyy");
                            worksheet.Cells[row, 10].Value = emp.Location;
                            row++;
                        }
                    }
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

        [HttpPost]
        public ActionResult UpdateEmployeeCheckIn(EmployeeCheckInUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the employee based on EmpId and Date
                var existingCheckIn = _dbContext.tbld_ambclogininformation.FirstOrDefault(e => e.Employee_Code == model.EmpId && e.Login_date == model.Date);

                if (existingCheckIn != null)
                {
                    // Update the check-in, check-out, and status fields
                    existingCheckIn.Signin_Time = model.CheckIn;
                    existingCheckIn.Signout_Time = model.CheckOut;
                   
                    _dbContext.SaveChanges();

                    return Json(new { success = true, message = "Employee data updated successfully!" });
                }

                return Json(new { success = false, message = "Employee check-in data not found." });
            }

            return Json(new { success = false, message = "Invalid data." });
        }
    }
}
