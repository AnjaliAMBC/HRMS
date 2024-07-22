using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;

    public class CompoffController : BaseController
    {

        private readonly HRMS_EntityFramework _dbContext;
        public CompoffController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public JsonResult SubmitCompOff(string employeeID, string empName, DateTime compOffDate, string reason, string submittedUser, string holidayname, int holidynumber, string holidaylocation, string empEmail)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var compOff = new Compoff
                {
                    EmployeeID = employeeID,
                    CampOffDate = compOffDate,
                    addStatus = "Pending",
                    createdby = submittedUser,
                    updatedby = submittedUser,
                    createddate = DateTime.Today,
                    EmployeeName = empName,
                    HolidayName = holidayname,
                    updateddate = DateTime.Today,
                    concatinatestring = employeeID + "_" + compOffDate.ToString(),
                    Reason = reason,
                    Holidayno = holidynumber,
                    Location = holidaylocation,
                    OfficalEmailid = empEmail
                };

                _dbContext.Compoffs.Add(compOff);
                _dbContext.SaveChanges();

                jsonResponse.Message = "Comp Off successfully submitted.";
                jsonResponse.StatusCode = 200;

                //When employee apply compoff request
                if (empName == submittedUser)
                {
                    var emailSubject = "Working status on " + holidayname + " - " + System.Convert.ToDateTime(compOffDate).ToString("dd MMMM yyyy") ;
                    var emailBody = RenderPartialToString(this, "_compoffNotificationempemail", compOff, ViewData, TempData);

                    var emailRequest = new EmailRequest()
                    {
                        Body = emailBody,
                        ToEmail = empEmail,
                        CCEmail = ConfigurationManager.AppSettings["LeaveEmails"],
                        Subject = emailSubject
                    };

                    var sendNotification = EMailHelper.SendEmail(emailRequest);
                }
                //In case admin submit the leave on employee behalf
                

                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonResponse = ErrorHelper.CaptureError(ex);
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }
    }
}