using HRMS.Helpers;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminLeaveController : Controller
    {
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

        public ActionResult AdminLeaveManagement()
        {
            return PartialView("~/Views/AdminDashboard/AdminLeaveEmpManage.cshtml");
        }
    }
}
