﻿using HRMS.Models;
using HRMS.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Helpers;
using Newtonsoft.Json;

namespace HRMS.Controllers
{
    public class AdminDashboardController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AdminDashboardController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: AdminDashboard
        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Dashboard()
        {
            var model = new DashboardViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            return View("~/Views/AdminDashboard/Dashboard.cshtml", model);
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult EmployeeManagement()
        {
            var model = new EmployeeManagementViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            var employeesList = _dbContext.emp_info.ToList();
            model.EmpList = employeesList;

            var json = JsonConvert.SerializeObject(model.EmpList);
            model.EmpListJson = json;

            return PartialView("~/Views/AdminDashboard/EmpManagement.cshtml", model);
        }

        [HttpGet]
        public ActionResult AddEmployee()
        {
            var model = new EmployeeManagementViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            var employeesList = _dbContext.emp_info.ToList();
            model.EmpList = employeesList;

            var json = JsonConvert.SerializeObject(model.EmpList);
            model.EmpListJson = json;

            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddEmployee(emp_info formData)
        {
            var model = new AddEmployeeViewModel();
            try
            {
                model.EmpInfo = formData;

                _dbContext.emp_info.Add(formData);
                _dbContext.SaveChanges();

                model.JsonResponse.Message = "Employee details added successfully!";
                model.JsonResponse.StatusCode = 200;
            }
            catch (Exception ex)
            {
                if(ex.InnerException != null)
                {
                    model.JsonResponse.Message = ex.InnerException.Message;
                }
                else
                {
                    model.JsonResponse.Message = ex.Message;
                }
               
                model.JsonResponse.StatusCode = 500;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult AttendanceManagement()
        {
            //ViewBag.ActivePage = "AttendanceManagement";
            return PartialView("~/Views/AdminDashboard/AddEmpTabs.cshtml");
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult LeaveManagement()
        {
            ViewBag.ActivePage = "LeaveManagement";
            return View();
        }

        //[CustomAuthorize(Roles = "HR Admin")]
        public ActionResult Ticketing()
        {
            ViewBag.ActivePage = "Ticketing";
            return View();
        }
    }
}