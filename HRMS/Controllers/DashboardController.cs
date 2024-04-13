using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class DashboardController : Controller
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }
    }
}