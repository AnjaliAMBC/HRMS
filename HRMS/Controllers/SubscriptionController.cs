﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class SubscriptionController : Controller
    {
        // GET: Subscription
        public ActionResult SubscriptionInfo()
        {
            return View("~/Views/Itsupport/SubscriptionInfo.cshtml");
        }
    }
}