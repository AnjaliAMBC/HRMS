using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class SiteContext
    {
        //// Database context
        //private readonly HRMS_EntityFramework _dbContext;
        //private readonly HttpContextBase _session;

        //// Constructor to initialize database context
        //public SiteContext(HRMS_EntityFramework context, HttpContextBase session = null)
        //{
        //    _dbContext = context;
        //    _session = session;
        //}

        //public void SetSiteContext(string empid, emplogin emplogin)
        //{
        //    var SiteContext = new SiteContextModel();
        //    var currentEmployee = _dbContext.emp_info.Where(emp => emp.EmployeeID == empid).FirstOrDefault();
        //    SiteContext.LoginInfo = emplogin;
        //    SiteContext.EmpInfo = currentEmployee;
        //    StoreInSession("SiteContext", SiteContext);
        //}


        //// Store data in session
        //public void StoreInSession(string key, SiteContextModel data)
        //{
        //    httpContext.se
        //    _session[key] = data;
        //}

        //// Retrieve data from session
        //public SiteContextModel RetrieveFromSession(string key)
        //{
        //    return _session[key] as SiteContextModel;
        //}

        public static SiteContextModel GetCurrentUserContext()
        {
            var contextModel = new SiteContextModel();
            if (HttpContext.Current.Session["SiteContext"] != null)
            {
                var siteContext = HttpContext.Current.Session["SiteContext"] as SiteContextModel;
                contextModel.EmpInfo = siteContext.EmpInfo;
                contextModel.LoginInfo = siteContext.LoginInfo;
                contextModel.IsAdmin = siteContext.IsAdmin;
                contextModel.IsSuperAdmin = siteContext.IsSuperAdmin;
                contextModel.IsITAdmin = siteContext.IsITAdmin;
                contextModel.IsAccountAdmin = siteContext.IsAccountAdmin;
                contextModel.EmpBasedClients = siteContext.EmpBasedClients;
                //contextModel.CheckInInfo = siteContext.CheckInInfo;
                //contextModel.empLastDayCheckInDetails = siteContext.empLastDayCheckInDetails;
            }

            return contextModel;
        }
    }
}