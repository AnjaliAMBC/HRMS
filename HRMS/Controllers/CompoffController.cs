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

        public JsonResult SubmitCompOff(int employeeID, DateTime compOffDate, string reason)
        {
            try
            {
                //// Logic to save the comp off information to the database
                //using (var context = new YourDbContext())
                //{
                //    var compOff = new CompOff
                //    {
                //        EmployeeID = employeeID,
                //        CampOffDate = compOffDate,
                //        Reason = reason,
                //        // Add other necessary fields
                //    };

                //    context.CompOffs.Add(compOff);
                //    context.SaveChanges();
                //}

                return Json(new { success = true, message = "Comp Off successfully submitted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred while submitting Comp Off: " + ex.Message });
            }
        }
    }
}