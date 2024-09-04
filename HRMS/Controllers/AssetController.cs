using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AssetController : Controller
    {
        // GET: Asset
        public ActionResult AssetInfo()
        {
            return View("~/Views/Itsupport/AssetInfo.cshtml");
        }

        public ActionResult AddAsset()
        {
            return View("~/Views/Itsupport/AddAsset.cshtml");
        }
    }
}