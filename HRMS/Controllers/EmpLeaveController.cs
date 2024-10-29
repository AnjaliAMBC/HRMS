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
    using static HRMS.Helpers.PartialViewHelper;
    public class EmpLeaveController : BaseController
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

            return View("~/Views/EmployeeDashboard/EmpApplyleave.cshtml", applyleaveModel);
        }

        public ActionResult AjaxApplyLeave(LeaveRequestModel leaveRequest)
        {
            var currentContext = Session["SiteContext"] as HRMS.Models.SiteContextModel;

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

                if (leaveRequest.ActionType == "Update")
                {
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
                            TeamEmails = leaveRequest.TeamEmail

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
                        TeamEmails = leaveRequest.TeamEmail
                    });
                }

                _dbContext.con_leaveupdate.AddRange(leaves);
                _dbContext.SaveChanges();

                leaveRequest.jsonResponse.Message = "Leave request submitted successfully.";
                leaveRequest.jsonResponse.StatusCode = 200;

                //When employee apply leave
                if (leaves[0].employee_name.Contains(leaveRequest.SubmittedBy))
                {
                    var emailSubject = "Leave Application from " + leaveRequest.EmpName + " on " + System.DateTime.Now.ToString("dd MMMM yyyy");
                    var emailBody = RenderPartialToString(this, "_LeaveNotificationEmpEmail", leaves, ViewData, TempData);

                    var teamEmails = "";
                    if (!string.IsNullOrWhiteSpace(leaveRequest.TeamEmail.Trim()))
                    {
                        teamEmails = "," + leaveRequest.TeamEmail.Trim();
                    }
                    teamEmails = teamEmails.Trim().Trim(',');

                    var emailRequest = new EmailRequest()
                    {
                        Body = emailBody,
                        ToEmail = ConfigurationManager.AppSettings["LeaveEmails"],
                        CCEmail = !string.IsNullOrWhiteSpace(teamEmails) ? teamEmails : string.Empty,
                        BCCEmail = leaveRequest.OfficalEmailid, // Add official email ID to BCC
                        Subject = emailSubject
                    };

                    var sendNotification = EMailHelper.SendEmail(emailRequest);
                }

                // In case admin submits the leave on employee's behalf
                else
                {
                    var emailSubject = "Leave Application from " + leaveRequest.EmpName + " on " + System.DateTime.Now.ToString("dd MMMM yyyy");
                    var emailBody = RenderPartialToString(this, "_LeaveNotificationAdminEmail", leaves, ViewData, TempData);

                    var teamEmails = "";
                    if (!string.IsNullOrWhiteSpace(leaveRequest.TeamEmail))
                    {
                        teamEmails = "," + leaveRequest.TeamEmail.Trim();
                    }
                    teamEmails = teamEmails.Trim().Trim(',');

                    var emailRequest = new EmailRequest()
                    {
                        Body = emailBody,
                        ToEmail = ConfigurationManager.AppSettings["LeaveEmails"],
                        CCEmail = !string.IsNullOrWhiteSpace(teamEmails) ? teamEmails : string.Empty,
                        BCCEmail = leaveRequest.OfficalEmailid, // Add official email ID to BCC
                        Subject = emailSubject
                    };

                    var sendNotification = EMailHelper.SendEmail(emailRequest);
                }

                var newNotification = new Notification
                {
                    NotificationDate = DateTime.Now,
                    NotificationFromName = currentContext.EmpInfo.EmployeeName,
                    NotificationFromID = currentContext.EmpInfo.EmployeeID,
                    NotificationToName = leaveRequest.EmpName,
                    NotificationToID = leaveRequest.EmpID,
                    NotificationType = "Leave",
                    Status = leaveRequest.ActionType == "Update" ? "Updated" : "Submitted",
                    Comments = "",
                    CreatedDate = DateTime.Now
                };

                _dbContext.Notifications.Add(newNotification);
                _dbContext.SaveChanges();

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

            LeaveTypesBasedOnEmpViewModel empLeaveTypes = new LeaveCalculator().GetLeavesByEmp(empId);
            var availableLeaves = new AvailableLeaves();

            if (empLeaveTypes != null && empLeaveTypes.LeaveTypes.Count() > 0)
            {
                availableLeaves = empLeaveTypes.LeaveTypes[0].AvailableLeaves.Where(x => x.Type == leaveType).FirstOrDefault();
            }

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
            var currentContext = Session["SiteContext"] as HRMS.Models.SiteContextModel;


            var jsonResponse = new JsonResponse();

            try
            {
                // Fetch the records to be cancelled
                var canLeaveItems = _dbContext.con_leaveupdate
                    .Where(x => x.LeaveRequestName == leavenumber)
                    .ToList();

                if (canLeaveItems != null && canLeaveItems.Any())
                {
                    // Store the existing data temporarily
                    var recreatedItems = new List<con_leaveupdate>();

                    var leaveRequestName = Guid.NewGuid().ToString();

                    foreach (var item in canLeaveItems)
                    {
                        // Copy the existing data into a new object, changing the leaveuniqkey
                        var newItem = new con_leaveupdate
                        {
                            leaveno = item.leaveno,
                            leavedate = item.leavedate,
                            employee_id = item.employee_id,
                            leave_reason = item.leave_reason,
                            DayType = item.DayType,
                            LeaveDays = item.LeaveDays,
                            HalfDayCategory = item.HalfDayCategory,
                            submittedby = item.submittedby,
                            leavesource = item.leavesource,
                            leaveuniqkey = Guid.NewGuid().ToString(), // Generate a new unique key
                            leavecategory = item.leavecategory,
                            employee_name = item.employee_name,
                            BackupResource_Name = item.BackupResource_Name,
                            EmergencyContact_no = item.EmergencyContact_no,
                            LeaveStatus = "Cancelled", // Update the leave status
                            OfficalEmailid = item.OfficalEmailid,
                            Fromdate = item.Fromdate,
                            Todate = item.Todate,
                            LeaveRequestName = leaveRequestName,
                            Location = item.Location,
                            createdby = item.createdby,
                            createddate = item.createddate,
                            updatedby = item.updatedby,
                            updateddate = item.updateddate,
                            Designation = item.Designation,
                            Department = item.Department,
                            TeamEmails = item.TeamEmails,
                            approvedbyname = item.approvedbyname,
                            approvedbydate = item.approvedbydate,
                            rejectedbyname = item.rejectedbyname,
                            rejectedbydate = item.rejectedbydate
                        };

                        recreatedItems.Add(newItem);
                    }

                    // Remove the existing records
                    _dbContext.con_leaveupdate.RemoveRange(canLeaveItems);

                    // Add the new records with the updated leaveuniqkey
                    _dbContext.con_leaveupdate.AddRange(recreatedItems);

                    // Save changes to the database
                    _dbContext.SaveChanges();

                    jsonResponse.Message = "Leave cancelled and records updated successfully.";
                    jsonResponse.StatusCode = 200;


                    var newNotification = new Notification
                    {
                        NotificationDate = DateTime.Now,
                        NotificationFromName = currentContext.EmpInfo.EmployeeName,
                        NotificationFromID = currentContext.EmpInfo.EmployeeID,
                        NotificationToName = canLeaveItems[0].employee_name,
                        NotificationToID = canLeaveItems[0].employee_id,
                        NotificationType = "Leave",
                        Status = "Cancelled",
                        Comments = "",
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.Notifications.Add(newNotification);
                    _dbContext.SaveChanges();
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
            var cuserContext = SiteContext.GetCurrentUserContext();

            var leaves = _dbContext.con_leaveupdate.Where(l => l.LeaveRequestName == leaveRequestName).ToList();
            if (leaves != null)
            {
                foreach (var leave in leaves)
                {
                    leave.LeaveStatus = "Approved";
                    leave.approvedbyname = cuserContext.LoginInfo.EmployeeName;
                    leave.approvedbydate = DateTime.Now;

                }
                _dbContext.SaveChanges();


                var emailSubject = "Leave Approved";
                var emailBody = RenderPartialToString(this, "_LeaveApprovedEmail", leaves, ViewData, TempData);


                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = leaves[0].OfficalEmailid,
                    Subject = emailSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);

                var newNotification = new Notification
                {
                    NotificationDate = DateTime.Now,
                    NotificationFromName = cuserContext.EmpInfo.EmployeeName,
                    NotificationFromID = cuserContext.EmpInfo.EmployeeID,
                    NotificationToName = leaves[0].employee_name,
                    NotificationToID = leaves[0].employee_id,
                    NotificationType = "Leave",
                    Status = "Approved",
                    Comments = "",
                    CreatedDate = DateTime.Now
                };

                _dbContext.Notifications.Add(newNotification);
                _dbContext.SaveChanges();


                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult RejectLeave(string leaveRequestName)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var leaves = _dbContext.con_leaveupdate.Where(l => l.LeaveRequestName == leaveRequestName).ToList();
            if (leaves != null)
            {
                foreach (var leave in leaves)
                {
                    leave.LeaveStatus = "Rejected";
                    leave.rejectedbyname = cuserContext.LoginInfo.EmployeeName;
                    leave.rejectedbydate = DateTime.Now;
                }
                _dbContext.SaveChanges();

                var emailSubject = "Leave Rejected";
                var emailBody = RenderPartialToString(this, "_LeaveRejectedEmail", leaves, ViewData, TempData);

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = leaves[0].OfficalEmailid,
                    Subject = emailSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);


                var newNotification = new Notification
                {
                    NotificationDate = DateTime.Now,
                    NotificationFromName = cuserContext.EmpInfo.EmployeeName,
                    NotificationFromID = cuserContext.EmpInfo.EmployeeID,
                    NotificationToName = leaves[0].employee_name,
                    NotificationToID = leaves[0].employee_id,
                    NotificationType = "Leave",
                    Status = "Rejected",
                    Comments = "",
                    CreatedDate = DateTime.Now
                };

                _dbContext.Notifications.Add(newNotification);
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
                   .Where(x => x.leavedate >= dateStart && x.leavedate <= dateEnd && x.employee_id == empID && x.LeaveStatus != "Cancelled")
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
