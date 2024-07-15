using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminHrTicketingController : Controller
    {
        // GET: AdminHrTicketing
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminHrTicket()
        {
            return View("~/Views/AdminDashboard/AdminHrTicketing.cshtml");
        }
        public ActionResult AdminHrTicketOpenClose()
        {
             return View("~/Views/AdminDashboard/AdminHrTicketOpenCloseView.cshtml");
        }
    }
}