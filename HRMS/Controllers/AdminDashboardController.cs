using HRMS.Models;
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
        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Dashboard()
        {
            //if (Session["SiteContext"] != null)
            //{
            //    var siteContext = Session["SiteContext"] as SiteContext;
            //    //model.EmpInfo = siteContext.EmpInfo;
            //    //model.LoginInfo = siteContext.LoginInfo;
            //    if(siteContext.LoginInfo.EmployeeRole == "HR Admin")
            //    {
            ViewBag.ActivePage = "Dashboard";
            return View("~/Views/AdminDashboard/Dashboard.cshtml");
            //}
            //else
            //{
            //    return View("~/Views/Shared/Error.cshtml");
            //}

        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult EmployeeManagement()
        {
            //ViewBag.ActivePage = "EmployeeManagement";
            return PartialView("~/Views/AdminDashboard/EmpManagement.cshtml");
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult AttendanceManagement()
        {
            //ViewBag.ActivePage = "AttendanceManagement";
            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml");
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult LeaveManagement()
        {
            ViewBag.ActivePage = "LeaveManagement";
            return View();
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Ticketing()
        {
            ViewBag.ActivePage = "Ticketing";
            return View();
        }
    }
}