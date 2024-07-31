using HRMS.Helpers;
using HRMS.Models.Admin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using Helpers;
    using HRMS.Models.Employee;
    using Microsoft.Ajax.Utilities;
    using System.Configuration;
    using System.Data.Entity;

    public class AdminAttendanceController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminAttendanceController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }


        // GET: Attendance
        public ActionResult Attendance(string selectedStartDate, string SelectedendDate)
        {
            var model = new AdminAttendanceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedDate = DateTime.Today;
            model.SelectedEndDate = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(selectedStartDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedDate = DateTime.Parse(selectedStartDate);
            }

            if (!string.IsNullOrWhiteSpace(SelectedendDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedEndDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedEndDate = DateTime.Parse(selectedStartDate);
            }
            else
            {
                model.SelectedEndDate = model.SelectedDate;
            }

            var selectedDateAttendence = _dbContext.CheckInViews.Where(x => x.Login_date >= model.SelectedDate && x.Login_date <= model.SelectedEndDate).ToList();

            if (selectedDateAttendence != null && selectedDateAttendence.Any())
            {
                model.EmpCheckInList = selectedDateAttendence;
            }

            var selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate).ToList();
            if (selectedDateLeaves != null && selectedDateLeaves.Any())
            {
                model.Leaves = selectedDateLeaves;
            }

            model.AllEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            if (!string.IsNullOrWhiteSpace(selectedStartDate) || !string.IsNullOrWhiteSpace(SelectedendDate))
            {
                return PartialView("~/Views/AdminDashboard/AdminAttendance.cshtml", model);
            }


            return View("~/Views/AdminDashboard/AdminAttendance.cshtml", model);
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

            var selectedDateAttendence = _dbContext.CheckInViews.Where(x => x.Login_date >= model.SelectedStartDate && x.Login_date <= model.SelectedEndDate && x.EmployeeID == selectedEmpID).ToList();

            if (selectedDateAttendence != null && selectedDateAttendence.Any())
            {
                model.EmpCheckInList = selectedDateAttendence;
            }

            var selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedStartDate && x.leavedate <= model.SelectedEndDate && x.employee_id == selectedEmpID).ToList();
            if (selectedDateLeaves != null && selectedDateLeaves.Any())
            {
                model.Leaves = selectedDateLeaves;
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
            var selectedEmployeeCheckins = new List<CheckInView>();

            var allSelectedDates = DateHelper.GetAllDates(DateTime.Parse(fromDate).Date, DateTime.Parse(toDate).Date);

            var requiredEmployeeList = new List<emp_info>();

            if (!string.IsNullOrWhiteSpace(empID))
            {
                var selectedEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empID).FirstOrDefault();
                requiredEmployeeList.Add(selectedEmp);
                selectedEmployeeCheckins = _dbContext.CheckInViews.Where(e => e.Login_date >= startDate && e.Login_date <= endDate && e.EmployeeID == empID).ToList();
            }
            else
            {
                var selectedEmps = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
                requiredEmployeeList.AddRange(selectedEmps);
                selectedEmployeeCheckins = _dbContext.CheckInViews.Where(e => e.Login_date >= startDate && e.Login_date <= endDate).ToList();
            }

            var leavesOnSelectedDates = _dbContext.con_leaveupdate.Where(x => x.leavedate >= startDate && x.leavedate <= endDate).ToList();

            // Define your color constants
            var headerBackgroundColor = Color.LightBlue;
            var headerFontColor = Color.DarkBlue;
            var headerFontSize = 12; //

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeInfo");

                var properties = typeof(CheckInView).GetProperties();
                int columnIndex = 1;

                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "imagepath" || properties[i].Name == "login_id")
                    {
                        continue;
                    }

                    var cell = worksheet.Cells[1, columnIndex];
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(headerBackgroundColor);
                    cell.Style.Font.Color.SetColor(headerFontColor);
                    cell.Style.Font.Size = headerFontSize;
                    cell.Value = properties[i].Name;

                    columnIndex++;
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
                                if (properties[i].Name == "imagepath" || properties[i].Name == "login_id")
                                {
                                    continue;
                                }

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

                                                    //cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    //cell.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
                                                }
                                                else
                                                {
                                                    worksheet.Cells[row, columnIndex].Value = "Permission";
                                                    var cell = worksheet.Cells[row, columnIndex];
                                                    //cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                    //cell.Style.Fill.BackgroundColor.SetColor(Color.MediumOrchid);
                                                }
                                            }
                                            else
                                            {
                                                worksheet.Cells[row, columnIndex].Value = "Present";
                                                var cell = worksheet.Cells[row, columnIndex];

                                                //cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                //cell.Style.Fill.BackgroundColor.SetColor(Color.GreenYellow);
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
                            var selectedDateIsWeekend = allSelectedDate.DayOfWeek == DayOfWeek.Saturday || allSelectedDate.DayOfWeek == DayOfWeek.Sunday;

                            var typeOfLeave = selectedDateIsWeekend == true ? "Weekend" : "Absent";

                            var isLeaveApplied = leavesOnSelectedDates.Where(x => x.leavedate == allSelectedDate && x.employee_id == emp.EmployeeID).FirstOrDefault();

                            if(isLeaveApplied != null)
                            {
                                typeOfLeave = "Leave";
                            }


                            worksheet.Cells[row, 1].Value = emp.EmployeeID;
                            //worksheet.Cells[row, 2].Value = emp.EmployeeStatus;

                            worksheet.Cells[row, 2].Value = typeOfLeave;
                            var cell = worksheet.Cells[row, 2];
                            //cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //cell.Style.Fill.BackgroundColor.SetColor(Color.OrangeRed);

                            worksheet.Cells[row, 3].Value = emp.EmployeeName;
                            worksheet.Cells[row, 4].Value = emp.ShiftTimings;
                            worksheet.Cells[row, 5].Value = emp.OfficalEmailid;
                            worksheet.Cells[row, 6].Value = allSelectedDate.ToString("dd-MM-yyyy");
                            worksheet.Cells[row, 7].Value = "--";
                            worksheet.Cells[row, 8].Value = "--";
                            worksheet.Cells[row, 9].Value = "--";
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
            var model = new AddShiftModel();

            model.Departments = new EmployeeHelper(_dbContext).GetDepartments();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            return PartialView("~/Views/AdminDashboard/AddShift.cshtml", model);
        }

        public ActionResult AddShiftInfotoDB(AjaxShiftUpdateModel data)
        {
            var model = new AjaxShiftUpdateModel();
            try
            {
                var selectedShift = data.startTime + "-" + data.endTime;
                TimeSpan shiftStartTimeSelected = TimeSpan.Parse(data.startTime);
                TimeSpan shiftEndTimeSelected = TimeSpan.Parse(data.endTime);

                var associatedEmloyees = new List<emp_info>();
                if (data.IsDepartmentBasedUpdate)
                {
                    foreach (var department in data.selectedIds)
                    {
                        var selectedDepartment = department;
                        associatedEmloyees = _dbContext.emp_info.Where(x => x.Department == selectedDepartment).ToList();
                        if (associatedEmloyees != null && associatedEmloyees.Any())
                        {
                            associatedEmloyees.ForEach(p =>
                            {
                                // Store existing shift timings
                                var existingShiftStartTime = p.ShiftStartTime;
                                var existingShiftEndTime = p.ShiftEndTime;

                                // Update to new shift timings
                                p.ShiftTimings = selectedShift;
                                p.ShiftStartTime = shiftStartTimeSelected;
                                p.ShiftEndTime = shiftEndTimeSelected;

                                // Send email notification if required
                                if (data.notification)
                                {
                                    SendShiftChangeNotificationEmail(p, existingShiftStartTime, existingShiftEndTime, shiftStartTimeSelected, shiftEndTimeSelected);
                                }

                                _dbContext.Entry(p).State = EntityState.Modified;
                            });
                        }
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    foreach (var empID in data.selectedIds)
                    {
                        associatedEmloyees = _dbContext.emp_info.Where(x => x.EmployeeID == empID).ToList();
                        if (associatedEmloyees != null && associatedEmloyees.Any())
                        {
                            associatedEmloyees.ForEach(p =>
                            {
                                // Store existing shift timings
                                var existingShiftStartTime = p.ShiftStartTime;
                                var existingShiftEndTime = p.ShiftEndTime;

                                // Update to new shift timings
                                p.ShiftTimings = selectedShift;
                                p.ShiftStartTime = shiftStartTimeSelected;
                                p.ShiftEndTime = shiftEndTimeSelected;

                                // Send email notification if required
                                if (data.notification)
                                {
                                    SendShiftChangeNotificationEmail(p, existingShiftStartTime, existingShiftEndTime, shiftStartTimeSelected, shiftEndTimeSelected);
                                }

                                _dbContext.Entry(p).State = EntityState.Modified;
                            });
                        }
                        _dbContext.SaveChanges();
                    }
                }

                if (data.notification)
                {
                    model.JsonResponse.Message = "Shift timings updated and Email notification sent successfully!";
                    model.JsonResponse.StatusCode = 200;
                }
                else
                {
                    model.JsonResponse.Message = "Shift timings updated successfully!";
                    model.JsonResponse.StatusCode = 200;
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model.JsonResponse.Message = "Error occurred!";
                model.JsonResponse.StatusCode = 500;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private void SendShiftChangeNotificationEmail(emp_info emp, TimeSpan? existingShiftStartTime, TimeSpan? existingShiftEndTime, TimeSpan newShiftStartTime, TimeSpan newShiftEndTime)
        {
            var siteURL = ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";

            string existingShiftStartTimeString = existingShiftStartTime.HasValue ? existingShiftStartTime.Value.ToString(@"hh\:mm") : "N/A";
            string existingShiftEndTimeString = existingShiftEndTime.HasValue ? existingShiftEndTime.Value.ToString(@"hh\:mm") : "N/A";
            string newShiftStartTimeString = newShiftStartTime.ToString(@"hh\:mm");
            string newShiftEndTimeString = newShiftEndTime.ToString(@"hh\:mm");

            string body = $@"
        <html>
    <head></head>
    <body>
        <div style='font-family: Arial, sans-serif; border: 1px solid gray; width: 50%; padding: 20px;'>
            <div style='margin-bottom: 20px;'>
                <img src='{logoURL}' alt='Company Logo' style='width: 100px; float: right;'>
            </div>
            <h2 style='color: #333;'>Dear {emp.EmployeeName}</h2>
            <h3 style='color: #333;'>SHIFT CHANGE</h3>
            <p>Your Shift has been changed as indicated below</p>
            <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 80%; margin-top: 20px;'>
                <tr>
                    <th style='background-color: #f2f2f2;'>Existing Shift(s)</th>
                    <th style='background-color: #f2f2f2;'>Changed Shift</th>
                </tr>
                <tr>
                    <td style='text-align: center;'>{existingShiftStartTimeString} - {existingShiftEndTimeString}</td>
                    <td style='text-align: center;'>{newShiftStartTimeString} - {newShiftEndTimeString}</td>
                </tr>
            </table>
            
        </div>
            <div style='font-family: Calibri; color: #696969; font-size: 1.1em; margin-top: 20px;'>
                This is an automated email, please do not reply.
                <br />
                Automated mail from <a href='https://prm.ambctechnologies.com' style='color: #2693F8;'>https://prm.ambctechnologies.com</a>
            </div>
    </body>
</html>";

            var emailRequest = new EmailRequest()
            {
                Body = body,
                ToEmail = emp.OfficalEmailid,
                Subject = "Shift Change Notification",
            };

            EMailHelper.SendEmail(emailRequest);
        }



        [HttpPost]
        public ActionResult UpdateEmployeeCheckIn(EmployeeCheckInUpdateModel model)
        {
            var existingCheckIn = _dbContext.tbld_ambclogininformation.FirstOrDefault(e => e.Employee_Code == model.EmpId && e.Login_date == model.Date);

            var selectedCheckinDate = model.Date.ToString("dd-MM-yyyy");
            var selectedSignInTime = model.CheckIn.ToString("HH:mm:ss");
            var selectedChekoutTime = model.CheckOut.ToString("HH:mm:ss");

            model.CheckIn = DateTime.ParseExact($"{selectedCheckinDate} {selectedSignInTime}", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            model.CheckOut = DateTime.ParseExact($"{selectedCheckinDate} {selectedChekoutTime}", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            if (existingCheckIn != null)
            {
                existingCheckIn.Signin_Time = model.CheckIn;
                existingCheckIn.Signout_Time = model.CheckOut;
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Employee data updated successfully!" });
            }
            else
            {
                var checkInModel = new tbld_ambclogininformation();
                checkInModel.Employee_Code = model.EmpId;
                checkInModel.Employee_Designation = "";
                checkInModel.Employee_LoginLocation = "";
                checkInModel.Employee_Name = model.EmpName;
                checkInModel.Login_date = model.CheckIn;
                checkInModel.Signin_Time = model.CheckIn;
                checkInModel.Signout_Time = model.CheckOut;
                checkInModel.Employee_Hostname = Dns.GetHostName();
                checkInModel.Concat_loginstring = model.EmpId + "_" + model.CheckIn.ToString("dd-MM-yyyy") + " 00:00:00";
                checkInModel.Employee_Shift = "General";
                checkInModel.Employee_IP = "";

                var newCheckInItem = _dbContext.tbld_ambclogininformation.Add(checkInModel);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Employee data updated successfully!" });
            }

            return Json(new { success = false, message = "Employee check-in data not found." });

        }
    }
}
