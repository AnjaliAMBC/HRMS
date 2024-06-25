using HRMS.Helpers;
using HRMS.Models.Admin;
using HRMS.Models.Employee;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminLeaveController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;
        public AdminLeaveController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public ActionResult Index(string selectedStartDate, string selectedEndDate)
        {
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var model = new AdminLeaveManagementModel();
            new LeaveCalculator().GetLeavesInfoBasedonStartandEndDate(startOfMonth.ToString("dd MMMM yyyy"), endOfMonth.ToString("dd MMMM yyyy"), model, "");
            return View("~/Views/AdminDashboard/AdminLeaveTracker.cshtml", model);
        }
        public ActionResult AdminLeaveBalance()
        {
            LeaveTypesBasedOnEmpViewModel empLeaveTypes = new LeaveCalculator().GetLeavesByEmp("");
            return PartialView("~/Views/AdminDashboard/AdminEmpBalanceView.cshtml", empLeaveTypes);
        }
        public ActionResult AdminLeaveApply()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }

        public ActionResult AdminCalenderLeaveManagement(int month, int year, string empID)
        {
            //DateTime startDate, endDate;

            //GetMonthStartAndEndDates(month, year, out startDate, out endDate);

            //var model = new AdminLeaveManagementModel();

            var leaves = new LeaveCalculator().EmpLeaveInfoBasedonDate(DateTime.Today.ToString("dd-MM-yyyy"), DateTime.Today.ToString("dd-MM-yyyy"));
            //new LeaveCalculator().GetLeavesInfoBasedonStartandEndDate(startDate.ToString(), "", model, empID);
            var json = JsonConvert.SerializeObject(leaves);
            return Json(json.Replace(";", ""), JsonRequestBehavior.AllowGet);
        }

        public static void GetMonthStartAndEndDates(int month, int year, out DateTime startDate, out DateTime endDate)
        {
            // Get the first day of the month

            if (month == 0)
            {
                month = 1;
            }
            startDate = new DateTime(year, month, 1);

            // Get the last day of the month
            endDate = startDate.AddMonths(1).AddDays(-1);

        }

        public ActionResult AdminLeaveManagement(string selectedStartDate, string SelectedEndDate)
        {
            var model = new AdminLeaveManagementModel();

            model.SelectedDate = DateTime.Today;
            model.SelectedEndDate = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(selectedStartDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedDate = DateTime.Parse(selectedStartDate);
            }


            model.SelectedEndDate = model.SelectedDate;
            model.LeavesInfoBasedOnFromAndTodate = new LeaveCalculator().EmpLeaveInfoBasedonDate(model.SelectedDate.ToString("dd-MM-yyyy"), model.SelectedEndDate.ToString("dd-MM-yyyy"));
            return PartialView("~/Views/AdminDashboard/AdminLeaveEmpManage.cshtml", model);
        }



        public ActionResult AdminLeaveBalanceUpdate(string selectedEmpID)
        {
            AdminLeaveBalanceUpdateModel model = EmpBasedLeaveBalance(selectedEmpID);
            return PartialView("~/Views/AdminDashboard/AdminLeaveBalanceUpdate.cshtml", model);
        }

        private AdminLeaveBalanceUpdateModel EmpBasedLeaveBalance(string selectedEmpID)
        {
            var model = new AdminLeaveBalanceUpdateModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            model.Employees = _dbContext.emp_info.ToList();

            var currentYear = DateTime.Today.Year.ToString();
            model.EmpLeaveBalance = _dbContext.LeaveBalances.Where(x => x.EmpID == selectedEmpID && x.Year == currentYear).FirstOrDefault();

            var leaveTypes = new LeaveCalculator().LeaveCategories();
            foreach (var leaveType in leaveTypes)
            {
                model.AvailableLeaves.Add(new LeaveCalculator().GetEmpBasedAvailableLeaves(model.EmpInfo, model.EmpLeaveBalance, leaveType));
            }

            return model;
        }

        public ActionResult AdminLeaveBalanceUpdatebyEmpID(string selectedEmpID)
        {
            AdminLeaveBalanceUpdateModel model = EmpBasedLeaveBalance(selectedEmpID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHolidaysBasedonLocation(string empid, int month, int year)
        {
            var selectedEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empid).FirstOrDefault();
            DateTime yearstartDate, yearendDate;
            GetMonthStartAndEndDates(month + 1, year, out yearstartDate, out yearendDate);
            DateTime currentDate = DateTime.Today;
            if (selectedEmp != null)
            {
                var holidayList = _dbContext.tblambcholidays.Where(x => x.region == selectedEmp.Location && x.holiday_date >= yearstartDate && x.holiday_date <= yearendDate);
                var json = JsonConvert.SerializeObject(holidayList);
                return Json(json.Replace(";", ""), JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdminLeaveHistory(string startdate, string endDate)
        {
            DateTime dateStart = new DateTime();
            DateTime dateEnd = new DateTime();
            if (string.IsNullOrWhiteSpace(startdate))
            {
                dateStart = new DateTime(System.DateTime.Today.Year, 1, 1);
            }
            else
            {
                dateStart = System.DateTime.Parse(startdate);
            }

            if (string.IsNullOrWhiteSpace(endDate))
            {
                dateEnd = new DateTime(System.DateTime.Today.Year, 12, 31);
            }
            else
            {
                dateEnd = System.DateTime.Parse(endDate);
            }

            var AdminLeaveHistoryModel = new AdminLeaveHistoryViewModel();
            var leavesHistory = new LeaveCalculator().EmpLeaveInfoBasedonFromAndToDatesWithLeaveDate(dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"), "", "", "", "");
            AdminLeaveHistoryModel.AllEMployeeLeaves = leavesHistory;
            AdminLeaveHistoryModel.Employees = _dbContext.emp_info.ToList();

            AdminLeaveHistoryModel.Departments = new EmployeeHelper(_dbContext).GetDepartments();

            return PartialView("~/Views/AdminDashboard/AdminLeaveHistory.cshtml", AdminLeaveHistoryModel);
        }

        public ActionResult UpdateLeaveBalanceBasedonEmpID(LeaveBalance leaveBalance)
        {
            var currentYear = DateTime.Today.Year.ToString();
            var leaveBalanceInfo = _dbContext.LeaveBalances.Where(x => x.EmpID == leaveBalance.EmpID && x.Year == currentYear).FirstOrDefault();
            if (leaveBalanceInfo != null)
            {
                leaveBalanceInfo.CompOff = leaveBalance.CompOff;
                leaveBalanceInfo.Bereavement = leaveBalance.Bereavement;
                leaveBalanceInfo.Earned = leaveBalance.Earned;
                leaveBalanceInfo.Emergency = leaveBalance.Emergency;
                leaveBalanceInfo.Marriage = leaveBalance.Marriage;
                leaveBalanceInfo.Maternity = leaveBalance.Maternity;
                leaveBalanceInfo.Sick = leaveBalance.Sick;
                leaveBalanceInfo.Paternity = leaveBalance.Paternity;
                //leaveBalanceInfo.HourlyPermission = 
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Leave balance updated successfully." });
            }

            return Json(new { success = false, message = "An error occurred: " });
        }

        public ActionResult ExportLeaveBalance()
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            var exportLeaveBalanceModel = new List<LeaveBalanceExport>();

            var currentYear = DateTime.Today.Year.ToString();
            var employees = _dbContext.emp_info.ToList();
            var leaveTypes = new LeaveCalculator().LeaveCategories();

            foreach (var emp in employees)
            {
                var exportModel = new LeaveBalanceExport
                {
                    Employees = emp,
                    EmpLeaveBalance = _dbContext.LeaveBalances
                        .FirstOrDefault(x => x.EmpID == emp.EmployeeID && x.Year == currentYear)
                };

                foreach (var leaveType in leaveTypes)
                {
                    exportModel.AvailableLeaves.Add(new LeaveCalculator().GetEmpBasedAvailableLeaves(emp, exportModel.EmpLeaveBalance, leaveType));
                }
                exportLeaveBalanceModel.Add(exportModel);
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Leave Balance");

                // Add headers
                worksheet.Cells[1, 1].Value = "EmpID";
                worksheet.Cells[1, 2].Value = "EmployeeName";
                worksheet.Cells[1, 3].Value = "EmailID";

                int colIndex = 4;
                foreach (var leaveType in leaveTypes)
                {
                    worksheet.Cells[1, colIndex].Value = leaveType.Type + " Balance";
                    //worksheet.Cells[1, colIndex + 1].Value = leaveType + " Booked";
                    //worksheet.Cells[1, colIndex + 2].Value = leaveType + " Balance";
                    colIndex += 1;
                }

                int rowIndex = 2;
                foreach (var model in exportLeaveBalanceModel)
                {
                    worksheet.Cells[rowIndex, 1].Value = model.Employees.EmployeeID;
                    worksheet.Cells[rowIndex, 2].Value = model.Employees.EmployeeName;
                    worksheet.Cells[rowIndex, 3].Value = model.Employees.OfficalEmailid;

                    colIndex = 4;
                    foreach (var leave in model.AvailableLeaves)
                    {
                        worksheet.Cells[rowIndex, colIndex].Value = leave.Balance;
                        //worksheet.Cells[rowIndex, colIndex + 1].Value = leave.Booked;
                        //worksheet.Cells[rowIndex, colIndex + 2].Value = leave.Balance;
                        colIndex += 1;
                    }
                    rowIndex++;
                }

                // Auto-fit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set some document properties
                package.Workbook.Properties.Title = "Leave Balance Report";
                package.Workbook.Properties.Author = cuserContext.EmpInfo.EmployeeName;
                package.Workbook.Properties.Comments = "This report was generated using AMBC PRM application";

                // Set some extended property values
                package.Workbook.Properties.Company = "AMBC";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"LeaveBalance-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        public ActionResult AdminLeaveCompensatoryOff()
        {
            return PartialView("~/Views/AdminDashboard/AdminLeaveCompOff.cshtml");
        }


        public ActionResult ExportLeaveDataToExcel(string startDate, string endDate, string empId, string department, string location, string status)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            DateTime dateStart = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
            DateTime dateEnd = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);

            var leaveInfos = new LeaveCalculator().EmpLeaveInfoBasedonFromAndToDatesWithLeaveDate(dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"), "", department, location, status);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Leave Data");

                // Add headers
                worksheet.Cells[1, 1].Value = "Emp-ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Leave Type";
                worksheet.Cells[1, 5].Value = "From Date";
                worksheet.Cells[1, 6].Value = "To Date";
                worksheet.Cells[1, 7].Value = "Days/Hours";
                worksheet.Cells[1, 8].Value = "Location";
                worksheet.Cells[1, 9].Value = "Reason";
                worksheet.Cells[1, 10].Value = "Status";
                worksheet.Cells[1, 11].Value = "Submitted By";
                worksheet.Cells[1, 12].Value = "Submitted Date";
                worksheet.Cells[1, 13].Value = "Updated By";
                worksheet.Cells[1, 14].Value = "Updated Date";

                // Add data rows
                for (int i = 0; i < leaveInfos.Count; i++)
                {
                    var leaveInfo = leaveInfos[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = leaveInfo.LatestLeave.employee_id;
                    worksheet.Cells[row, 2].Value = leaveInfo.LatestLeave.employee_name;
                    worksheet.Cells[row, 3].Value = leaveInfo.LatestLeave.OfficalEmailid;
                    worksheet.Cells[row, 4].Value = leaveInfo.LatestLeave.leavesource;
                    worksheet.Cells[row, 5].Value = leaveInfo.Fromdate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 6].Value = leaveInfo.Todate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 7].Value = leaveInfo.TotalLeaveDays;
                    worksheet.Cells[row, 8].Value = leaveInfo.LatestLeave.Location;
                    worksheet.Cells[row, 9].Value = leaveInfo.LatestLeave.leave_reason;
                    worksheet.Cells[row, 10].Value = leaveInfo.LatestLeave.LeaveStatus;
                    worksheet.Cells[row, 11].Value = leaveInfo.LatestLeave.submittedby;
                    worksheet.Cells[row, 12].Value = leaveInfo.LatestLeave.createddate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 13].Value = leaveInfo.LatestLeave.updatedby;
                    worksheet.Cells[row, 14].Value = leaveInfo.LatestLeave.updateddate?.ToString("yyyy-MM-dd");
                }

                //// Save the file
                //FileInfo fileInfo = new FileInfo(filePath);
                //package.SaveAs(fileInfo);

                // Auto-fit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set some document properties
                package.Workbook.Properties.Title = "Leave History";
                package.Workbook.Properties.Author = cuserContext.EmpInfo.EmployeeName;
                package.Workbook.Properties.Comments = "This report was generated using AMBC PRM application";

                // Set some extended property values
                package.Workbook.Properties.Company = "AMBC";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"LeaveHistory-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
