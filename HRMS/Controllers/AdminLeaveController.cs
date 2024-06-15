﻿using HRMS.Helpers;
using HRMS.Models.Admin;
using HRMS.Models.Employee;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AdminLeaveController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;
        public AdminLeaveController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public ActionResult Index(string selectedStartDate, string selectedEndDate)
        {
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var model = new AdminLeaveManagementModel();
            GetLeavesInfoBasedonStartandEndDate(startOfMonth.ToString(), endOfMonth.ToString(), model, "");
            return View("~/Views/AdminDashboard/AdminLeaveTracker.cshtml", model);
        }
        public ActionResult AdminLeaveBalance()
        {
            LeaveTypesBasedOnEmpViewModel empLeaveTypes = new LeaveCalculator().GetLeavesByEmp("");
            return PartialView("~/Views/AdminDashboard/AdminEmpBalanceView.cshtml", empLeaveTypes);
        }
        public ActionResult AdminLeaveApply()
        {
            return PartialView("~/Views/EmployeeDashboard/EmpApplyleave.cshtml");
        }

        public ActionResult AdminCalenderLeaveManagement(int month, int year, string empID)
        {
            DateTime startDate, endDate;

            GetMonthStartAndEndDates(month, year, out startDate, out endDate);

            var model = new AdminLeaveManagementModel();
            GetLeavesInfoBasedonStartandEndDate(startDate.ToString(), "", model, empID);
            var json = JsonConvert.SerializeObject(model.LeavesInfo);
            return Json(json.Replace(";", ""), JsonRequestBehavior.AllowGet);
        }

        public static void GetMonthStartAndEndDates(int month, int year, out DateTime startDate, out DateTime endDate)
        {
            // Get the first day of the month

            if (month == 0)
            {
                month = 1;
            }
            startDate = new DateTime(year, month, 1);

            // Get the last day of the month
            endDate = startDate.AddMonths(1).AddDays(-1);

        }

        public ActionResult AdminLeaveManagement(string selectedStartDate, string SelectedEndDate)
        {
            var model = new AdminLeaveManagementModel();

            if (string.IsNullOrWhiteSpace(SelectedEndDate))
            {
                SelectedEndDate = selectedStartDate;
            }
            GetLeavesInfoBasedonStartandEndDate(selectedStartDate, SelectedEndDate, model, "");
            return PartialView("~/Views/AdminDashboard/AdminLeaveEmpManage.cshtml", model);
        }

        private void GetLeavesInfoBasedonStartandEndDate(string selectedStartDate, string SelectedEndDate, AdminLeaveManagementModel model, string empID)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedDate = DateTime.Today;
            model.SelectedEndDate = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(selectedStartDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedDate = DateTime.Parse(selectedStartDate);
            }

            if (!string.IsNullOrWhiteSpace(SelectedEndDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedEndDate = DateTime.ParseExact(SelectedEndDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedEndDate = DateTime.Parse(SelectedEndDate);
            }

            var selectedDateLeaves = new List<con_leaveupdate>();

            if (string.IsNullOrWhiteSpace(empID))
            {
                selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate).ToList();
            }
            else
            {
                selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate && x.employee_id == empID).ToList();
            }


            model.LeavesInfo = selectedDateLeaves.OrderBy(x => x.leavedate).ToList();
        }

        public ActionResult AdminLeaveBalanceUpdate()
        {
            var model = new AdminLeaveBalanceUpdateModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            model.Employees = _dbContext.emp_info.ToList();

            return PartialView("~/Views/AdminDashboard/AdminLeaveBalanceUpdate.cshtml", model);
        }

        public ActionResult GetHolidaysBasedonLocation(string empid, int month, int year)
        {
            var selectedEmp = _dbContext.emp_info.Where(x => x.EmployeeID == empid).FirstOrDefault();
            DateTime yearstartDate, yearendDate;
            GetMonthStartAndEndDates(month + 1, year, out yearstartDate, out yearendDate);
            DateTime currentDate = DateTime.Today;
            if (selectedEmp != null)
            {
                var holidayList = _dbContext.tblambcholidays.Where(x => x.region == selectedEmp.Location && x.holiday_date >= yearstartDate && x.holiday_date <= yearendDate);
                var json = JsonConvert.SerializeObject(holidayList);
                return Json(json.Replace(";", ""), JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
