using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class TimeSheetController : Controller
    {
        // GET: TimeSheet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubmitTimesheet()
        {
            return View("~/Views/EmployeeDashboard/EmpTimesheetSubmit.cshtml");
        }

        public ActionResult TimesheetView()
        {
            return View("~/Views/EmployeeDashboard/EmpTimesheetView.cshtml");
        }
    }
}
