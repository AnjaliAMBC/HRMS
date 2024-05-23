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
    public class EmpDashController : Controller
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
            model.CheckInInfo = cuserContext.CheckInInfo;
            model.AnniversaryModel = Anniversary();
            model.BirthModel = Birthday();
            model.UpcomingHolidays = upcomingHolidays();
            return PartialView("~/Views/EmployeeDashboard/EmpDash.cshtml", model);
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
                    checkInModel.Employee_IP = "123.344";

                    var newCheckInItem = _dbContext.tbld_ambclogininformation.Add(checkInModel);
                    _dbContext.SaveChanges();

                    cuserContext.CheckInInfo = newCheckInItem;
                    Session["SiteContext"] = cuserContext;

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

                        model.CheckInInfo = checkInRecord;
                        model.JsonResponse.StatusCode = 200;
                        model.JsonResponse.Message = "Check-Out is successful";
                    }

                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
                model.JsonResponse.StatusCode = 500;
                model.JsonResponse.Message = "Error while Check-Out";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
        public List<AnniversaryModel> Anniversary()
        {
            var today = DateTime.Today;
            var anniversaries = _dbContext.emp_info.Where(e => e.DOJ.Month == today.Month && e.DOJ.Day == today.Day).Select(e => new AnniversaryModel
            {
                EmpName = e.EmployeeName,
                Designation = e.Designation,
                imagePath = e.imagepath,
                years = today.Year - e.DOJ.Year,
                EmpEmail = e.OfficalEmailid,
            }).ToList();

            return anniversaries;
        }
        public List<BirthdayModel> Birthday()
        {
            var today = DateTime.Today;
            var birthdays = _dbContext.emp_info.Where(e => e.DOB.Month == today.Month && e.DOB.Day == today.Day)
                .Select(e => new BirthdayModel
                {
                    EmpName = e.EmployeeName,
                    imagePath = e.imagepath,
                    Designation = e.Designation,
                    EmpEmail = e.OfficalEmailid,
                })
                .ToList();

            return birthdays;
        }
        public List<UpcomingHoliday> upcomingHolidays()
        {
            // Fetch the upcoming holidays from the database
            var today = DateTime.Today;

            // Fetch the upcoming holidays from the database
            var upcomingHolidays = _dbContext.tblambcholidays
                .Where(h => h.holiday_date >= today)
                .OrderBy(h => h.holiday_date)
                .Select(h => new UpcomingHoliday
                {
                    HolidayNo = h.holidayno,
                    HolidayDate = (DateTime)h.holiday_date,
                    HolidayName = h.holiday_name,
                    Region = h.region
                })
                .ToList();

            return upcomingHolidays;
        }

        //public ActionResult SendAnniversaryWishes(EmailRequest request)
        //{
        //    var emailModel = EMailHelper.SendEmail(request);
        //    return Json(emailModel);
        //}

        //public ActionResult SendBirthDayWishes(EmailRequest request)
        //{
        //    var emailModel = EMailHelper.SendEmail(request);
        //    return Json(emailModel);
        //}
    }
}