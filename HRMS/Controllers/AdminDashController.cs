using HRMS.Helpers;
using HRMS.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminDashController : Controller
    {
        // GET: AdminDash
        public ActionResult Index()
        {
            var model = new DashboardViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            return PartialView("~/Views/AdminDashboard/AdminDash.cshtml", model);
        }
    }
}