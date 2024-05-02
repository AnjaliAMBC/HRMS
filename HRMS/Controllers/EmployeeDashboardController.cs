using HRMS.Helpers;
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

    public class EmployeeDashboardController : Controller
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
            return View("~/Views/EmployeeDashboard/Dashboard.cshtml");
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
                    var uploadsFolderPath = ConfigurationManager.AppSettings["WebRootFolder"] + ConfigurationManager.AppSettings["EmpImagesFolder"];

                    if (!Directory.Exists(uploadsFolderPath))
                        Directory.CreateDirectory(uploadsFolderPath);

                    var deleteExistignImage = ImageHelper.DoesImageExistForEmployee(cuserContext.LoginInfo.EmployeeID, uploadsFolderPath);

                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = file.ContentType.Split('/')[1];
                    var updatedFileName = cuserContext.LoginInfo.EmployeeID + "." + fileExtension;
                    var filePath = Path.Combine(uploadsFolderPath, updatedFileName);

                    file.SaveAs(filePath);

                    var employee = _dbContext.emp_info.FirstOrDefault(e => e.EmployeeID == cuserContext.LoginInfo.EmployeeID);

                    if (employee != null)
                    {
                        // Update the properties of the retrieved employee entity
                        employee.imagepath = updatedFileName;
                        _dbContext.SaveChanges();
                        model.ImageURl = ConfigurationManager.AppSettings["EmpImagesFolder"] + "/" + updatedFileName;
                        model.JsonResponse.Message = "Image Uploaded successfully!";
                        model.JsonResponse.StatusCode = 200;

                        //Update the image in session object as well
                        cuserContext.EmpInfo.Reason = updatedFileName;
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