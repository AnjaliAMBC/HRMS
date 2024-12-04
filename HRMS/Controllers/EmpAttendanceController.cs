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
        private readonly HRMS_EntityFramework _dbContext;

        public EmpAttendanceController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public ActionResult Index(string startdate, string endDate)
        {
            if (startdate == "Invalid Date" || endDate == "Invalid Date")
            {
                return null;
            }
            EmpAttedenceModel model = GetAttedenceDetails(startdate, endDate);

            return View("~/Views/EmployeeDashboard/EmpAttendance.cshtml", model);
        }

        private EmpAttedenceModel GetAttedenceDetails(string startdate, string endDate)
        {
            var model = new EmpAttedenceModel();
            var cuserContext = SiteContext.GetCurrentUserContext();

            DateTime currentDate = DateTime.Today;
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;

            model.startWeek = currentDate.AddDays(-(int)currentDayOfWeek + (int)DayOfWeek.Monday);

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


            var location = cuserContext.EmpInfo.Location?.Trim().ToLower(); 
            Console.WriteLine($"Employee Location: {location}");  

            
            var holidays = _dbContext.tblambcholidays
                .Where(x => x.holiday_date >= model.startWeek && x.holiday_date <= model.EndWeek)
                .ToList();
            
            var filteredHolidays = holidays
                .Where(x => x.region != null && x.region.Split(',')
                    .Any(r => r.Trim().ToLower() == location))  
                .ToList();
           
            Console.WriteLine($"Filtered Holidays found: {filteredHolidays.Count}");  

            if (filteredHolidays != null && filteredHolidays.Any())
            {
                model.holidaysList = filteredHolidays;
            }

            List<DateTime> allDates = DateHelper.GetAllDates(model.startWeek, model.EndWeek);
            model.AllDates = allDates;
            return model;
        }

        public ActionResult GetAttedenceInfo(string startdate, string endDate)
        {
            if (startdate == "Invalid Date" || endDate == "Invalid Date")
            {
                return null;
            }

            EmpAttedenceModel model = GetAttedenceDetails(startdate, endDate);
            return PartialView("~/Views/EmployeeDashboard/_EmpAttendanceTableRows.cshtml", model);
        }
    }
}