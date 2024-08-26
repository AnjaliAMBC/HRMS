using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Admin;
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
            var model = new VendorViewModel();
            var vendors = _dbContext.VendorLists.ToList();
            model.Allvendors = vendors;
       
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

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private void NewVendorAddEmail(VendorList vendor)
        {
            try
            {
                var emailBody = PartialViewHelper.RenderPartialToString(this, "_newVendorCreate", vendor, ViewData, TempData);
                var emailSubject = "New Vendor Created";

                var emailRequest = new VendorEmailRequest
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                    CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],
                    Subject = emailSubject
                };

                // Implement the actual email sending logic here
                //EmailHelper.SendEmail(emailRequest);
            }
            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
                // Optionally log the error or handle it
            }
        }
        public ActionResult ImportVendor()
        {
            return View("~/Views/Itsupport/VendorImport.cshtml");
        }
        public ActionResult ApproveVendor()
        {
            var model = new VendorViewModel();
            var vendors = _dbContext.VendorLists.ToList();
            model.Allvendors = vendors;

            return View("~/Views/Itsupport/VendorApprovalPage.cshtml", model);
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
        public JsonResult ImportVendors()
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Invalid file format." });
                        }

                        // Assuming that the first row contains headers
                        var startRow = 2; // Data starts from the second row
                        for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                        {

                            int vendorId;
                            bool isParsed = int.TryParse(worksheet.Cells[row, 1].Text, out vendorId);

                            if (!isParsed)
                            {
                                // Handle the case where the value is not a valid integer
                                // For example, log an error or skip the row
                                continue;
                            }                            
                            var vendorName = worksheet.Cells[row, 2].Text;
                            var vendorEmail = worksheet.Cells[row, 3].Text;
                            var vendorContact = worksheet.Cells[row, 4].Text;
                            var vendorAddress = worksheet.Cells[row, 5].Text;
                            var vendorGst = worksheet.Cells[row, 6].Text;
                            var createdDate = DateTime.Parse(worksheet.Cells[row, 7].Text);
                            var createdBy = worksheet.Cells[row, 8].Text;                            

                            // Add your code to save each row data to the database
                            // Example:
                            VendorList vendor = new VendorList
                            {
                                VedorID = vendorId,
                                VendorName = vendorName,
                                VendorEmail = vendorEmail,
                                VendorContact = vendorContact,
                                VendorAddress = vendorAddress,
                                VendorGST=vendorGst,
                                CreatedDate = createdDate,
                                CreatedBy = createdBy,
                             };

                            // Save the vendor to the database
                            _dbContext.VendorLists.Add(vendor); // Assuming _context is your database context
                        }

                        _dbContext.SaveChanges();
                    }

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No file uploaded." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}