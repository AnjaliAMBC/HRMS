using HRMS.Helpers;
using HRMS.Models.Admin;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;

namespace HRMS.Controllers
{
    public class AdminDashController : BaseController
    {
        // GET: AdminDash
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminDashController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }
        public ActionResult Index()
        {
            var model = new AdminDashView();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            model.AnniversaryModel = new EmployeeEventHelper().Anniversary();
            model.BirthModel = new EmployeeEventHelper().Birthday();
            model.UpcomingHolidays = new EmployeeEventHelper().GetUpcomingHolidays("");

            Dictionary<int, int> compoffsaplliedbasedonholiday = new Dictionary<int, int>();
            foreach (var upcomingholiday in model.UpcomingHolidays)
            {
                var appliedCompoffs = _dbContext.Compoffs.Where(x => x.Holidayno == upcomingholiday.HolidayNo).ToList();

                if (appliedCompoffs != null && appliedCompoffs.Count() > 0)
                {
                    compoffsaplliedbasedonholiday.Add(upcomingholiday.HolidayNo, appliedCompoffs.Count());
                }
                else
                {
                    compoffsaplliedbasedonholiday.Add(upcomingholiday.HolidayNo, 0);
                }
            }

            model.CompoffApplied = compoffsaplliedbasedonholiday;

            //var AdminLeaveManagementModel = new AdminLeaveManagementModel();
            //var leavesAppliedToday = new LeaveCalculator().GetLeavesInfoBasedonStartandEndDate(DateTime.Today.ToString("dd MMMM yyyy"), DateTime.Today.ToString("dd MMMM yyyy"), AdminLeaveManagementModel, "", true);
            //model.LeavesInfo = leavesAppliedToday;

            var AdminLeaveHistoryModel = new AdminLeaveHistoryViewModel();
            var leavesAppliedToday = new LeaveCalculator().EmpLeaveInfoBasedonBasedOnTodayDate(DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.ToString("yyyy-MM-dd"), "", "", "", "");
            model.AllEMployeeLeaves = leavesAppliedToday;

            model.Employees = _dbContext.emp_info.ToList();

            model.AllHRTickets = _dbContext.IT_Ticket
                .Where(x => x.TicketType == "HR" && DbFunctions.TruncateTime(x.Created_date) == DateTime.Today)
                .ToList();

            return View("~/Views/AdminDashboard/AdminDash.cshtml", model);
        }

        public ActionResult AdminHolidayView()
        {
            var currentYear = DateTime.Today.Year;
            var holidaysList = _dbContext.tblambcholidays
                                         .Where(x => x.holiday_date.HasValue && x.holiday_date.Value.Year == currentYear)
                                         .ToList(); // Fetch the list

            // Pass the holidays list to the view
            return View("~/Views/AdminDashboard/AdminHolidayListView.cshtml", holidaysList);
        }



        public ActionResult HolidayImport()
        {
            return View("~/Views/AdminDashboard/HolidayBulkImport.cshtml");
        }


        [HttpPost]
        public ActionResult ImportHolidays(HttpPostedFileBase file)
        {
            var model = new ImportEmployeeViewModel();
            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    List<tblambcholiday> holidayList = new List<tblambcholiday>();

                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        DataTable dt = new DataTable();

                        int colCount = worksheet.Dimension.End.Column;
                        for (int col = 1; col <= colCount; col++)
                        {
                            dt.Columns.Add(worksheet.Cells[1, col].Value.ToString().Trim());
                        }

                        int rowCount = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            bool isEmptyRow = true;
                            for (int col = 1; col <= colCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value != null)
                                {
                                    isEmptyRow = false;
                                    break;
                                }
                            }

                            if (isEmptyRow)
                            {
                                continue;
                            }

                            DataRow dataRow = dt.NewRow();
                            for (int col = 1; col <= colCount; col++)
                            {
                                dataRow[col - 1] = worksheet.Cells[row, col].Value;
                            }

                            dt.Rows.Add(dataRow);
                            tblambcholiday holidayInfo = new tblambcholiday();

                            holidayInfo.holiday_date = System.Convert.ToDateTime(dataRow["Holiday Date"].ToString());
                            holidayInfo.holiday_name = dataRow["Holiday Name"].ToString();
                            holidayInfo.region = dataRow["Region"].ToString();

                            holidayList.Add(holidayInfo);
                        }
                    }

                    if (holidayList.Count > 0)
                    {
                        _dbContext.tblambcholidays.AddRange(holidayList);
                        _dbContext.SaveChanges();


                    }
                }

                return Json(new { success = true, message = "Holidays list import is successful!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Holidays list import is un-successful!" });

            }
        }

        public ActionResult UpdateHoliday(int holidayId, string holidayName, string holidayDate)
        {
            var model = new ImportEmployeeViewModel();
            try
            {
                var holiday = _dbContext.tblambcholidays.Where(x => x.holidayno == holidayId).FirstOrDefault();

                if (holiday != null)
                {
                    holiday.holiday_name = holidayName;
                    holiday.holiday_date = System.DateTime.Parse(holidayDate);
                    _dbContext.SaveChanges();
                }

                return Json(new { success = true, message = "Holiday updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Holidays update un-successful!" });

            }
        }


        public ActionResult JobImport()
        {
            return View("/Views/AdminDashboard/JobBulkImport.cshtml");
        }

    }
}