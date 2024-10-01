using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class MaintananceController : Controller
    {
        // GET: Maintanance
        public ActionResult MaintananceInfo()
        {
            return View("~/Views/Itsupport/Maintanance.cshtml");
        }
        [HttpPost]
        //public JsonResult AddMaintenanceSchedule(Maintanance model)
        //{
        //    // Assuming your DbContext is HRMS_EntityFramework, already defined
        //    using (var db = new HRMS_EntityFramework())
        //    {
        //        // Map the incoming model data to your database entity (create a corresponding DB entity if necessary)
        //        //var schedule = new MaintananceModel
        //        //{
        //        //    Location = model.Location,
        //        //    Date = model.Date,
        //        //    Agent = model.Agent,
        //        //    TimeIn = model.TimeIn,
        //        //    TimeOut = model.TimeOut
        //        //};

        //        //// Add selected employees
        //        //foreach (var empId in model.SelectedEmployees)
        //        //{
        //        //    var employee = db.Employees.FirstOrDefault(e => e.EmployeeID == empId);
        //        //    if (employee != null)
        //        //    {
        //        //        // Add to a relation, like a table mapping employees to maintenance schedules
        //        //        db.MaintenanceEmployees.Add(new MaintenanceEmployee
        //        //        {
        //        //            EmployeeID = empId,
        //        //            MaintenanceScheduleID = schedule.Id // Assuming this is an auto-generated ID
        //        //        });
        //        //    }
        //        //}

        //        //// Add the schedule to the database
        //        //db.MaintenanceSchedules.Add(schedule);
        //        db.SaveChanges();
        //    }

        //    return Json(new { success = true, message = "Maintenance schedule added successfully" });
        //}

        public ActionResult EmpMaintananceHistory()
        {
            return View("~/Views/Itsupport/EmpMaintananceHistory.cshtml");
        }
        public ActionResult MaintananceApproval()
        {
            return View("~/Views/Itsupport/MaintananceApprovalView.cshtml");
        }
    }
}
