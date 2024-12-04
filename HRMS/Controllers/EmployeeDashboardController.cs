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
                var employeeid = siteContext.EmpInfo.EmployeeID;

                model.empAsset = _dbContext.Assets.FirstOrDefault(x => x.AllocatedEmployeeID == employeeid);

                int currentYear = DateTime.Now.Year;

              
                model.empMaintanance = _dbContext.IT_Maintenance
                                           .Where(x => x.EmployeeID == employeeid && x.MaintenanceDate.Value.Year == currentYear)
                                           .ToList();

                model.EmpInfo = siteContext.EmpInfo;
                model.LoginInfo = siteContext.LoginInfo;

                model.Empbasedclients = _dbContext.EmployeeBasedClients.Where(x => x.EmployeeID == employeeid && x.EmployeeStatus=="Active").ToList();
                model.ITEmployees = _dbContext.emp_info.Where(x => x.Department.Contains("IT")).ToList();

                return View("~/Views/EmployeeDashboard/SelfService.cshtml", model);
            }

            return null;
        }

        public SelfServiceViewModel GetMaintenanceDataForYear(int year, string empID)
        {
            var model = new SelfServiceViewModel();
            // Assuming you have a DbContext (e.g., HRMS_EntityFramework)
            using (var context = new HRMS_EntityFramework())
            {
                // Query the database to get maintenance records for the given year

                if (string.IsNullOrWhiteSpace(empID))
                {
                    model.empMaintanance = context.IT_Maintenance
                      .Where(m => m.MaintenanceDate.HasValue && m.MaintenanceDate.Value.Year == year)
                      .ToList();
                }
                else
                {
                    model.empMaintanance = context.IT_Maintenance
                       .Where(m => m.MaintenanceDate.HasValue && m.MaintenanceDate.Value.Year == year && m.EmployeeID == empID)
                      .ToList();
                }
            }

            return model;
        }

        public ActionResult GetMaintenanceByYear(int year, string empID)
        {
            var model = new SelfServiceViewModel();
            model = GetMaintenanceDataForYear(year, empID);  // Fetch data based on the year
            return PartialView("_MaintenanceTableRows", model);
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
        public ActionResult MaintananceAck(int Sno)
        {
            // Retrieve the maintenance record based on Sno
            var maintenance = _dbContext.IT_Maintenance.FirstOrDefault(m => m.Sno == Sno);

            if (maintenance == null)
            {
                // If no matching maintenance record is found, return an error or handle it as needed
                return HttpNotFound();
            }

            // Pass the maintenance data to the view
            return View("/Views/EmployeeDashboard/EmpMaintananceAcknowledge.cshtml", maintenance);
        }

        [HttpPost]
        public ActionResult AcknowledgeMaintenance(string AcknowledgeDate, string ProblemCategory, string IssueFacing, string NewAssetRequirement, string Acknowledge, int Sno)
        {
            using (var context = new HRMS_EntityFramework())
            {
                // Find the maintenance record by Sno
                var maintenanceRecord = context.IT_Maintenance.FirstOrDefault(m => m.Sno == Sno);

                if (maintenanceRecord != null)
                {
                    if (maintenanceRecord.AcknowledgeDate != null && maintenanceRecord.AcknowledgeDate != DateTime.MinValue)
                    {
                        return Json(new { success = true, message = "You've already acknowledged!" });
                    }
                    // Update the fields with the provided values
                    maintenanceRecord.AcknowledgeDate = string.IsNullOrEmpty(AcknowledgeDate)
                        ? (DateTime?)null
                        : DateTime.Parse(AcknowledgeDate);
                    maintenanceRecord.ProblemCategory = ProblemCategory;
                    maintenanceRecord.IssueFacing = IssueFacing;
                    maintenanceRecord.NewAssetRequirement = NewAssetRequirement;
                    maintenanceRecord.Acknowledge = Acknowledge.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";

                    // Save changes to the database
                    context.SaveChanges();

                    return Json(new { success = true, message = "Maintenance successfully acknowledged!" }); // Return a success message
                }
                else
                {
                    return Json(new { success = false, message = "No record found." }); // Handle record not found
                }
            }
        }


        public JsonResult GetEmployeesByClient(string clientname)
        {
            var employees = _dbContext.EmployeeBasedClients.Where(x => x.Client == clientname).ToList();          
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

    }
}