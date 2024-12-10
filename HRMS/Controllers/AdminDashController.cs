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
using System.Configuration;

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

        [HttpPost]
        public ActionResult JobImport(HttpPostedFileBase file)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            try
            {
                // Check if a file is uploaded
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "No file uploaded." });
                }

                var jobList = new List<JobDetail>();

                using (var package = new ExcelPackage(file.InputStream))
                {
                    // Access the first worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "Invalid Excel file format." });
                    }

                    // Read headers
                    int colCount = worksheet.Dimension.End.Column;
                    DataTable dt = new DataTable();

                    for (int col = 1; col <= colCount; col++)
                    {
                        dt.Columns.Add(worksheet.Cells[1, col].Value?.ToString()?.Trim());
                    }

                    // Read rows
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

                        // Map data to JobDetails model
                        var jobDetails = new JobDetail
                        {
                            JobTitle = dataRow["JobTitle"]?.ToString(),
                            Experience = dataRow["Experience"]?.ToString(),
                            Location = dataRow["Location"]?.ToString(),
                            PostedBy = dataRow["PostedBy"]?.ToString(),
                            PostedDate = DateTime.TryParse(dataRow["PostedDate"]?.ToString(), out var postedDate) ? postedDate : (DateTime?)null,
                            JobType = dataRow["JobType"]?.ToString(),
                            JobStatus = dataRow["JobStatus"]?.ToString(),
                            SalaryRange = dataRow["SalaryRange"]?.ToString(),
                            Priority = dataRow["Priority"]?.ToString(),
                            JobDescription = dataRow["JobDescription"]?.ToString(),
                            UpdatedBy = cuserContext.EmpInfo.EmployeeName,
                            UpdatedDate = DateTime.Now
                        };
                        // Validate data before adding
                        if (!string.IsNullOrWhiteSpace(jobDetails.JobTitle))
                        {
                            jobList.Add(jobDetails);
                        }
                    }
                }

                // Save the job data to the database
                if (jobList.Count > 0)
                {
                    _dbContext.JobDetails.AddRange(jobList);
                    _dbContext.SaveChanges();
                }

                return Json(new { success = true, message = "Job details imported successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Job details import failed: " + ex.Message });
            }
        }


        public ActionResult JobImport()
        {
            return View("/Views/AdminDashboard/JobBulkImport.cshtml");
        }

        [HttpPost]
        public ActionResult UploadHandbook(HttpPostedFileBase file)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            if (file != null && file.ContentLength <= 0)
            {
                return Json(new { success = false, message = "No file uploaded." });
            }

            try
            {
                string handBookFolderPath = "";
                if (file != null && file.ContentLength > 0)
                {
                    var ticketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                    handBookFolderPath = Path.Combine(ticketingFolderPath, "Handbook");

                    if (!Directory.Exists(handBookFolderPath))
                    {
                        Directory.CreateDirectory(handBookFolderPath);
                    }
                }

                var handBookInfo = new HandBook()
                {
                    FilePath = DateTime.Today.ToString("dd-MM-yyyy") + "-" + file.FileName,
                    CreatedBy = cuserContext.EmpInfo.EmployeeName,                    
                    CreatedDate = DateTime.Now,
                    UpdatedBy = cuserContext.EmpInfo.EmployeeName,
                    UpdatedDate = DateTime.Now

                };

                _dbContext.HandBooks.Add(handBookInfo);
                _dbContext.SaveChanges();

                string filePath = Path.Combine(handBookFolderPath, Path.GetFileName(file.FileName));
                file.SaveAs(filePath);

                return Json(new { success = true, message = "Handbook uploaded successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Handbook upload  un-success! " });
            }
        }

        [HttpPost]
        public ActionResult CreateAnnouncement(string Location, string Subject, string Description, HttpPostedFileBase File)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            if (string.IsNullOrEmpty(Location) || string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(Description))
            {
                return Json(new { success = false, message = "All fields except file are required." });
            }

            string announcementFolderPath = "";
            if (File != null && File.ContentLength > 0)
            {
                var ticketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                announcementFolderPath = Path.Combine(ticketingFolderPath, "Announcement");
                if (!Directory.Exists(announcementFolderPath))
                {
                    Directory.CreateDirectory(announcementFolderPath);
                }
            }

            var announcement = new HRAnnouncement
            {
                Location = Location,
                Subject = Subject,
                Description = Description,
                Filepath = DateTime.Today.ToString("dd-MM-yyyy") + "-" + File.FileName,
                CreatedDate = DateTime.Now,
                CreatedBy = cuserContext.EmpInfo.EmployeeName,
                UpdatedBy = cuserContext.EmpInfo.EmployeeName,
                UpdatedDate = DateTime.Now,
            };

            _dbContext.HRAnnouncements.Add(announcement);
            _dbContext.SaveChanges();

            string filePath = Path.Combine(announcementFolderPath, Path.GetFileName(File.FileName));
            File.SaveAs(filePath);

            return Json(new { success = true, message = "Announcement created successfully!" });
        }

    }
}