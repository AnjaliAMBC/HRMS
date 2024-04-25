using HRMS.Models;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmployeeDashboardController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }
        public ActionResult Dashboard()
        {
            return View("~/Views/EmployeeDashboard/Dashboard.cshtml");
        }

        public ActionResult SelfService()
        {
            var model = new SelfServiceViewModel();
            if (Session["SiteContext"] != null)
            {
                var siteContext = Session["SiteContext"] as SiteContextModel;
                model.EmpInfo = siteContext.EmpInfo;
                model.LoginInfo = siteContext.LoginInfo;
                return View("~/Views/EmployeeDashboard/SelfService.cshtml", model);
            }
            return null;           
        }
    }
}