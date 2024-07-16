using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminITTickettingController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;
        public ActionResult Index()
        {
       
            return View("~/Views/AdminDashboard/AdminTicketingView.cshtml");
        }
        
        public ActionResult AdminItTicketOpenClose()
        {
          return View("~/Views/AdminDashboard/AdminItTicketOpenClose.cshtml");
        }
    }
}