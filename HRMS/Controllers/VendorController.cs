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
using System.Data;
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
        [HttpGet]
        public ActionResult ImportVendor()
        {           
            return View("~/Views/Itsupport/VendorImport.cshtml");
        }

        [HttpPost]
        public JsonResult ImportVendors(HttpPostedFileBase file)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var model = new JsonResponse();

            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    List<VendorList> vendorList = new List<VendorList>();

                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        DataTable dt = new DataTable();

                        // Get column headers from the first row
                        int colCount = worksheet.Dimension.End.Column;
                        for (int col = 1; col <= colCount; col++)
                        {
                            dt.Columns.Add(worksheet.Cells[1, col].Value.ToString());
                        }

                        // Loop through each row starting from the second row
                        int rowCount = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            bool isEmptyRow = true;
                            for (int col = 1; col <= colCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value != null)
                                {
                                    isEmptyRow = false;
                                    break;
                                }
                            }

                            if (isEmptyRow)
                            {
                                continue;
                            }

                            DataRow dataRow = dt.NewRow();
                            for (int col = 1; col <= colCount; col++)
                            {
                                dataRow[col - 1] = worksheet.Cells[row, col].Value;
                            }

                            dt.Rows.Add(dataRow);

                            // Map DataRow to Asset object
                            VendorList vendor = new VendorList
                            {
                                VendorName = dataRow["VendorName"].ToString(),
                                VendorEmail = dataRow["VendorEmail"].ToString(),
                                VendorContact = dataRow["VendorContact"].ToString(),
                                VendorAddress = dataRow["VendorAddress"].ToString(),
                                VendorType = dataRow["VendorType"].ToString(),
                                VendorGST = dataRow["VendorGST"].ToString(),
                                ApprovedBy = dataRow["ApprovedBy"].ToString(),
                                ApprovedDate = string.IsNullOrEmpty(dataRow["ApprovedDate"].ToString())
                       ? (DateTime?)null
                       : Convert.ToDateTime(dataRow["ApprovedDate"]),
                                RejectedBy = dataRow["RejectedBy"].ToString(),
                                RejectedDate = string.IsNullOrEmpty(dataRow["RejectedDate"].ToString())
                       ? (DateTime?)null
                       : Convert.ToDateTime(dataRow["RejectedDate"]),
                                Status = dataRow["Status"].ToString(),
                                ApproveRejectReason = dataRow["ApproveRejectReason"].ToString(),
                                CreatedBy = cuserContext.EmpInfo.EmployeeName,
                                CreatedDate = DateTime.Now
                            };

                            vendorList.Add(vendor);
                        }
                    }

                    if (vendorList.Count > 0)
                    {
                        using (var context = new HRMS_EntityFramework())
                        {
                            context.VendorLists.AddRange(vendorList);
                            context.SaveChanges();

                            model.Message = "Vendors imported successfully";
                            model.StatusCode = 200;
                        }
                    }
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

    }
}