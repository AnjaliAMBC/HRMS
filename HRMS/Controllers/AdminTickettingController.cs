using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminTickettingController : Controller
    {
        // GET: AdminTicketting
        public ActionResult Index()
        {
       
            return View("~/Views/AdminDashboard/AdminTicketingView.cshtml");
        }
        public ActionResult AdminHrTicket()
        {
            return View("~/Views/AdminDashboard/AdminHrTicketing.cshtml");
        }
        public ActionResult AdminItAssignAgent()
        {
       
            return PartialView("~/Views/AdminDashboard/AdminAssignAgent.cshtml");
        }
    }
}