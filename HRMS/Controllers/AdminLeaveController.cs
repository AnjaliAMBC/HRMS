using HRMS.Helpers;
using HRMS.Models.Admin;
using HRMS.Models.Employee;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            GetLeavesInfoBasedonStartandEndDate(startOfMonth.ToString(), endOfMonth.ToString(), model);
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

        public ActionResult AdminCalenderLeaveManagement(int month, int year)
        {
            DateTime startDate, endDate;
            GetMonthStartAndEndDates(month, year, out startDate, out endDate);

            var model = new AdminLeaveManagementModel();
            GetLeavesInfoBasedonStartandEndDate(startDate.ToString(), "", model);
            var json = JsonConvert.SerializeObject(model.LeavesInfo);
            return Json(json.Replace(";", ""), JsonRequestBehavior.AllowGet);
        }

        public static void GetMonthStartAndEndDates(int month, int year, out DateTime startDate, out DateTime endDate)
        {
            // Get the first day of the month
            startDate = new DateTime(year, month, 1);

            // Get the last day of the month
            endDate = startDate.AddMonths(1).AddDays(-1);
        }

        public ActionResult AdminLeaveManagement(string selectedStartDate, string SelectedEndDate)
        {
            var model = new AdminLeaveManagementModel();

            if (string.IsNullOrWhiteSpace(SelectedEndDate))
            {
                SelectedEndDate = selectedStartDate;
            }
            GetLeavesInfoBasedonStartandEndDate(selectedStartDate, SelectedEndDate, model);
            return PartialView("~/Views/AdminDashboard/AdminLeaveEmpManage.cshtml", model);
        }

        private void GetLeavesInfoBasedonStartandEndDate(string selectedStartDate, string SelectedEndDate, AdminLeaveManagementModel model)
        {
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

            if (!string.IsNullOrWhiteSpace(SelectedEndDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedEndDate = DateTime.ParseExact(SelectedEndDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedEndDate = DateTime.Parse(SelectedEndDate);
            }


            var selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate).ToList();
            model.LeavesInfo = selectedDateLeaves.OrderBy(x => x.leavedate).ToList();
        }

        public ActionResult AdminLeaveBalanceUpdate()
        {
            var model = new AdminLeaveBalanceUpdateModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            model.Employees = _dbContext.emp_info.ToList();

            return PartialView("~/Views/AdminDashboard/AdminLeaveBalanceUpdate.cshtml", model);
        }
    }
}
