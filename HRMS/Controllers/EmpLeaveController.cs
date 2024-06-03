using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Models;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using HRMS;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Security;

namespace HRMS.Controllers
{
    public class EmpLeaveController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmpLeaveController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: EmpLeave
        public ActionResult Index()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpLeaveTracker.cshtml");
        }
        public ActionResult EmpApplyLeave()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }

        public ActionResult AjaxApplyLeave(LeaveRequestModel leaveRequest)
        {
            if (leaveRequest == null ||
                leaveRequest.LeaveType <= 0 ||
                string.IsNullOrEmpty(leaveRequest.FromDate) ||
                string.IsNullOrEmpty(leaveRequest.ToDate) ||
                string.IsNullOrEmpty(leaveRequest.TeamEmail) ||
                leaveRequest.DayTypeEntries == null ||
                leaveRequest.DayTypeEntries.Count == 0)
            {                
                return Json(leaveRequest, JsonRequestBehavior.AllowGet);
            }

            foreach(var DayTypeEntrie in leaveRequest.DayTypeEntries)
            {

            }

            return Json(leaveRequest, JsonRequestBehavior.AllowGet);
        }
    }
}
