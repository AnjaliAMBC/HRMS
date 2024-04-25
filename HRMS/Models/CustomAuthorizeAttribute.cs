using System;
using System.Web;
using System.Web.Mvc;
using HRMS.Helpers;

namespace HRMS.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //private readonly HttpSessionStateBase _session;
        //private readonly SiteContext _siteCotext;
        //private readonly HRMS_EntityFramework _dbContext;

        //public CustomAuthorizeAttribute()
        //{
        //    _session = new HttpSessionStateWrapper(HttpContext.Current.Session);
        //    _siteCotext = new SiteContext(_dbContext, _session);
        //}
        // Override the AuthorizeCore method to implement custom authorization logic
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            //var sessionValue = _siteCotext.RetrieveFromSession("SiteContext");
            //httpContext.Session[] as SiteContextModel;
            //
            var sessionValue = httpContext.Session["SiteContext"] as SiteContextModel;

            // Check if the user belongs to the specified role (if any)
            if (!string.IsNullOrEmpty(Roles))
            {
                string[] roles = Roles.Split(',');
                foreach (var role in roles)
                {
                    if (sessionValue.LoginInfo.EmployeeRole == role)
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

            filterContext.Result = result; ;
        }
    }
}