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
            return View();
        }
    }
}