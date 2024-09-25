using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Admin;
using HRMS.Models.Employee;
using HRMS.Models.ITsupport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;
    public class VendorController : Controller
    {

        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public VendorController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }
        // GET: Vendor
        public ActionResult Index()
        {
            var model = ApproverVendorFromDB();
            return View("~/Views/Itsupport/VendorListView.cshtml", model);
        }

        [HttpGet]
        public ActionResult AddVendor(int vendorid = 0)
        {
            var model = new VendorViewModel();

            if (vendorid != 0)
            {
                model.EditVendor = _dbContext.VendorLists.Where(x => x.VedorID == vendorid).FirstOrDefault();
            }
            model.ITDeptEmployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();
            model.vendorTypes = _dbContext.VendorTypes.ToList();

            var lastVendorId = _dbContext.VendorLists.OrderByDescending(x => x.VedorID).Select(x => x.VedorID).FirstOrDefault();
            model.LasteVenorId = lastVendorId;
            model.NewVendorId = model.LasteVenorId + 1;

            return View("~/Views/Itsupport/VendorAdd.cshtml", model);
        }


        [HttpPost]
        public ActionResult AddVendor(VendorList vendor)
        {
            var model = new JsonResponse();
            try
            {
                var existingVendor = _dbContext.VendorLists.Find(vendor.VedorID);
                if (existingVendor == null)
                {
                    vendor.Status = "Pending";
                    _dbContext.VendorLists.Add(vendor);
                    model.Message = "Vendor details Added successfully!.";
                    model.StatusCode = 200;
                }
                else
                {
                    existingVendor.VendorName = vendor.VendorName;
                    existingVendor.VendorEmail = vendor.VendorEmail;
                    existingVendor.VendorContact = vendor.VendorContact;
                    existingVendor.VendorAddress = vendor.VendorAddress;
                    existingVendor.VendorType = vendor.VendorType;
                    existingVendor.VendorGST = vendor.VendorGST;
                    existingVendor.CreatedBy = vendor.CreatedBy;
                    existingVendor.CreatedDate = vendor.CreatedDate;
                    model.Message = "Vendor details updated successfully!.";
                    model.StatusCode = 200;
                }
                _dbContext.SaveChanges();

                // Generate email body using the partial view
                var emailBody = RenderPartialToString(this, "_VendorAddEmailNotification", vendor, ViewData, TempData);                

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                    CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],
                    Subject = "New Vendor Approval Request",
                };

                EMailHelper.SendEmail(emailRequest);


                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }


        //Vendor Approve
        public ActionResult ApproveVendorSuperAdmin(int vendorId = 0, string approvalReason = "")
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var vendor = _dbContext.VendorLists.Find(vendorId);

            if (vendor != null)
            {
                vendor.Status = "Approved";
                vendor.ApproveRejectReason = approvalReason;
                vendor.ApprovedBy = cuserContext.EmpInfo.EmployeeName;
                vendor.ApprovedDate = DateTime.Now;
                _dbContext.SaveChanges();

                // Prepare the email
                var emailSubject = "Vendor Approved";
                var emailBody = RenderPartialToString(this, "_VendorApprovedEmail", vendor, ViewData, TempData);

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                    CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],// Assuming you have VendorEmail in your model
                    Subject = emailSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);

                return Json(new { success = true, message = "Vendor approved successfully." });
            }

            return Json(new { success = false, message = "Vendor not found." });
        }


        public ActionResult RejectVendor(int vendorId, string approvalReason)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var vendor = _dbContext.VendorLists.Find(vendorId);

            if (vendor != null)
            {
                vendor.Status = "Rejected";
                vendor.ApproveRejectReason = approvalReason;
                vendor.RejectedBy = cuserContext.EmpInfo.EmployeeName;
                vendor.RejectedDate = DateTime.Now;
                _dbContext.SaveChanges();

                // Prepare the email
                var emailSubject = "Vendor Rejected";
                var emailBody = RenderPartialToString(this, "_VendorRejectedEmail", vendor, ViewData, TempData);

                var emailRequest = new EmailRequest()
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                    CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"], 
                    Subject = emailSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);

                return Json(new { success = true, message = "Vendor rejected successfully." });
            }

            return Json(new { success = false, message = "Vendor not found." });
        }

      
        public ActionResult ApproveVendor()
        {
            VendorViewModel model = ApproverVendorFromDB();
            return View("~/Views/Itsupport/VendorApprovalPage.cshtml", model);
        }

        public ActionResult ApproveVendorPartial()
        {
            VendorViewModel model = ApproverVendorFromDB();
            return PartialView("~/Views/Itsupport/_vendorapprovalpartial.cshtml", model);
        }

        private VendorViewModel ApproverVendorFromDB()
        {
            var model = new VendorViewModel();
            var vendors = _dbContext.VendorLists.OrderByDescending(x => x.VedorID).ToList();
            model.Allvendors = vendors;
            return model;
        }

        [HttpPost]
        public ActionResult AddVendorType(string VendorType)
        {
            var model = new JsonResponse();
            if (!string.IsNullOrEmpty(VendorType))
            {
                try
                {

                    var newVendortype = _dbContext.VendorTypes.Where(vt => vt.VendorType1 == VendorType).FirstOrDefault();
                    if (newVendortype == null)
                    {
                        var newVendor = new VendorType { VendorType1 = VendorType };
                        _dbContext.VendorTypes.Add(newVendor);
                        _dbContext.SaveChanges();

                        model.Message = "Leave Manager added successfully!";
                        model.StatusCode = 200;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        model.Message = "Leave Manager not added!";
                        model.StatusCode = 400;
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    model = ErrorHelper.CaptureError(ex);
                }
            }
            else
            {
                model.Message = "Vendor Type cannot be empty!";
                model.StatusCode = 400;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportSelectedVendors(List<int> selectedVendorIds)
        {
            var vendors = _dbContext.VendorLists
                            .Where(v => selectedVendorIds.Contains(v.VedorID))
                            .ToList();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Vendors");

                worksheet.Cells[1, 1].Value = "Vendor ID";
                worksheet.Cells[1, 2].Value = "Vendor Name";
                worksheet.Cells[1, 3].Value = "Vendor EmailID";
                worksheet.Cells[1, 4].Value = "Vendor Contact";
                worksheet.Cells[1, 5].Value = "Vendor Address";
                worksheet.Cells[1, 6].Value = "Vendor GST";
                worksheet.Cells[1, 7].Value = "Created Date";
                worksheet.Cells[1, 8].Value = "Created By";
                worksheet.Cells[1, 9].Value = "Status";

                using (var range = worksheet.Cells[1, 1, 1, 9]) // Range from (row 1, column 1) to (row 1, column 7)
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Font.Bold = true;
                }

                for (int i = 0; i < vendors.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = vendors[i].VedorID;
                    worksheet.Cells[i + 2, 2].Value = vendors[i].VendorName;
                    worksheet.Cells[i + 2, 3].Value = vendors[i].VendorEmail;
                    worksheet.Cells[i + 2, 4].Value = vendors[i].VendorContact;
                    worksheet.Cells[i + 2, 5].Value = vendors[i].VendorAddress;
                    worksheet.Cells[i + 2, 6].Value = vendors[i].VendorGST;
                    worksheet.Cells[i + 2, 7].Value = vendors[i].CreatedDate;
                    worksheet.Cells[i + 2, 8].Value = vendors[i].CreatedBy;
                    worksheet.Cells[i + 2, 9].Value = vendors[i].Status;
                }

                var stream = new MemoryStream(excelPackage.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SelectedVendors.xlsx");
            }
        }

        [HttpPost]
        public ActionResult ImportVendor(HttpPostedFileBase vendorExcelFile)
        {
            if (vendorExcelFile != null && vendorExcelFile.ContentLength > 0)
            {
                try
                {
                    
                    if (vendorExcelFile.FileName.EndsWith(".xlsx") || vendorExcelFile.FileName.EndsWith(".xls"))
                    {
                        var vendors = new List<VendorList>();

                        using (var package = new ExcelPackage(vendorExcelFile.InputStream))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                            
                            if (worksheet == null)
                            {
                                return Json(new { message = "Invalid Excel file. Please check the file and try again." });
                            }

                            
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                
                                string vendorName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                string vendorEmail = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                string vendorContact = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                string vendorAddress = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                string vendorGST = worksheet.Cells[row, 5].Value?.ToString().Trim();
                                string createdBy = worksheet.Cells[row, 6].Value?.ToString().Trim();
                                string createdDateString = worksheet.Cells[row, 7].Value?.ToString().Trim();

                                
                                DateTime createdDate;
                                if (!DateTime.TryParse(createdDateString, out createdDate))
                                {
                                    createdDate = DateTime.Now; // Use current date if parsing fails
                                }
                                                               
                                if (string.IsNullOrWhiteSpace(vendorName) || string.IsNullOrWhiteSpace(vendorEmail) ||
                                    string.IsNullOrWhiteSpace(vendorContact) || string.IsNullOrWhiteSpace(vendorGST))
                                {
                                    continue; // Skip this row if any required field is missing
                                }

                                // Add the vendor to the list
                                var vendor = new VendorList
                                {
                                    VendorName = vendorName,
                                    VendorEmail = vendorEmail,
                                    VendorContact = vendorContact,
                                    VendorAddress = vendorAddress,
                                    VendorGST = vendorGST,
                                    CreatedBy = createdBy,
                                    CreatedDate = createdDate
                                };

                                vendors.Add(vendor);
                            }

                            if (vendors.Count > 0)
                            {
                                // Save to the database
                                _dbContext.VendorLists.AddRange(vendors);
                                _dbContext.SaveChanges();

                                return Json(new { message = "Vendors imported successfully!", success = true });
                            }
                            else
                            {
                                return Json(new { message = "No valid vendors found in the file. Please check the file and try again.", success = false });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { message = "Invalid file format. Please upload an Excel file.", success = false });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { message = "Error occurred while importing vendors: " + ex.Message, success = false });
                }
            }
            else
            {
                return Json(new { message = "Please upload a file.", success = false });
            }
        }

    }
}