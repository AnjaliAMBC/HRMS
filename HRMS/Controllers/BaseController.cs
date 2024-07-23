using HRMS.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string currentAction = filterContext.ActionDescriptor.ActionName;
            string currentController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // Skip session validation for the login action
            if (currentController.Equals("Account", StringComparison.OrdinalIgnoreCase) &&
                currentAction.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (Session["SiteContext"] == null)
            {
                throw new SessionTimeoutException("Session has expired.");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}