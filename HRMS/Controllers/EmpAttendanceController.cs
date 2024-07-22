using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HRMS.Controllers
{
    using Helpers;
    public class EmpAttendanceController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmpAttendanceController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: EmpAttendance
        public ActionResult Index(string startdate, string endDate)
        {
            if (startdate == "Invalid Date" || endDate == "Invalid Date")
            {
                return null;
            }
            var model = new EmpAttedenceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();

            DateTime currentDate = DateTime.Today;
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;

            // Calculate the start of the week (Monday)
            model.startWeek = currentDate.AddDays(-(int)currentDayOfWeek + (int)DayOfWeek.Monday);

            // Calculate the end of the week (Sunday)
            model.EndWeek = model.startWeek.AddDays(6);

            if (!string.IsNullOrWhiteSpace(startdate) && !string.IsNullOrWhiteSpace(endDate))
            {
                model.startWeek = DateTime.ParseExact(startdate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                model.EndWeek = DateTime.ParseExact(endDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
            }

            var selectedWeekAttendence = _dbContext.tbld_ambclogininformation.Where(x => x.Login_date >= model.startWeek && x.Login_date <= model.EndWeek && x.Employee_Code == cuserContext.EmpInfo.EmployeeID).ToList();

            if (selectedWeekAttendence != null && selectedWeekAttendence.Any())
            {
                model.AttedenceList = selectedWeekAttendence;
            }

            var selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.startWeek && x.leavedate <= model.EndWeek && x.employee_id == cuserContext.EmpInfo.EmployeeID).ToList();
            if (selectedDateLeaves != null && selectedDateLeaves.Any())
            {
                model.Leaves = selectedDateLeaves;
            }

            // Get all dates between start and end date
            List<DateTime> allDates = DateHelper.GetAllDates(model.startWeek, model.EndWeek);
            model.AllDates = allDates;

            return View("~/Views/EmployeeDashboard/EmpAttendance.cshtml", model);
        }
    }
}