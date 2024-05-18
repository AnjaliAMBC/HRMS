using HRMS.Helpers;
using HRMS.Models.Admin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult AddShift()
        {
            return PartialView("~/Views/AdminDashboard/AddShift.cshtml");
        }
    }
}