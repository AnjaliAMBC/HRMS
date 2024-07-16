using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public static class SuperAdminHelper
    {
        public static bool IsSuperAdmin(SiteContextModel currentContext)
        {
            return currentContext.LoginInfo.EmployeeRole.Contains("Super Admin");
        }

        public static List<string> GetAdminLocations(SiteContextModel currentContext)
        {
            var adminLocations = new List<string>();
            if (IsSuperAdmin(currentContext))
            {
                var superAdminLocations = currentContext.LoginInfo.EmployeeRole.Split(',');

                foreach (var superAdminLocation in superAdminLocations)
                {
                    if(superAdminLocation != "Super Admin")
                    {
                        var locationName = superAdminLocation.Replace("Admin", "").Trim();
                        adminLocations.Add(locationName);
                    }                   
                }
            }
            return adminLocations;
        }

    }
}