using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class EmpTicketingController : Controller
    {
        // GET: EmpTicketing
        public ActionResult Index()
        {
             return View("~/Views/EmployeeDashboard/EmpTicketing.cshtml");
        }
    }
}