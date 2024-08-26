using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        public ActionResult Index()
        {
            return View("~/Views/Itsupport/PurchaseListView.cshtml");
        }

        public ActionResult PurchaseAdd()
        {
            return View("~/Views/Itsupport/PurchaseAdd.cshtml");
        }

        public ActionResult PurchaseImport()
        {
            return View("~/Views/Itsupport/PurchangeImport.cshtml");
        }

        public ActionResult PurchasesuperAdmin()
        {
            return View("~/Views/Itsupport/PurchaseSuperAdminView.cshtml");
        }

        public ActionResult PurchaseItSuperAdmin()
        {
            return View("~/Views/Itsupport/PurchaseItSuperAdmin.cshtml");
        }        
    }
}
