using System;
using System.Web.Mvc;

namespace HRMS.Filters
{
    public class CustomExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is SessionTimeoutException)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                filterContext.ExceptionHandled = true;
            }
            else
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(filterContext.Exception)
                };
                filterContext.ExceptionHandled = true;
            }
        }
    }
}