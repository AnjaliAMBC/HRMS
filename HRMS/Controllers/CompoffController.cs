using HRMS.Helpers;
using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class CompoffController : Controller
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