using HRMS.Helpers;
using HRMS.Models.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


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
            return View("~/Views/AdminDashboard/AdminHolidayListView.cshtml");
        }
        public ActionResult HolidayImport()
        {
            return View("~/Views/AdminDashboard/HolidayBulkImport.cshtml");
        }
        public ActionResult JobImport()
        {
            return View("/Views/AdminDashboard/JobBulkImport.cshtml");
        }

    }
}