using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmpAttendanceController : Controller
    {
        // GET: EmpAttendance
        public ActionResult Index()
        {
            return View("~/Views/EmployeeDashboard/EmpAttendance.cshtml");
        }
    }
}