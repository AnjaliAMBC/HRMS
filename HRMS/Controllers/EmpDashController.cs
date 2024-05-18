﻿using HRMS.Helpers;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
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
    }
}