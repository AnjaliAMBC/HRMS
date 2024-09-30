using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class MaintananceController : Controller
    {
        // GET: Maintanance
        public ActionResult MaintananceInfo()
        {
            return View("~/Views/Itsupport/Maintanance.cshtml");
        }
        public ActionResult EmpMaintananceHistory()
        {
            return View("~/Views/Itsupport/EmpMaintananceHistory.cshtml");
        }
        public ActionResult MaintananceApproval()
        {
            return View("~/Views/Itsupport/MaintananceApprovalView.cshtml");
        }

    }
   }
