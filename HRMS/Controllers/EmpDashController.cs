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
    using Helpers;
    using HRMS.Models;
    using HRMS.Services;
    using System.Data.Entity;
    using System.Globalization;

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

            model.AnniversaryModel = new EmployeeEventHelper().Anniversary();
            model.Birthdays = new EmployeeEventHelper().Birthday();
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


            model.NewJoiners = _dbContext.emp_info.Where(x => x.DOJ == DateTime.Today).ToList();


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

        public ActionResult Policies()
        {
            return View("/Views/EmployeeDashboard/PoliciesView.cshtml");
        }
        public ActionResult Holidays()
        {
            return View("/Views/EmployeeDashboard/HolidaysListView.cshtml");
        }


        public JsonResult GetAttedenceSummary(string employeeId, string location, int month, int year)
        {
            var leaveService = new LeaveService(_dbContext);
            string jsonResult = leaveService.GetLeaveSummary(employeeId, location, month, year);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
    }
}