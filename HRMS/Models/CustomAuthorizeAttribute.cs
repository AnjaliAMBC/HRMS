using System;
using System.Web;
using System.Web.Mvc;
using HRMS.Helpers;

namespace HRMS.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        // Override the AuthorizeCore method to implement custom authorization logic
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var sessionValue = httpContext.Session["SiteContext"] as SiteContextModel;

            if (sessionValue == null)
            {
                return false;
            }

            // Check if the user belongs to the specified role (if any)
            if (!string.IsNullOrEmpty(Roles))
            {
                string[] roles = Roles.Split(',');
                foreach (var role in roles)
                {
                    if (sessionValue.LoginInfo.EmployeeRole.Contains("HR Admin") || sessionValue.LoginInfo.EmployeeRole.Contains("Super Admin") || sessionValue.LoginInfo.EmployeeRole.Contains("IT Admin"))
                    {
                        return true;
                    }
                }
                return false;
            }

            // If no roles are specified, then any authenticated user is allowed
            return true;
        }

        // Override the HandleUnauthorizedRequest method to customize the behavior when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Custom access denied message
            string message = "You are not authorized to access this page.";

            // Create a custom ActionResult to display the access denied message
            var result = new ViewResult
            {
                ViewName = "AccessDenied", // Name of your custom access denied view
                ViewData = new ViewDataDictionary { { "Message", message } }
            };

            filterContext.Result = result;
        }
    }
}
