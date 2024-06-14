using HRMS.Helpers;
using HRMS.Models.Admin;
using HRMS.Models.Employee;
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

        // GET: AdminLeave
        public ActionResult Index()
        {
            return View("~/Views/AdminDashboard/AdminLeaveTracker.cshtml");
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

        public ActionResult AdminLeaveManagement(string selectedStartDate, string SelectedEndDate)
        {

            var model = new AdminLeaveManagementModel();
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
                    model.SelectedEndDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedEndDate = DateTime.Parse(selectedStartDate);
            }
            else
            {
                model.SelectedEndDate = model.SelectedDate;
            }

            var selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate).ToList();

            model.LeavesInfo = selectedDateLeaves;

            return PartialView("~/Views/AdminDashboard/AdminLeaveEmpManage.cshtml", model);
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
