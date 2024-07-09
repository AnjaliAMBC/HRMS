using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "AdminDashboard",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "AdminDashboard", action = "Dashboard", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "EmployeeDashboard",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "EmployeeDashboard", action = "Dashboard", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "EmpAttendance",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "EmpAttendance", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "adnAttendance",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "AdminAttendance", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "EmployeeDash",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "EmpDash", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "AdminDash",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "AdminDash", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "EmpLeave",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "EmpLeave", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "AdminLeave",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "AdminLeave", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "AdminLeaveBalance",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "AdminLeave", action = "AdminLeaveBalance", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "AdminLeaveApply",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "AdminLeave", action = "AdminLeaveApply", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "CompOff",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Compoff", action = "SubmitCompOff", id = UrlParameter.Optional }
          );
            routes.MapRoute(
              name: "AdminCompOff",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "AdminLeave", action = "AdminLeaveCompensatoryOff", id = UrlParameter.Optional }
          );
            routes.MapRoute(
             name: "EmpTicketing",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "EmpTicketing", action = "Index", id = UrlParameter.Optional }
         );
        }
    }
}
