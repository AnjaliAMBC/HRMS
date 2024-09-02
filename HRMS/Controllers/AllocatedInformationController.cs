using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AllocatedInformationController : Controller
    {
        // GET: AllocatedInformation
        public ActionResult AllocatedInfo()
        {
            return View("~/Views/Itsupport/AllocatedInfo.cshtml");
        }
    }
}