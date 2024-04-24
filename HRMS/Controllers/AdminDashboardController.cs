using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminDashboardController : Controller
    {
        // GET: AdminDashboard
        public ActionResult Dashboard()
        {
            ViewBag.ActivePage = "Dashboard";
            return View("~/Views/AdminDashboard/Dashboard.cshtml");
        }

        public ActionResult EmployeeManagement()
        {
            ViewBag.ActivePage = "EmployeeManagement";
            return View();
        }

        public ActionResult AttendanceManagement()
        {
            ViewBag.ActivePage = "AttendanceManagement";
            return View();
        }

        public ActionResult LeaveManagement()
        {
            ViewBag.ActivePage = "LeaveManagement";
            return View();
        }

        public ActionResult Ticketing()
        {
            ViewBag.ActivePage = "Ticketing";
            return View();
        }
    }
}