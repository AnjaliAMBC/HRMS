using HRMS.Helpers;
using HRMS.Models;
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
    public class AdminLeaveController : BaseController
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
            return View("~/Views/AdminDashboard/AdminEmpBalanceView.cshtml", empLeaveTypes);
        }
        public ActionResult AdminLeaveApply()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }

        public ActionResult AdminCalenderLeaveManagement(int month, int year, string empID)
        {
            // Calculate the start and end dates of the selected month
            //DateTime startDate = new DateTime(year, month + 1, 1);
            //DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Fetch the leave information for the selected month
            var leaves = new LeaveCalculator().EmpLeaveInfoBasedonDate(DateTime.Today.ToString("dd-MM-yyyy"), DateTime.Today.ToString("dd-MM-yyyy"));

            var currentContext = Session["SiteContext"] as HRMS.Models.SiteContextModel;
            var isSuperAdmin = HRMS.Helpers.SuperAdminHelper.IsSuperAdmin(currentContext);

            if (isSuperAdmin)
            {
                var adminLocations = HRMS.Helpers.SuperAdminHelper.GetAdminLocations(currentContext);
                leaves = adminLocations.Any()
                                           ? leaves.Where(x => adminLocations.Contains(x.LatestLeave.Location)).ToList()
                                           : leaves.ToList();
            }
            // Serialize the leave information to JSON
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
                var holidayList = _dbContext.tblambcholidays.Where(x => x.region.Contains(selectedEmp.Location) && x.holiday_date >= yearstartDate && x.holiday_date <= yearendDate);
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

            return View("~/Views/AdminDashboard/AdminLeaveHistory.cshtml", AdminLeaveHistoryModel);
        }

        public ActionResult AdminLeaveHistoryViewFilter(string startDate, string endDate, string empId, string department, string location, string status)
        {
            DateTime dateStart = new DateTime();
            DateTime dateEnd = new DateTime();
            if (string.IsNullOrWhiteSpace(startDate))
            {
                dateStart = new DateTime(System.DateTime.Today.Year, 1, 1);
            }
            else
            {
                dateStart = System.DateTime.Parse(startDate);
            }

            if (string.IsNullOrWhiteSpace(endDate))
            {
                dateEnd = new DateTime(System.DateTime.Today.Year, 12, 31);
            }
            else
            {
                dateEnd = System.DateTime.Parse(endDate);
            }

            if (department == "All")
            {
                department = "";
            }


            if (location == "All")
            {
                location = "";
            }

            if (status == "All")
            {
                status = "";
            }

            var AdminLeaveHistoryModel = new AdminLeaveHistoryViewModel();
            var leavesHistory = new LeaveCalculator().EmpLeaveInfoBasedonFromAndToDatesWithLeaveDate(dateStart.ToString("yyyy-MM-dd"), dateEnd.ToString("yyyy-MM-dd"), empId, department, location, status);
            AdminLeaveHistoryModel.AllEMployeeLeaves = leavesHistory;

            var json = JsonConvert.SerializeObject(AdminLeaveHistoryModel);

            return Json(json, JsonRequestBehavior.AllowGet);
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
                leaveBalanceInfo.HourlyPermission = leaveBalance.HourlyPermission;
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
        public ActionResult AdminLeaveCompensatoryOff(string fromDate, string todate)
        {
            CompOffModel model = GetCompoffsByFromandTodate(fromDate, todate, "");
            return View("~/Views/AdminDashboard/AdminLeaveCompOff.cshtml", model);
        }

        public ActionResult EmployeeLeaveCompensatoryOff(string fromDate, string todate, string empID)
        {
            CompOffModel model = GetCompoffsByFromandTodate(fromDate, todate, empID);
            var json = JsonConvert.SerializeObject(model);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private CompOffModel GetCompoffsByFromandTodate(string fromDate, string todate, string empID)
        {
            var model = new CompOffModel();
            DateTime dateStart = new DateTime();
            DateTime dateEnd = new DateTime();
            if (string.IsNullOrWhiteSpace(fromDate))
            {
                dateStart = new DateTime(System.DateTime.Today.Year, 1, 1);
            }
            else
            {
                dateStart = System.DateTime.Parse(fromDate);
            }

            if (string.IsNullOrWhiteSpace(todate))
            {
                dateEnd = new DateTime(System.DateTime.Today.Year, 12, 31);
            }
            else
            {
                dateEnd = System.DateTime.Parse(todate);
            }

            var compoffsApplied = new List<Compoff>();

            if (string.IsNullOrWhiteSpace(empID))
            {
                compoffsApplied = _dbContext.Compoffs.Where(x => x.CampOffDate >= dateStart && x.CampOffDate <= dateEnd).OrderByDescending(x => x.createddate).ToList();
            }
            else
            {
                compoffsApplied = _dbContext.Compoffs.Where(x => x.CampOffDate >= dateStart && x.CampOffDate <= dateEnd && x.EmployeeID == empID).OrderByDescending(x => x.createddate).ToList();
            }

            model.CompOffs = compoffsApplied;
            return model;
        }

        public ActionResult ExportCompOffstoExcel(string startDate, string endDate)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var compOffs = GetCompoffsByFromandTodate(startDate, endDate, "").CompOffs;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Leave Data");

                // Add headers
                worksheet.Cells[1, 1].Value = "Emp-ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Requested Date";
                worksheet.Cells[1, 5].Value = "Work Date";
                worksheet.Cells[1, 6].Value = "Location";
                worksheet.Cells[1, 7].Value = "Reason";
                worksheet.Cells[1, 8].Value = "Status";

                // Add data rows
                for (int i = 0; i < compOffs.Count; i++)
                {
                    var copmOffInfo = compOffs[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = copmOffInfo.EmployeeID;
                    worksheet.Cells[row, 2].Value = copmOffInfo.EmployeeName;
                    worksheet.Cells[row, 3].Value = copmOffInfo.OfficalEmailid;
                    worksheet.Cells[row, 4].Value = copmOffInfo.createddate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = copmOffInfo.CampOffDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 6].Value = copmOffInfo.Location;
                    worksheet.Cells[row, 7].Value = copmOffInfo.Reason;
                    worksheet.Cells[row, 8].Value = copmOffInfo.addStatus;
                }

                //// Save the file
                //FileInfo fileInfo = new FileInfo(filePath);
                //package.SaveAs(fileInfo);

                // Auto-fit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set some document properties
                package.Workbook.Properties.Title = "Compoff Requests";
                package.Workbook.Properties.Author = cuserContext.EmpInfo.EmployeeName;
                package.Workbook.Properties.Comments = "This report was generated using AMBC PRM application";

                // Set some extended property values
                package.Workbook.Properties.Company = "AMBC";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"CompoffRequests-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
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

                // Define colors
                var headerColor = System.Drawing.Color.SkyBlue;
                var fontColor = System.Drawing.Color.White;

                // Add first three rows with custom information and color
                worksheet.Cells[1, 1].Value = "From Date";
                worksheet.Cells[1, 2].Value = dateStart.ToString("dd-MMM-yyyy");
                worksheet.Cells[1, 1, 1, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(headerColor);
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(fontColor);

                worksheet.Cells[2, 1].Value = "To Date";
                worksheet.Cells[2, 2].Value = dateEnd.ToString("dd-MMM-yyyy");
                worksheet.Cells[2, 1, 2, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(headerColor);
                worksheet.Cells[2, 1].Style.Font.Color.SetColor(fontColor);

                worksheet.Cells[3, 1].Value = "Department";
                worksheet.Cells[3, 2].Value = department;
                worksheet.Cells[3, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(headerColor);
                worksheet.Cells[3, 1].Style.Font.Color.SetColor(fontColor);

                // Add headers starting from the fifth row and color
                worksheet.Cells[5, 1].Value = "Emp-ID";
                worksheet.Cells[5, 2].Value = "Name";
                worksheet.Cells[5, 3].Value = "Email";
                worksheet.Cells[5, 4].Value = "Leave Type";
                worksheet.Cells[5, 5].Value = "From Date";
                worksheet.Cells[5, 6].Value = "To Date";
                worksheet.Cells[5, 7].Value = "Days/Hours";
                worksheet.Cells[5, 8].Value = "Location";
                worksheet.Cells[5, 9].Value = "Reason";
                worksheet.Cells[5, 10].Value = "Status";
                worksheet.Cells[5, 11].Value = "Submitted By";
                worksheet.Cells[5, 12].Value = "Submitted Date";
                worksheet.Cells[5, 13].Value = "Updated By";
                worksheet.Cells[5, 14].Value = "Updated Date";
                worksheet.Cells[5, 1, 5, 14].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[5, 1, 5, 14].Style.Fill.BackgroundColor.SetColor(headerColor);
                worksheet.Cells[5, 1, 5, 14].Style.Font.Color.SetColor(fontColor);

                // Add data rows starting from the sixth row
                for (int i = 0; i < leaveInfos.Count; i++)
                {
                    var leaveInfo = leaveInfos[i];
                    var row = i + 6; // Adjust for the rows added above

                    worksheet.Cells[row, 1].Value = leaveInfo.LatestLeave.employee_id;
                    worksheet.Cells[row, 2].Value = leaveInfo.LatestLeave.employee_name;
                    worksheet.Cells[row, 3].Value = leaveInfo.LatestLeave.OfficalEmailid;
                    worksheet.Cells[row, 4].Value = leaveInfo.LatestLeave.leavesource;
                    worksheet.Cells[row, 5].Value = leaveInfo.Fromdate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 6].Value = leaveInfo.Todate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 7].Value = leaveInfo.TotalLeaveDays.ToString().Trim();
                    worksheet.Cells[row, 8].Value = leaveInfo.LatestLeave.Location;
                    worksheet.Cells[row, 9].Value = leaveInfo.LatestLeave.leave_reason;
                    worksheet.Cells[row, 10].Value = leaveInfo.LatestLeave.LeaveStatus;
                    worksheet.Cells[row, 11].Value = leaveInfo.LatestLeave.submittedby;
                    worksheet.Cells[row, 12].Value = leaveInfo.LatestLeave.createddate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 13].Value = leaveInfo.LatestLeave.updatedby;
                    worksheet.Cells[row, 14].Value = leaveInfo.LatestLeave.updateddate?.ToString("yyyy-MM-dd");
                }

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


        public ActionResult ChangeCompoffStatus(int compoffNum, string status)
        {
            try
            {
                var compoffsApplied = _dbContext.Compoffs.Where(x => x.Compoff_no == compoffNum).FirstOrDefault();
                if (compoffsApplied != null)
                {
                    compoffsApplied.addStatus = status;
                    _dbContext.SaveChanges();
                }
                return Json(new { success = true, message = "Compoff " + status + " successfully." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occured while chaning compoff status." });
            }
        }


        public ActionResult AdminTotalLeavesImport()
        {
            return PartialView("~/Views/AdminDashboard/AdminTotalLeavesImport.cshtml");
        }

        public ActionResult AdminLeavesHistoryImport()
        {
            return PartialView("~/Views/AdminDashboard/AdminEmpLeaveHistoryImport.cshtml");
        }


        [HttpPost]
        public JsonResult UploadLeavesHistoryExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new { success = false, message = "No file uploaded or file is empty." });
            }

            try
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var workbook = package.Workbook;

                    if (workbook == null || workbook.Worksheets.Count == 0)
                    {
                        return Json(new { success = false, message = "No worksheets found in the Excel file." });
                    }

                    // Access the first worksheet
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "Failed to retrieve worksheet." });
                    }

                    List<con_leaveupdate> records = new List<con_leaveupdate>();
                    int totalRows = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= totalRows; row++)
                    {
                        if (worksheet.Cells[row, 3].Value == null)
                        {
                            continue;
                        }

                        var record = new con_leaveupdate
                        {
                            leavedate = DateTime.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty),
                            employee_id = worksheet.Cells[row, 2].Value?.ToString(),
                            leave_reason = worksheet.Cells[row, 3].Value?.ToString(),
                            DayType = worksheet.Cells[row, 4].Value?.ToString(),
                            LeaveDays = ParseDecimal(worksheet.Cells[row, 5].Value?.ToString()),
                            HalfDayCategory = worksheet.Cells[row, 6].Value?.ToString(),
                            submittedby = worksheet.Cells[row, 7].Value?.ToString(),
                            leavesource = worksheet.Cells[row, 8].Value?.ToString(),                        
                            leavecategory = worksheet.Cells[row, 9].Value?.ToString(),
                            employee_name = worksheet.Cells[row, 10].Value?.ToString(),
                            BackupResource_Name = worksheet.Cells[row, 11].Value?.ToString(),
                            EmergencyContact_no = worksheet.Cells[row, 12].Value?.ToString(),
                            LeaveStatus = worksheet.Cells[row, 13].Value?.ToString(),
                            OfficalEmailid = worksheet.Cells[row, 14].Value?.ToString(),
                            Fromdate = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 15].Value?.ToString()) ? DateTime.Parse(worksheet.Cells[row, 15].Value?.ToString()) : DateTime.MaxValue,
                            Todate = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 16].Value?.ToString()) ? DateTime.Parse(worksheet.Cells[row, 16].Value?.ToString()) : DateTime.MaxValue,
                            LeaveRequestName = worksheet.Cells[row, 17].Value?.ToString(),
                            Location = worksheet.Cells[row, 18].Value?.ToString(),
                            createdby = worksheet.Cells[row, 19].Value?.ToString(),
                            createddate = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 20].Value?.ToString()) ? DateTime.Parse(worksheet.Cells[row, 20].Value?.ToString()) : DateTime.MaxValue,
                            updatedby = worksheet.Cells[row, 21].Value?.ToString(),
                            updateddate = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 22].Value?.ToString()) ? DateTime.Parse(worksheet.Cells[row, 22].Value?.ToString()) : DateTime.MaxValue,
                            Designation = worksheet.Cells[row, 23].Value?.ToString(),
                            Department = worksheet.Cells[row, 24].Value?.ToString()
                        };

                        records.Add(record);
                    }

                    foreach (var record in records)
                    {
                        var isRecordExists = _dbContext.con_leaveupdate.FirstOrDefault(x => x.leaveuniqkey == record.leaveuniqkey);

                        if (isRecordExists != null)
                        {
                            // Update existing record
                            isRecordExists.leavedate = record.leavedate;
                            isRecordExists.employee_id = record.employee_id;
                            isRecordExists.leave_reason = record.leave_reason;
                            isRecordExists.DayType = record.DayType;
                            isRecordExists.LeaveDays = record.LeaveDays;
                            isRecordExists.HalfDayCategory = record.HalfDayCategory;
                            isRecordExists.submittedby = record.submittedby;
                            isRecordExists.leavesource = record.leavesource;
                            isRecordExists.leavecategory = record.leavecategory;
                            isRecordExists.employee_name = record.employee_name;
                            isRecordExists.BackupResource_Name = record.BackupResource_Name;
                            isRecordExists.EmergencyContact_no = record.EmergencyContact_no;
                            isRecordExists.LeaveStatus = record.LeaveStatus;
                            isRecordExists.OfficalEmailid = record.OfficalEmailid;
                            isRecordExists.Fromdate = record.Fromdate;
                            isRecordExists.Todate = record.Todate;
                            isRecordExists.LeaveRequestName = record.LeaveRequestName;
                            isRecordExists.Location = record.Location;
                            isRecordExists.createdby = record.createdby;
                            isRecordExists.createddate = record.createddate;
                            isRecordExists.updatedby = record.updatedby;
                            isRecordExists.updateddate = record.updateddate;
                            isRecordExists.Designation = record.Designation;
                            isRecordExists.Department = record.Department;
                        
                        }
                        else
                        {
                            // Add new record
                            var leaveUpdate = new con_leaveupdate
                            {
                                leavedate = record.leavedate,
                                employee_id = record.employee_id,
                                leave_reason = record.leave_reason,
                                DayType = record.DayType,
                                LeaveDays = record.LeaveDays,
                                HalfDayCategory = record.HalfDayCategory,
                                submittedby = record.submittedby,
                                leavesource = record.leavesource,
                                leavecategory = record.leavecategory,
                                employee_name = record.employee_name,
                                BackupResource_Name = record.BackupResource_Name,
                                EmergencyContact_no = record.EmergencyContact_no,
                                LeaveStatus = record.LeaveStatus,
                                OfficalEmailid = record.OfficalEmailid,
                                Fromdate = record.Fromdate,
                                Todate = record.Todate,
                                LeaveRequestName = record.LeaveRequestName,
                                Location = record.Location,
                                createdby = record.createdby,
                                createddate = record.createddate,
                                updatedby = record.updatedby,
                                updateddate = record.updateddate,
                                Designation = record.Designation,
                                Department = record.Department,
                                leaveuniqkey = record.employee_id + "_" + record.leavedate
                            };

                            _dbContext.con_leaveupdate.Add(leaveUpdate);
                        }

                        _dbContext.SaveChanges();
                    }

                    return Json(new { success = true, message = "Total Leaves info uploaded successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error reading Excel file: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult UploadTotalLeavesExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new { success = false, message = "No file uploaded or file is empty." });
            }

            try
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var workbook = package.Workbook;

                    if (workbook == null || workbook.Worksheets.Count == 0)
                    {
                        return Json(new { success = false, message = "No worksheets found in the Excel file." });
                    }

                    // Access the first worksheet
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "Failed to retrieve worksheet." });
                    }

                    List<EmployeeRecord> records = new List<EmployeeRecord>();
                    int totalRows = worksheet.Dimension.End.Row;

                    // Assuming headers are in the first row
                    for (int row = 2; row <= totalRows; row++)
                    {
                        if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 2].Value == null)
                        {
                            // If required cells (e.g., EmpID and EmployeeName) are empty, skip this row
                            continue;
                        }

                        var record = new EmployeeRecord
                        {
                            EmpID = worksheet.Cells[row, 1].Value?.ToString(),
                            EmployeeName = worksheet.Cells[row, 2].Value?.ToString(),
                            Year = worksheet.Cells[row, 3].Value?.ToString(),
                            Earned = ParseDecimal(worksheet.Cells[row, 4].Value?.ToString()),
                            Emergency = ParseDecimal(worksheet.Cells[row, 5].Value?.ToString()),
                            Sick = ParseDecimal(worksheet.Cells[row, 6].Value?.ToString()),
                            Bereavement = ParseDecimal(worksheet.Cells[row, 7].Value?.ToString()),
                            HourlyPermission = ParseDecimal(worksheet.Cells[row, 8].Value?.ToString()),
                            Marriage = ParseDecimal(worksheet.Cells[row, 9].Value?.ToString()),
                            Maternity = ParseDecimal(worksheet.Cells[row, 10].Value?.ToString()),
                            Paternity = ParseDecimal(worksheet.Cells[row, 11].Value?.ToString()),
                            CompOff = ParseDecimal(worksheet.Cells[row, 12].Value?.ToString())
                        };

                        records.Add(record);
                    }


                    foreach (var record in records)
                    {
                        var isRecordExists = _dbContext.LeaveBalances.Where(x => x.EmpID == record.EmpID && x.Year == record.Year).FirstOrDefault();

                        if (isRecordExists != null)
                        {
                            isRecordExists.Earned = record.Earned;
                            isRecordExists.Emergency = record.Emergency;
                            isRecordExists.Sick = record.Sick;
                            isRecordExists.Bereavement = record.Bereavement;
                            isRecordExists.HourlyPermission = record.HourlyPermission;
                            isRecordExists.Marriage = record.Marriage;
                            isRecordExists.Maternity = record.Marriage;
                            isRecordExists.Paternity = record.Paternity;
                            isRecordExists.CompOff = record.CompOff;
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            var leaveBalance = new LeaveBalance()
                            {
                                Bereavement = record.Bereavement,
                                CompOff = record.CompOff,
                                EmployeeName = record.EmployeeName,
                                EmpID = record.EmpID,
                                Emergency = record.Emergency,
                                Earned = record.Earned,
                                Marriage = record.Marriage,
                                Maternity = record.Maternity,
                                Paternity = record.Paternity,
                                Sick = record.Sick,
                                Year = record.Year,
                                HourlyPermission = record.HourlyPermission
                            };

                            _dbContext.LeaveBalances.Add(leaveBalance);
                            _dbContext.SaveChanges();
                        }
                    }

                    return Json(new { success = true, message = "Total Leaves info uploaded successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error reading Excel file: " + ex.Message });
            }
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0; // or handle as needed
            }

            decimal result;
            if (!decimal.TryParse(value, out result))
            {
                throw new FormatException("Invalid numeric format."); // Handle parsing failure
            }

            return result;
        }

        public ActionResult AdminEmpLeaveCalender()
        {
            var AdminLeaveEmpCalenderViewModel = new AdminLeaveEmpCalenderViewModel();
            AdminLeaveEmpCalenderViewModel.Employees = _dbContext.emp_info.ToList();

            return View("~/Views/AdminDashboard/AdminEmpLeaveCalender.cshtml", AdminLeaveEmpCalenderViewModel);
        }

        
        public JsonResult GetEmployeeEmails()
        {
            var emails = _dbContext.emp_info.Select(e => e.OfficalEmailid).ToList();
            return Json(emails, JsonRequestBehavior.AllowGet);
        }
    }
}
