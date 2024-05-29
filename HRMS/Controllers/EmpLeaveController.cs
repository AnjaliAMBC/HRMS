using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmpLeaveController : Controller
    {
        // GET: EmpLeave
        public ActionResult Index()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpLeaveTracker.cshtml");
        }
        public ActionResult EmpApplyLeave()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }
    }
}
