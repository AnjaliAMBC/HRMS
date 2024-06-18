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
            var applyleaveModel = new ApplyLeaveViewModel();

            var employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            applyleaveModel.employees = employees;
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml", applyleaveModel);
        }

        public ActionResult AjaxApplyLeave(LeaveRequestModel leaveRequest)
        {
            try
            {
                if (leaveRequest == null ||
                      string.IsNullOrEmpty(leaveRequest.LeaveType) ||
                       string.IsNullOrEmpty(leaveRequest.FromDate) ||
                       string.IsNullOrEmpty(leaveRequest.TeamEmail) ||
                       (leaveRequest.DayTypeEntries == null && string.IsNullOrEmpty(leaveRequest.hourPermission)))
                {
                    return Json(leaveRequest, JsonRequestBehavior.AllowGet);
                }

                var leaves = new List<con_leaveupdate>();

                if (leaveRequest.DayTypeEntries != null && leaveRequest.DayTypeEntries.Count() > 0)
                {
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
                            LeaveDays = DayTypeEntrie.DayType == "fullDay" ? (decimal)1 : (decimal)0.5,
                            LeaveStatus = "Pending",
                            Fromdate = Convert.ToDateTime(leaveRequest.FromDate),
                            Todate = Convert.ToDateTime(leaveRequest.ToDate),
                            LeaveRequestName = leaveRequest.EmpID + "_" + leaveRequest.FromDate + "_" + leaveRequest.ToDate,
                            OfficalEmailid = leaveRequest.OfficalEmailid,
                            Location = leaveRequest.Location
                        });
                    }
                }
                else
                {
                    leaves.Add(new con_leaveupdate()
                    {
                        employee_id = leaveRequest.EmpID,
                        employee_name = leaveRequest.EmpName,
                        leavecategory = leaveRequest.LeaveType,
                        leavedate = Convert.ToDateTime(leaveRequest.FromDate),
                        leavesource = leaveRequest.LeaveType,
                        leaveuniqkey = leaveRequest.EmpID + "_" + leaveRequest.FromDate,
                        leave_reason = leaveRequest.Reason,
                        submittedby = leaveRequest.SubmittedBy,
                        DayType = "",
                        HalfDayCategory = "",
                        BackupResource_Name = leaveRequest.BackupResource_Name,
                        EmergencyContact_no = leaveRequest.EmergencyContact_no,
                        LeaveDays = System.Convert.ToDecimal(leaveRequest.hourPermission),
                        LeaveStatus = "Pending",
                        Fromdate = Convert.ToDateTime(leaveRequest.FromDate),
                        Todate = Convert.ToDateTime(leaveRequest.ToDate),
                        LeaveRequestName = leaveRequest.EmpID + "_" + leaveRequest.FromDate + "_" + leaveRequest.ToDate,
                        OfficalEmailid = leaveRequest.OfficalEmailid,
                        Location = leaveRequest.Location
                    });
                }

                _dbContext.con_leaveupdate.AddRange(leaves);
                _dbContext.SaveChanges();

                leaveRequest.jsonResponse.Message = "Leave request submitted successfully.";
                leaveRequest.jsonResponse.StatusCode = 200;
                return Json(leaveRequest, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                leaveRequest.jsonResponse = ErrorHelper.CaptureError(ex);
                return Json(leaveRequest, JsonRequestBehavior.AllowGet);
            }
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

            var leaveTypeCategory = new LeavesCategory();
            leaveTypeCategory.Type = leaveType;

            var availableLeaves = new LeaveCalculator().CalculateAvailableLeaves(employee, leaveTypeCategory);

            // Return available leaves as JSON
            return Json(availableLeaves, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmpLeaveHistory(string empId, int year)
        {
            int currentYear = year == 0 ? DateTime.Now.Year : year;
            string january1stString = $"{currentYear}-01-01";
            string december31stString = $"{currentYear}-12-31";

            DateTime january1st = DateTime.ParseExact(january1stString, "yyyy-MM-dd", null);
            DateTime december31st = DateTime.ParseExact(december31stString, "yyyy-MM-dd", null);

            var leaves = _dbContext.con_leaveupdate
                .Where(x => x.employee_id == empId && x.leavedate >= january1st && x.leavedate <= december31st)
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            return Json(leaves, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmpLeaveCancel(string leavenumber)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var canLeaveItems = _dbContext.con_leaveupdate
                       .Where(x => x.LeaveRequestName == leavenumber).ToList();

                if (canLeaveItems != null && canLeaveItems.Any())
                {
                    foreach (var item in canLeaveItems)
                    {
                        //item.LeaveStatus = "Cancelled";
                        //item.leaveuniqkey = item.leaveuniqkey + DateTime.Now.ToString();

                        _dbContext.con_leaveupdate.Remove(item);
                    }
                   

                    _dbContext.SaveChanges();

                    jsonResponse.Message = "Leave cancelled successfully.";
                    jsonResponse.StatusCode = 200;
                }
                else
                {
                    jsonResponse.Message = "No leave records found to cancel.";
                    jsonResponse.StatusCode = 404;
                }

                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonResponse = ErrorHelper.CaptureError(ex);
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult EmployeeLeavesByType(string empId)
        {
            LeaveTypesBasedOnEmpViewModel empLeaveTypes = new LeaveCalculator().GetLeavesByEmp(empId);
            return PartialView("~/Views/EmployeeDashboard/LeaveTypesAvailability.cshtml", empLeaveTypes);
        }

    }
}
