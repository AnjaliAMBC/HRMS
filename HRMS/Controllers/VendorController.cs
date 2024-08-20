using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class VendorController : Controller
    {
        // GET: Vendor
        public ActionResult Index()
        {
            return View("~/Views/AdminDashboard/VendorListView.cshtml");
        }
        public ActionResult AddVendor()
        {
            return View("~/Views/AdminDashboard/VendorAdd.cshtml");
        }
        public ActionResult ImportVendor()
        {
            return View("~/Views/AdminDashboard/VendorImport.cshtml");
        }
        public ActionResult ApproveVendor()
        {
            return View("~/Views/AdminDashboard/VendorApprovalPage.cshtml");
        }        
    }
}