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
using HRMS.Helpers;

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
            return View("~/Views/EmployeeDashboard/EmpLeaveTracker.cshtml");
        }
        public ActionResult EmpApplyLeave()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }

        public ActionResult AjaxApplyLeave(LeaveRequestModel leaveRequest)
        {
            if (leaveRequest == null ||
               string.IsNullOrEmpty(leaveRequest.LeaveType) ||
                string.IsNullOrEmpty(leaveRequest.FromDate) ||
                string.IsNullOrEmpty(leaveRequest.ToDate) ||
                string.IsNullOrEmpty(leaveRequest.TeamEmail) ||
                leaveRequest.DayTypeEntries == null ||
                leaveRequest.DayTypeEntries.Count == 0)
            {
                return Json(leaveRequest, JsonRequestBehavior.AllowGet);
            }

            var leaves = new List<con_leaveupdate>();

            foreach (var DayTypeEntrie in leaveRequest.DayTypeEntries)
            {
                leaves.Add(new con_leaveupdate()
                {
                    employee_id = leaveRequest.EmpID,
                    employee_name = leaveRequest.EmpName,
                    leavecategory = leaveRequest.LeaveType + " " + DayTypeEntrie.DayType.Replace("fullDay", "Full day").Replace("halfDay", DayTypeEntrie.HalfType),
                    leavedate = Convert.ToDateTime(DayTypeEntrie.Date),
                    leavesource = leaveRequest.LeaveType,
                    leaveuniqkey = leaveRequest.EmpID + "_" + DayTypeEntrie.Date,
                    leave_reason = leaveRequest.Reason,
                    submittedby = leaveRequest.SubmittedBy,
                    DayType = DayTypeEntrie.DayType,
                    HalfDayCategory = DayTypeEntrie.HalfType,
                    BackupResource_Name = leaveRequest.BackupResource_Name,
                    EmergencyContact_no = leaveRequest.EmergencyContact_no,
                    LeaveDays = DayTypeEntrie.DayType == "fullDay" ? (decimal)1 : (decimal)0.5
                });
            }

            _dbContext.con_leaveupdate.AddRange(leaves);
            _dbContext.SaveChanges();

            return Json(leaveRequest, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAvailableLeaves(string empId, string leaveType)
        {
            int currentYear = DateTime.Now.Year;
            string january1stString = $"{currentYear}-01-01";
            string december31stString = $"{currentYear}-12-31";

            DateTime january1st = DateTime.ParseExact(january1stString, "yyyy-MM-dd", null);
            DateTime december31st = DateTime.ParseExact(december31stString, "yyyy-MM-dd", null);

            var employee = _dbContext.emp_info.FirstOrDefault(x => x.EmployeeID == empId);
            if (employee == null)
            {
                // Handle case where employee is not found
                return Json(new { error = "Employee not found" }, JsonRequestBehavior.AllowGet);
            }

            var leaves = _dbContext.con_leaveupdate
                .Where(x => x.employee_id == empId && x.leavedate >= january1st && x.leavedate <= december31st && x.leavesource == leaveType)
                .ToList();


            var availableLeaves = LeaveCalculator.CalculateAvailableLeaves(employee, leaves, leaveType);

            // Return available leaves as JSON
            return Json(availableLeaves, JsonRequestBehavior.AllowGet);
        }
    }
}
