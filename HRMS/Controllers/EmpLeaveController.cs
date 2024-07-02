﻿using HRMS.Models.Employee;
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
        public ActionResult EmpApplyLeave(string leaveRequestName)
        {
            var applyleaveModel = new ApplyLeaveViewModel();

            var employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            applyleaveModel.employees = employees;

            var leaves = _dbContext.con_leaveupdate
                    .Where(x => x.LeaveRequestName == leaveRequestName).ToList();

            applyleaveModel.EditLeaveInfo = leaves.FirstOrDefault();
            applyleaveModel.Leaves = JsonConvert.SerializeObject(leaves);

            applyleaveModel.leaveTypes = new List<string>
            {
                "Earned Leave",
                "Emergency Leave",
                "Sick Leave",
                "Bereavement Leave",
                "Hourly Permission",
                "Marriage Leave",
                "Maternity Leave",
                "Paternity Leave",
                "Comp Off"
            };

            applyleaveModel.HourlyPermissions = new List<string>
            {
                "0.5 Hours",
                "1 Hour",
                "1.5 Hours",
                "2 Hours"
            };



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

                // Check if ActionType is Edit
                if (leaveRequest.ActionType == "Edit")
                {
                    // Remove existing leaves within the edited date range
                    var existingLeaves = _dbContext.con_leaveupdate
                        .Where(l => l.LeaveRequestName == leaveRequest.EditRequestName)
                        .ToList();

                    if (existingLeaves.Any())
                    {
                        _dbContext.con_leaveupdate.RemoveRange(existingLeaves);
                        _dbContext.SaveChanges();
                    }
                }

                var leaves = new List<con_leaveupdate>();

                if (leaveRequest.DayTypeEntries != null && leaveRequest.DayTypeEntries.Count() > 0)
                {
                    foreach (var DayTypeEntry in leaveRequest.DayTypeEntries)
                    {
                        leaves.Add(new con_leaveupdate()
                        {
                            employee_id = leaveRequest.EmpID,
                            employee_name = leaveRequest.EmpName,
                            leavecategory = leaveRequest.LeaveType + " " + DayTypeEntry.DayType.Replace("fullDay", "Full day").Replace("halfDay", DayTypeEntry.HalfType),
                            leavedate = Convert.ToDateTime(DayTypeEntry.Date),
                            leavesource = leaveRequest.LeaveType,
                            leaveuniqkey = leaveRequest.EmpID + "_" + DayTypeEntry.Date,
                            leave_reason = leaveRequest.Reason,
                            submittedby = leaveRequest.SubmittedBy,
                            DayType = DayTypeEntry.DayType,
                            HalfDayCategory = DayTypeEntry.HalfType,
                            BackupResource_Name = leaveRequest.BackupResource_Name,
                            EmergencyContact_no = leaveRequest.EmergencyContact_no,
                            LeaveDays = DayTypeEntry.DayType == "fullDay" ? (decimal)1 : (decimal)0.5,
                            LeaveStatus = "Pending",
                            Fromdate = Convert.ToDateTime(leaveRequest.FromDate),
                            Todate = Convert.ToDateTime(leaveRequest.ToDate),
                            LeaveRequestName = leaveRequest.EmpID + "_" + leaveRequest.FromDate + "_" + leaveRequest.ToDate,
                            OfficalEmailid = leaveRequest.OfficalEmailid,
                            Location = leaveRequest.Location,
                            createdby = leaveRequest.SubmittedBy,
                            createddate = DateTime.Now,
                            updatedby = leaveRequest.SubmittedBy,
                            updateddate = DateTime.Now,
                            Designation = leaveRequest.Designation,
                            Department = leaveRequest.Department,

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
                        Location = leaveRequest.Location,
                        createdby = leaveRequest.SubmittedBy,
                        createddate = DateTime.Now,
                        updatedby = leaveRequest.SubmittedBy,
                        updateddate = DateTime.Now,
                        Designation = leaveRequest.Designation,
                        Department = leaveRequest.Department,
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
            //int currentYear = DateTime.Now.Year;
            //string january1stString = $"{currentYear}-01-01";
            //string december31stString = $"{currentYear}-12-31";

            //DateTime january1st = DateTime.ParseExact(january1stString, "yyyy-MM-dd", null);
            //DateTime december31st = DateTime.ParseExact(december31stString, "yyyy-MM-dd", null);

            //var employee = _dbContext.emp_info.FirstOrDefault(x => x.EmployeeID == empId);
            //if (employee == null)
            //{
            //    // Handle case where employee is not found
            //    return Json(new { error = "Employee not found" }, JsonRequestBehavior.AllowGet);
            //}

            //var leaveTypeCategory = new LeavesCategory();
            //leaveTypeCategory.Type = leaveType;

            //var availableLeaves = new LeaveCalculator().CalculateAvailableLeaves(employee, leaveTypeCategory);

            LeaveTypesBasedOnEmpViewModel empLeaveTypes = new LeaveCalculator().GetLeavesByEmp(empId);
            var availableLeaves = new AvailableLeaves();

            if (empLeaveTypes != null && empLeaveTypes.LeaveTypes.Count() > 0)
            {
                availableLeaves = empLeaveTypes.LeaveTypes[0].AvailableLeaves.Where(x => x.Type == leaveType).FirstOrDefault();
            }


            // Return available leaves as JSON
            return Json(availableLeaves, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmpLeaveHistory(string empId, int year)
        {
            List<LeaveInfo> result = new LeaveCalculator().EmpLeaveInfo(empId, year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult EmpLeaveEdit(string leavenumber)
        {
            var jsonResponse = new JsonResponse();

            try
            {
                var editItems = _dbContext.con_leaveupdate
                       .Where(x => x.LeaveRequestName == leavenumber).ToList();

                return Json(editItems.FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonResponse = ErrorHelper.CaptureError(ex);
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
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
                        item.LeaveStatus = "Cancelled";
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


        [HttpPost]
        public ActionResult ApproveLeave(string leaveRequestName)
        {
            var leaves = _dbContext.con_leaveupdate.Where(l => l.LeaveRequestName == leaveRequestName).ToList();
            if (leaves != null)
            {
                foreach (var leave in leaves)
                {
                    leave.LeaveStatus = "Approved";
                }
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult RejectLeave(string leaveRequestName)
        {
            var leaves = _dbContext.con_leaveupdate.Where(l => l.LeaveRequestName == leaveRequestName).ToList();
            if (leaves != null)
            {
                foreach (var leave in leaves)
                {
                    leave.LeaveStatus = "Rejected";
                }
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult GetAllEmpLeavesInfoBasedonDate(string startdate, string enddate, string leaverequestname, string empID)
        {
            DateTime dateStart = DateTime.ParseExact(startdate, "yyyy-MM-dd", null);
            DateTime dateEnd = DateTime.ParseExact(enddate, "yyyy-MM-dd", null);
            var leaves = new List<con_leaveupdate>();

            if (string.IsNullOrWhiteSpace(leaverequestname))
            {
                // Query leave data and group by LeaveRequestName
                leaves = _dbContext.con_leaveupdate
                   .Where(x => x.leavedate >= dateStart && x.leavedate <= dateEnd && x.employee_id == empID)
                   .OrderByDescending(x => x.Fromdate)
                   .ToList();
            }
            else
            {
                // Query leave data and group by LeaveRequestName
                if (!string.IsNullOrWhiteSpace(empID))
                {
                    leaves = _dbContext.con_leaveupdate
                  .Where(x => x.Fromdate >= dateStart && x.Todate <= dateEnd && x.LeaveRequestName == leaverequestname && x.employee_id == empID)
                  .OrderByDescending(x => x.Fromdate)
                  .ToList();
                }
                else
                {
                    leaves = _dbContext.con_leaveupdate
                 .Where(x => x.Fromdate >= dateStart && x.Todate <= dateEnd && x.LeaveRequestName == leaverequestname)
                 .OrderByDescending(x => x.Fromdate)
                 .ToList();
                }

            }
            var json = JsonConvert.SerializeObject(leaves);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

    }
}
