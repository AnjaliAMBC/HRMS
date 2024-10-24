﻿using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using Helpers;
    using System.Configuration;

    public class EmployeeDashboardController : BaseController
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmployeeDashboardController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }
        public ActionResult Dashboard()
        {
            var model = new EmpDashBoardModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;
            return View("~/Views/EmployeeDashboard/Dashboard.cshtml", model);
        }

        public ActionResult SelfService()
        {
            var model = new SelfServiceViewModel();
            if (Session["SiteContext"] != null)
            {
                var siteContext = Session["SiteContext"] as SiteContextModel;
                model.EmpInfo = siteContext.EmpInfo;
                model.LoginInfo = siteContext.LoginInfo;
                return View("~/Views/EmployeeDashboard/SelfService.cshtml", model);
            }
            return null;
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            var model = new SelfServiceEmpImageModel();
            try
            {
                var cuserContext = SiteContext.GetCurrentUserContext();

                if (file != null && file.ContentLength > 0)
                {
                    //var uploadsFolderPath = Path.Combine(
                    //    ConfigurationManager.AppSettings["WebRootFolder"],
                    //    ConfigurationManager.AppSettings["EmpImagesFolder"]
                    //);

                    var uploadsFolderPath = ConfigurationManager.AppSettings["EmpImagesFolderUpload"];

                    if (!Directory.Exists(uploadsFolderPath))
                        Directory.CreateDirectory(uploadsFolderPath);

                    // Construct the old file name and path
                    var oldFileNamePattern = $"{cuserContext.LoginInfo.EmployeeID}.*";
                    var oldFilePath = Directory.GetFiles(uploadsFolderPath, oldFileNamePattern).FirstOrDefault();

                    // Delete the existing image if it exists
                    if (!string.IsNullOrEmpty(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                            Console.WriteLine($"Deleted old file: {oldFilePath}");
                        }
                        catch (IOException ex)
                        {
                            // Handle file deletion errors
                            Console.WriteLine($"Failed to delete file: {oldFilePath}. Error: {ex.Message}");
                            model.JsonResponse.Message = "Failed to delete the existing image.";
                            model.JsonResponse.StatusCode = 500;
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }

                    // Construct the new file path
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = file.ContentType.Split('/')[1];
                    var updatedFileName = $"{cuserContext.LoginInfo.EmployeeID}.{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, updatedFileName);

                    // Save the file using stream
                    using (var fileStream = file.InputStream)
                    {
                        using (var outputStream = System.IO.File.Create(filePath))
                        {
                            fileStream.CopyTo(outputStream);
                        }
                    }

                    // Update employee record with new image path
                    var employee = _dbContext.emp_info.FirstOrDefault(e => e.EmployeeID == cuserContext.LoginInfo.EmployeeID);

                    if (employee != null)
                    {
                        employee.imagepath = updatedFileName;
                        _dbContext.SaveChanges();
                        model.ImageURl = ConfigurationManager.AppSettings["EmpImagesFolder"] + "/" + $"{cuserContext.LoginInfo.EmployeeID}.{fileExtension}";
                        model.JsonResponse.Message = "Image Uploaded successfully!";
                        model.JsonResponse.StatusCode = 200;

                        // Update the image in session object as well
                        cuserContext.EmpInfo.imagepath = updatedFileName;

                        Session["SiteContext"] = cuserContext;
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model.JsonResponse = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

    }
}