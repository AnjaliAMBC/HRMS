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
            return PartialView("~/Views/AdminDashboard/AdminLeaveTracker.cshtml");
        }
        public ActionResult AdminLeaveBalance()
        {
            return PartialView("~/Views/AdminDashboard/AdminEmpBalanceView.cshtml");
        }
    }
}