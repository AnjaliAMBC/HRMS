using HRMS.Helpers;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;
    using Helpers;
    using HRMS.Models;
    using HRMS.Services;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;

    public class EmpDashController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmpDashController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: EmpDash
        public ActionResult Index()
        {
            var model = new EmpDashBoardModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            //model.CheckInInfo = cuserContext.CheckInInfo;
            //model.empLastDayCheckInDetails = cuserContext.empLastDayCheckInDetails;

            var empSignInfo = _dbContext.tbld_ambclogininformation.Where(x => x.Employee_Code == model.EmpInfo.EmployeeID).ToList();

            //Last Day
            var empLastSignedInfo = empSignInfo.OrderByDescending(x => x.Login_date).ToList().FirstOrDefault();
            if (empLastSignedInfo != null && empLastSignedInfo.Login_date != DateTime.Today)
            {
                model.empLastDayCheckInDetails.SignInTime = empLastSignedInfo.Signin_Time;
                if (empLastSignedInfo.Signout_Time != null && empLastSignedInfo.Signout_Time != DateTime.MinValue)
                {
                    model.empLastDayCheckInDetails.SignOutTime = empLastSignedInfo.Signout_Time ?? DateTime.MinValue;
                    model.empLastDayCheckInDetails.IsSignedOutOnLastCheckInDate = true;
                }
                else
                {
                    model.timerModel = new TimerModel(empLastSignedInfo.Signin_Time);
                    model.empLastDayCheckInDetails.LoginID = empLastSignedInfo.login_id;
                }
            }

            //Current Day
            var empCheckInInfo = empSignInfo.Where(x => x.Login_date == DateTime.Today).FirstOrDefault();

            if (empCheckInInfo != null)
            {
                model.todayCheckInInfo = empCheckInInfo;

                if (empCheckInInfo.Signin_Time != DateTime.MinValue && empCheckInInfo.Signout_Time.HasValue)
                {
                    TimeSpan difference = empCheckInInfo.Signout_Time.Value - empCheckInInfo.Signin_Time;
                    int hours = difference.Hours;
                    int minutes = difference.Minutes;
                    int seconds = difference.Seconds;

                    model.totalCheckedInTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
                }
                model.timerModel = new TimerModel(empCheckInInfo.Signin_Time);
            }

            model.AnniversaryModel = new EmployeeEventHelper().Anniversary().Where(a => a.Empid != model.EmpInfo.EmployeeID).ToList();
            model.Birthdays = new EmployeeEventHelper().Birthday().Where(a => a.Empid != model.EmpInfo.EmployeeID).ToList(); ;
            model.UpcomingHolidays = new EmployeeEventHelper().GetUpcomingHolidays(model.EmpInfo.Location);

            var isEmployeeAPpliedLeaveToday = _dbContext.con_leaveupdate.Where(x => x.employee_id == model.EmpInfo.EmployeeID && x.leavedate == DateTime.Today).FirstOrDefault();

            if (isEmployeeAPpliedLeaveToday != null)
            {
                model.empLeaveInfo = isEmployeeAPpliedLeaveToday;
                model.IsOnLeave = true;
            }

            var leaveTypes = new LeaveCalculator().GetLeavesByEmp(model.EmpInfo.EmployeeID);
            model.LeavesTypeInfo = leaveTypes;

            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            model.MyTickets = _dbContext.IT_Ticket
              .Where(x => x.EmployeeID == model.EmpInfo.EmployeeID && DbFunctions.TruncateTime(x.Created_date) == DateTime.Today)
              .ToList();


            model.NewJoiners = _dbContext.emp_info.Where(x => x.DOJ == DateTime.Today && x.EmployeeID != model.EmpInfo.EmployeeID).ToList();


            return View("~/Views/EmployeeDashboard/EmpDash.cshtml", model);
        }

        public ActionResult CheckIn()
        {
            var model = new CheckInOutModel();
            try
            {
                var cuserContext = SiteContext.GetCurrentUserContext();

                if (cuserContext.LoginInfo != null && !string.IsNullOrWhiteSpace(cuserContext.LoginInfo.EmployeeID))
                {
                    var checkInModel = new tbld_ambclogininformation();
                    checkInModel.Employee_Code = cuserContext.EmpInfo.EmployeeID;
                    checkInModel.Employee_Designation = cuserContext.EmpInfo.Designation;
                    checkInModel.Employee_LoginLocation = cuserContext.EmpInfo.Location;
                    checkInModel.Employee_Name = cuserContext.EmpInfo.EmployeeName;
                    checkInModel.Login_date = System.DateTime.Today;
                    checkInModel.Signin_Time = DateTime.Now;
                    checkInModel.Employee_Hostname = Dns.GetHostName();
                    checkInModel.Concat_loginstring = cuserContext.EmpInfo.EmployeeID + "_" + System.DateTime.Today;
                    checkInModel.Employee_Shift = "General";
                    checkInModel.Employee_IP = Dns.GetHostName();

                    var newCheckInItem = _dbContext.tbld_ambclogininformation.Add(checkInModel);
                    _dbContext.SaveChanges();

                    model.CheckInInfo = newCheckInItem;
                    model.JsonResponse.Message = "Check in Successful";
                    model.JsonResponse.StatusCode = 200;
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
                model.JsonResponse.Message = "Error occured while checkin";
                model.JsonResponse.StatusCode = 500;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckOut(int checkInID)
        {
            var model = new CheckInOutModel();
            try
            {
                if (checkInID != 0)
                {
                    var checkInRecord = _dbContext.tbld_ambclogininformation.Where(x => x.login_id == checkInID).FirstOrDefault();

                    if (checkInRecord != null)
                    {
                        checkInRecord.Signout_Time = DateTime.Now;
                        _dbContext.SaveChanges();

                        var siteContext = Session["SiteContext"] as SiteContextModel;

                        model.CheckInInfo = checkInRecord;
                        model.JsonResponse.StatusCode = 200;
                        model.JsonResponse.Message = "Check Out is successful";
                    }

                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
                model.JsonResponse.StatusCode = 500;
                model.JsonResponse.Message = "Error while Check Out";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCurrentTime(string signedInDateTime)
        {
            if (!string.IsNullOrWhiteSpace(signedInDateTime))
            {
                DateTime dateTime = DateTime.ParseExact(signedInDateTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (dateTime != DateTime.MinValue)
                {
                    var model = new TimerModel(dateTime);
                    return Json(model.FormattedTime, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);

        }


        public ActionResult JobReferral()
        {
            var jobListings = _dbContext.JobDetails
              .OrderByDescending(job => job.PostedDate)
              .ToList();

            var model = new EmpJobModel
            {
                jobdetail = jobListings
            };

            return View("/Views/EmployeeDashboard/EmpJobReferralView.cshtml", model);
        }

        public ActionResult JobDetail(int jobID)
        {
            var jobDetail = _dbContext.JobDetails.FirstOrDefault(x => x.JobID == jobID);
            var cuserContext = SiteContext.GetCurrentUserContext();
            var empID = cuserContext.EmpInfo.EmployeeID;

            var jobReferrals = _dbContext.JobReferrals.Where(x => x.JobID == jobID && x.ReferredById == empID).ToList();

            var model = new EmpJobModel
            {
                jobInfo = jobDetail,
                EmpInfo = cuserContext.EmpInfo,
                jobReferrals = jobReferrals
            };

            return View("/Views/EmployeeDashboard/EmpJobDetail.cshtml", model);
        }

        [HttpPost]
        public JsonResult ReferJob()
        {
            try
            {
                int JobID = Convert.ToInt32(Request.Form["JobID"]);
                string CandidateName = Request.Form["CandidateName"];
                string ReferredBy = Request.Form["ReferredBy"];
                string ReferredByID = Request.Form["ReferredByID"];
                string Condidatemblnumber = Request.Form["Condidatemblnumber"];
                string ReferredByEmail = Request.Form["ReferredByEmail"];

                HttpPostedFileBase friendResume = Request.Files["Resume"];
                string resumeName = "";
                string resumePath = "";

                if (friendResume != null)
                {
                    var ticketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                    string resumeFolderPath = Path.Combine(ticketingFolderPath, "Resume");

                    // Ensure the resume folder exists
                    if (!Directory.Exists(resumeFolderPath))
                    {
                        Directory.CreateDirectory(resumeFolderPath);
                    }

                    resumeName = Path.GetFileName(friendResume.FileName);
                    resumePath = Path.Combine(resumeFolderPath, resumeName);

                    // Save the resume file
                    friendResume.SaveAs(resumePath);
                }

                // Save referral details to the database
                var referral = new JobReferral
                {
                    JobID = JobID,
                    CandidateName = CandidateName,
                    ResumePath = resumeName,
                    ReferredBy = ReferredBy,
                    ReferredById = ReferredByID,
                    Condidatemblnumber = Condidatemblnumber,
                    ReferredByEmail = ReferredByEmail,
                    ReferredDate = DateTime.Now,
                    CandidateStatus = "Open"
                };

                _dbContext.JobReferrals.Add(referral);
                _dbContext.SaveChanges();

                // Render the email body from a partial view
                var emailBody = RenderPartialToString(this, "_JobReferralNotificationEmail", referral, ViewData, TempData);

                // Prepare the email request with attachment
                var emailRequest = new EmailRequest
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["JobReferalNotification"],
                    CCEmail = ConfigurationManager.AppSettings["JobReferalNotificationCC"],
                    Subject = $"Resume Referral: {referral.CandidateName}",
                    AttachmentPath = resumePath
                };

                // Send the email
                EMailHelper.SendEmail(emailRequest);

                // Update job item referrer count if it exists
                var jobItem = _dbContext.JobDetails.FirstOrDefault(x => x.JobID == JobID);
                if (jobItem != null)
                {
                    jobItem.TotalReferrers = (jobItem.TotalReferrers ?? 0) + 1;
                    _dbContext.SaveChanges();
                }

                return Json(new { success = true, message = "Referral submitted successfully!" });
            }

            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }


        public ActionResult Holidays()
        {
            var model = new HolidayModel();

            var cuserContext = SiteContext.GetCurrentUserContext();
            var employeeLocation = cuserContext.EmpInfo.Location;
            var holidayList = _dbContext.tblambcholidays.ToList();

            var filteredHolidays = holidayList
        .Where(x => x.region.Split(',').Select(r => r.Trim().ToLower()).Contains(employeeLocation.ToLower())) // Filter in-memory
        .OrderBy(x => x.holiday_date) // Sort holidays by date
        .ToList();

            model.Holidays = filteredHolidays;
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();

            return View("/Views/EmployeeDashboard/HolidaysListView.cshtml", model);
        }


        public JsonResult GetAttedenceSummary(string employeeId, string location, int month, int year)
        {
            var leaveService = new LeaveService(_dbContext);
            string jsonResult = leaveService.GetLeaveSummary(employeeId, location, month, year);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveNotificationReply()
        {
            try
            {
                // Parse the parameters from the request
                var sno = int.Parse(Request.Form["SNo"]);
                var replyComments = Request.Form["ReplyComments"];
                var replyFrom = Request.Form["ReplyFrom"];
                var replyTo = Request.Form["ReplyTo"];

                // Validate input
                if (string.IsNullOrWhiteSpace(replyComments))
                {
                    return Json(new { success = false, message = "Reply comments cannot be empty." });
                }

                var notification = _dbContext.Notifications.FirstOrDefault(n => n.RepiedSno == sno);
                if (notification == null)
                {
                    var oldNotificationInfo = _dbContext.Notifications.FirstOrDefault(n => n.SNo == sno);

                    var newNotification = new Notification
                    {
                        NotificationDate = DateTime.Now,
                        NotificationFromName = oldNotificationInfo.NotificationToName,
                        NotificationFromID = oldNotificationInfo.NotificationToID,
                        NotificationToName = oldNotificationInfo.NotificationFromName,
                        NotificationToID = oldNotificationInfo.NotificationFromID,
                        NotificationType = oldNotificationInfo.NotificationType,
                        Status = "Sent",
                        Comments = replyComments,
                        RepiedSno = sno,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.Notifications.Add(newNotification);
                    _dbContext.SaveChanges();

                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Notification not found." });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging library)
                // For example: _logger.LogError(ex, "An error occurred while saving notification reply.");

                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        public JsonResult UpdateRoles(string employeeId, string roles)
        {
            try
            {
                var empInfo = _dbContext.emp_info.Where(x => x.EmployeeID == employeeId).FirstOrDefault();
                if (empInfo != null)
                {
                    empInfo.Roles_Responsibilities = roles;
                    _dbContext.SaveChanges();

                    var siteContext = Session["SiteContext"] as SiteContextModel;

                    siteContext.EmpInfo = empInfo;

                }
                return Json(new { success = true, message = "Roles updated successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error when updating roles!" });
            }

        }

    }
}