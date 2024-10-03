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
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public MaintananceController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: Maintanance
        public ActionResult MaintananceInfo()
        {
            var model = new MaintananceModel();
            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            return View("~/Views/Itsupport/Maintanance.cshtml", model);
        }

        public JsonResult GetEmpBasedOnLocation(string Location)
        {
            var emmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Location == Location).ToList();
            return Json(emmployees, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]*/
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
