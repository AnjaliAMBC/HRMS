using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminAttendanceController : Controller
    {
        // GET: Attendance
        public ActionResult Attendance()
        {
            return PartialView("~/Views/AdminDashboard/AdminAttendance.cshtml");
        }

        public ActionResult AddShift()
        {
            return PartialView("~/Views/AdminDashboard/AddShift.cshtml");
        }
    }
}