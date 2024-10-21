using HRMS.Helpers;
using HRMS.Models;
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
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;
    public class PurchaseController : Controller
    {

        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public PurchaseController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: Purchase
        public ActionResult Index()
        {
            var model = PurchaseListView();
            return View("~/Views/Itsupport/PurchaseListView.cshtml", model);
        }

        private PurchaseListViewModel PurchaseListView()
        {
            var model = new PurchaseListViewModel();
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.PurchaseRequests = _dbContext.PurchaseRequests.OrderByDescending(x => x.PurchaseRequestID).ToList();
            model.PurchasRequestFolderPath = "/purchase";
            model.ContextModel = cuserContext;
            return model;
        }

        [HttpGet]
        public ActionResult AddPurchaseRequest(int purchaseId = 0)
        {
            var model = new PurchaseViewModel();

            if (purchaseId != 0)
            {
                model.EditPurchase = _dbContext.PurchaseRequests
                                                      .FirstOrDefault(x => x.PurchaseRequestID == purchaseId);
            }

            model.Assettypes = _dbContext.VendorTypes.ToList();
            model.allVendors = _dbContext.VendorLists.Where(x => x.Status == "Approved").ToList();
            model.ITDeptEmployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();
            var lastPurchaseRequestId = _dbContext.PurchaseRequests
                                                   .OrderByDescending(x => x.PurchaseRequestID)
                                                   .FirstOrDefault();

            if (lastPurchaseRequestId != null)
            {
                model.LastePurchaseId = lastPurchaseRequestId.PRNumber;
                model.NewPurchaseId = "PR-" + (lastPurchaseRequestId.PurchaseRequestID + 1);
            }
            else
            {
                model.LastePurchaseId = "NA";
                model.NewPurchaseId = "PR-1";
            }

            return View("~/Views/Itsupport/PurchaseAdd.cshtml", model);
        }

        [HttpPost]
        public JsonResult AddPurchaseRequest()
        {
            var model = new JsonResponse();
            var cuserContext = SiteContext.GetCurrentUserContext();
            try
            {

                string PRNumber = Request.Form["PRNumber"];
                string assetType = Request.Form["AssetType"];
                string requiredOn = Request.Form["RequiredOn"];
                string requestedBy = Request.Form["Requestedby"];
                string vendorName1 = Request.Form["Vendorname1"];

                string[] vendor1Parts = vendorName1.Split('-');
                string vendor1idlastpart = vendor1Parts[vendor1Parts.Length - 1].Trim();
                int vendor1ID = int.Parse(vendor1idlastpart);

                var vendor1 = _dbContext.VendorLists.Where(x => x.VedorID == vendor1ID).FirstOrDefault();

                string vendor1Name = "";
                int vendorID1 = 0;
                string vendor1Emails = "";
                if (vendor1 != null)
                {
                    vendor1Name = vendor1.VendorName;
                    vendorID1 = vendor1.VedorID;
                    vendor1Emails = vendor1.VendorEmail;
                }


                string vendor1Quotation = Request.Form["Vendor1quotation"]?.Trim();
                string vendorName2 = Request.Form["Vendorname2"];
                string vendor2idlastpart = "0";
                if (vendorName2 != "Select Vendor Name")
                {
                    string[] vendor2Parts = vendorName2.Split('-');
                    vendor2idlastpart = vendor2Parts[vendor2Parts.Length - 1].Trim();
                }
                int vendor2ID = int.Parse(vendor2idlastpart);

                var vendor2 = _dbContext.VendorLists.Where(x => x.VedorID == vendor2ID).FirstOrDefault();

                string vendor2Name = "";
                int vendorID2 = 0;
                string vendor2Emails = "";
                if (vendor2 != null)
                {
                    vendor2Name = vendor2.VendorName;
                    vendorID2 = vendor2.VedorID;
                    vendor2Emails = vendor2.VendorEmail;
                }


                string vendor2Quotation = !string.IsNullOrWhiteSpace(Request.Form["Vendor2quotation"]) ?
                           Request.Form["Vendor2quotation"].Trim() : "0";

                string vendorName3 = Request.Form["Vendorname3"];
                string vendor3idlastpart = "0";
                if (vendorName3 != "Select Vendor Name")
                {
                    string[] vendor3Parts = vendorName3.Split('-');
                    vendor3idlastpart = vendor3Parts[vendor3Parts.Length - 1].Trim();
                }

                int vendor3ID = int.Parse(vendor3idlastpart);

                var vendor3 = _dbContext.VendorLists.Where(x => x.VedorID == vendor3ID).FirstOrDefault();

                string vendor3Name = "";
                int vendorID3 = 0;
                string vendor3Emails = "";
                if (vendor3 != null)
                {
                    vendor3Name = vendor3.VendorName;
                    vendorID3 = vendor3.VedorID;
                    vendor3Emails = vendor3.VendorEmail;
                }

                string vendor3Quotation = !string.IsNullOrWhiteSpace(Request.Form["Vendor3quotation"]) ?
                           Request.Form["Vendor3quotation"].Trim() : "0";


                HttpPostedFileBase fileInput1 = Request.Files["AttachFile-1"];
                HttpPostedFileBase fileInput2 = Request.Files["AttachFile-2"];
                HttpPostedFileBase fileInput3 = Request.Files["AttachFile-3"];

                var TicketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                TicketingFolderPath = TicketingFolderPath + "/Purchase";

                string fileInput1Path = "";
                string fileInput1Name = "";
                if (fileInput1 != null)
                {
                    fileInput1Name = Path.GetFileName(fileInput1.FileName);
                }

                string fileInput2Path = "";
                string fileInput2Name = "";
                if (fileInput2 != null)
                {
                    fileInput2Name = Path.GetFileName(fileInput2.FileName);
                }

                string fileInput3Path = "";
                string fileInput3Name = "";
                if (fileInput3 != null)
                {
                    fileInput3Name = Path.GetFileName(fileInput3.FileName);
                }

                using (var context = new HRMS_EntityFramework())
                {
                    var purchaseRequest = context.PurchaseRequests.FirstOrDefault(pr => pr.PRNumber == PRNumber);

                    if (purchaseRequest == null)
                    {
                        // Create new record
                        purchaseRequest = new PurchaseRequest
                        {
                            AssetType = assetType,
                            RequestedBy = requestedBy,
                            RequiredOn = DateTime.Parse(requiredOn),
                            VendorName1 = vendor1Name,
                            VendorEmail1 = vendor1Emails,
                            VendorID1 = vendorID1,
                            QuotationPrice1 = vendor1Quotation,
                            AttachFile1 = fileInput1Name,
                            VendorName2 = vendor2Name,
                            VendorEmail2 = vendor2Emails,
                            VendorID2 = vendorID2,
                            QuotationPrice2 = vendor2Quotation,
                            AttachFile2 = fileInput2Name,
                            VendorName3 = vendor3Name,
                            VendorEmail3 = vendor3Emails,
                            VendorID3 = vendorID3,
                            QuotationPrice3 = vendor3Quotation,
                            AttachFile3 = fileInput3Name,
                            Status = "Pending",
                            CreatedBy = cuserContext.LoginInfo.EmployeeName,
                            CreatedDate = DateTime.Now,
                            PRNumber = PRNumber,
                            FinalStatus = "Pending"
                        };

                        context.PurchaseRequests.Add(purchaseRequest);
                    }
                    else
                    {
                        purchaseRequest.AssetType = assetType;
                        purchaseRequest.RequestedBy = requestedBy;
                        purchaseRequest.RequiredOn = DateTime.Parse(requiredOn);
                        purchaseRequest.VendorName1 = vendor1Name;
                        purchaseRequest.VendorEmail1 = vendor1Emails;
                        purchaseRequest.VendorID1 = vendorID1;
                        purchaseRequest.QuotationPrice1 = vendor1Quotation;
                        purchaseRequest.VendorName2 = vendor2Name;
                        purchaseRequest.VendorEmail2 = vendor2Emails;
                        purchaseRequest.VendorID2 = vendorID2;
                        purchaseRequest.QuotationPrice2 = vendor2Quotation;
                        purchaseRequest.VendorName3 = vendor3Name;
                        purchaseRequest.VendorEmail3 = vendor3Emails;
                        purchaseRequest.VendorID3 = vendorID3;
                        purchaseRequest.QuotationPrice3 = vendor3Quotation;
                        purchaseRequest.UpdatedBy = cuserContext.LoginInfo.EmployeeName;
                        purchaseRequest.UpdatedDate = DateTime.Now;
                        purchaseRequest.FinalStatus = "Pending";
                        if (fileInput1 != null)
                        {
                            purchaseRequest.AttachFile1 = fileInput1Name;
                        }
                        if (fileInput2 != null)
                        {
                            purchaseRequest.AttachFile2 = fileInput2Name;
                        }
                        if (fileInput3 != null)
                        {
                            purchaseRequest.AttachFile3 = fileInput3Name;
                        }
                    }

                    if (fileInput1 != null)
                    {
                        fileInput1Path = Path.Combine(TicketingFolderPath, fileInput1Name);
                        fileInput1.SaveAs(fileInput1Path);
                    }

                    if (fileInput2 != null)
                    {
                        fileInput2Path = Path.Combine(TicketingFolderPath, fileInput2Name);
                        fileInput2.SaveAs(fileInput2Path);
                    }

                    if (fileInput3 != null)
                    {
                        fileInput3Path = Path.Combine(TicketingFolderPath, fileInput3Name);
                        fileInput3.SaveAs(fileInput3Path);
                    }

                    context.SaveChanges();

                    model.StatusCode = 200;
                    model.Message = "Purchase request created successfully!";

                    //var emailBody = RenderPartialToString(this, "_PurchaseAddEmailNotification", purchaseRequest, ViewData, TempData);

                    //var emailRequest = new EmailRequest()
                    //{
                    //    Body = emailBody,
                    //    ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                    //    CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],
                    //    Subject = "New Purchase Request Created",
                    //};

                    //EMailHelper.SendEmail(emailRequest);

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
                return Json(model, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult PurchaseSubmitRequest()
        {
            var model = new JsonResponse();
            try
            {
                int PurchaseID = System.Convert.ToInt32(Request.Form["PurchaseID"]);
                HttpPostedFileBase fileInput1 = Request.Files["PAPurchaseOrder"];
                HttpPostedFileBase fileInput2 = Request.Files["PATaxInvoice"];
                var cuserContext = SiteContext.GetCurrentUserContext();

                using (var context = new HRMS_EntityFramework())
                {
                    var purchaseRequest = context.PurchaseRequests.FirstOrDefault(pr => pr.PurchaseRequestID == PurchaseID);
                    if (purchaseRequest != null)
                    {
                        var TicketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                        TicketingFolderPath = TicketingFolderPath + "/Purchase";

                        string fileInput1Path = "";
                        string fileInput1Name = "";
                        if (fileInput1 != null)
                        {
                            fileInput1Name = Path.GetFileName(fileInput1.FileName);
                            purchaseRequest.PO = fileInput1Name;
                        }

                        string fileInput2Name = "";
                        if (fileInput2 != null)
                        {
                            fileInput2Name = Path.GetFileName(fileInput2.FileName);
                            purchaseRequest.TaxInvoice = fileInput2Name;
                        }


                        if (fileInput1 != null)
                        {
                            fileInput1Path = Path.Combine(TicketingFolderPath, fileInput1Name);
                            fileInput1.SaveAs(fileInput1Path);
                        }

                        string fileInput2Path = "";
                        if (fileInput2 != null)
                        {
                            fileInput2Path = Path.Combine(TicketingFolderPath, fileInput2Name);
                            fileInput2.SaveAs(fileInput2Path);
                        }
                        context.SaveChanges();
                        model.StatusCode = 200;
                        model.Message = "Purchase Request Submitted successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private string SaveFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var filePath = Path.Combine(Server.MapPath("~/uploads"), Path.GetFileName(file.FileName));
                file.SaveAs(filePath);
                return Path.GetFileName(file.FileName);
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExportSelectedPurchaseRequests(List<int> selectedPurchaseRequestIds)
        {
            try
            {
                // Validate input
                if (selectedPurchaseRequestIds == null || !selectedPurchaseRequestIds.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No purchase requests selected.");
                }

                var purchaseRequests = _dbContext.PurchaseRequests
                                    .Where(pr => selectedPurchaseRequestIds.Contains(pr.PurchaseRequestID))
                                    .ToList();

                if (!purchaseRequests.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "No purchase requests found.");
                }

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PurchaseRequests");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Request No.";
                    worksheet.Cells[1, 2].Value = "Asset Type";
                    worksheet.Cells[1, 3].Value = "Requested By";
                    worksheet.Cells[1, 4].Value = "Vendor 1 Name";
                    worksheet.Cells[1, 5].Value = "Vendor 1 File";
                    worksheet.Cells[1, 6].Value = "Vendor 2 Name";
                    worksheet.Cells[1, 7].Value = "Vendor 2 File";
                    worksheet.Cells[1, 8].Value = "Vendor 3 Name";
                    worksheet.Cells[1, 9].Value = "Vendor 3 File";

                    // Style headers
                    using (var range = worksheet.Cells[1, 1, 1, 9])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                        range.Style.Font.Bold = true;
                    }

                    // Add data
                    for (int i = 0; i < purchaseRequests.Count; i++)
                    {
                        var pr = purchaseRequests[i];
                        worksheet.Cells[i + 2, 1].Value = pr.PRNumber;
                        worksheet.Cells[i + 2, 2].Value = pr.AssetType;
                        worksheet.Cells[i + 2, 3].Value = pr.RequestedBy;
                        worksheet.Cells[i + 2, 4].Value = pr.VendorName1;
                        worksheet.Cells[i + 2, 5].Value = pr.AttachFile1; // File name only
                        worksheet.Cells[i + 2, 6].Value = pr.VendorName2;
                        worksheet.Cells[i + 2, 7].Value = pr.AttachFile2; // File name only
                        worksheet.Cells[i + 2, 8].Value = pr.VendorName3;
                        worksheet.Cells[i + 2, 9].Value = pr.AttachFile3; // File name only
                    }

                    var stream = new MemoryStream(excelPackage.GetAsByteArray());

                    // Return file for download
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SelectedPurchaseRequests.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public JsonResult Getpurchaserequestdetails(int id)
        {
            var purchase = _dbContext.PurchaseRequests.Where(x => x.PurchaseRequestID == id).OrderByDescending(x => x.PurchaseRequestID).FirstOrDefault();
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(purchase), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PurchaseAccountSubmit(int purchaseId)
        {
            var purchase = _dbContext.PurchaseRequests.Where(x => x.PurchaseRequestID == purchaseId).FirstOrDefault();
            return View("~/Views/Itsupport/PurchaseAccountSubmit.cshtml", purchase);
        }

        public ActionResult PurchaseSuperApproval(int purchaseId)
        {
            var purchase = _dbContext.PurchaseRequests.Where(x => x.PurchaseRequestID == purchaseId).OrderByDescending(x => x.PurchaseRequestID).FirstOrDefault();
            return View("~/Views/Itsupport/PurchaseSuperApprovalView.cshtml", purchase);
        }

        public ActionResult PurchasesuperAdmin()
        {
            var model = PurchaseListView();
            return View("~/Views/Itsupport/PurchaseSuperAdminView.cshtml", model);
        }

        public ActionResult PurchaseAccountAdmin()
        {
            var model = PurchaseListView();
            model.PurchaseRequests = model.PurchaseRequests.Where(x => x.FinalStatus == "Approved").ToList();
            return View("~/Views/Itsupport/PurchaseAccountAdmin.cshtml", model);
        }

        [HttpPost]
        public JsonResult UpdateVendorStatus(int purchaseId, string vendor1Status, string vendor2Status, string vendor3Status, string ButtonName)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();

            try
            {
                var purchaseRequest = _dbContext.PurchaseRequests.FirstOrDefault(pr => pr.PurchaseRequestID == purchaseId);
                if (purchaseRequest != null)
                {
                    purchaseRequest.Vendor1Status = vendor1Status;
                    purchaseRequest.Vendor2Status = vendor2Status;
                    purchaseRequest.Vendor3Status = vendor3Status;
                    purchaseRequest.UpdatedDate = DateTime.Now;
                    purchaseRequest.FinalStatus = ButtonName;

                    if (ButtonName == "Approved")
                    {
                        purchaseRequest.ApprovedBy = cuserContext.EmpInfo.EmployeeName;
                        purchaseRequest.ApprovedDate = DateTime.Now;

                        var approvalEmailBody = RenderPartialToString(this, "_PurchaseApprovedEmailNotification", purchaseRequest, ViewData, TempData);
                        var approvalEmailRequest = new EmailRequest()
                        {
                            Body = approvalEmailBody,
                            ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                            CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],
                            Subject = "Purchase Request Approved",
                        };
                        EMailHelper.SendEmail(approvalEmailRequest);
                    }

                    if (ButtonName == "Rejected")
                    {
                        purchaseRequest.RejectedBy = cuserContext.EmpInfo.EmployeeName;
                        purchaseRequest.RejectedDate = DateTime.Now;

                        var rejectionEmailBody = RenderPartialToString(this, "_PurchaseRejectedEmailNotification", purchaseRequest, ViewData, TempData);
                        var rejectionEmailRequest = new EmailRequest()
                        {
                            Body = rejectionEmailBody,
                            ToEmail = ConfigurationManager.AppSettings["VendorEmailsTo"],
                            CCEmail = ConfigurationManager.AppSettings["VendorEmailsCC"],
                            Subject = "Purchase Request Rejected",
                        };
                        EMailHelper.SendEmail(rejectionEmailRequest);
                    }

                    _dbContext.SaveChanges();
                }
                return Json(new { success = true, message = "Purchase request " + ButtonName + " successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ExportSelectedAccountPurchaseRequests(List<int> selectedPurchaseRequestIds)
        {
            try
            {
                // Check if any IDs are provided
                if (selectedPurchaseRequestIds == null || !selectedPurchaseRequestIds.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No purchase request IDs provided.");
                }

                // Fetch purchase requests based on selected IDs
                var purchaseRequests = _dbContext.PurchaseRequests
                    .Where(pr => selectedPurchaseRequestIds.Contains(pr.PurchaseRequestID))
                    .ToList();

                // Filter to include only those requests where at least one vendor is approved
                var approvedPurchaseRequests = purchaseRequests
                    .Where(pr =>
                        (pr.Vendor1Status == "Approved") ||
                        (pr.Vendor2Status == "Approved") ||
                        (pr.Vendor3Status == "Approved")
                    )
                    .ToList();

                if (!approvedPurchaseRequests.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent, "No approved vendors to export.");
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("PurchaseRequests");

                    worksheet.Cells[1, 1].Value = "Request No.";
                    worksheet.Cells[1, 2].Value = "Asset Type";
                    worksheet.Cells[1, 3].Value = "Requested By";
                    worksheet.Cells[1, 4].Value = "Vendor Name";
                    worksheet.Cells[1, 5].Value = "Vendor Email";
                    worksheet.Cells[1, 6].Value = "Quotation Price";
                    worksheet.Cells[1, 7].Value = "Attachment File";

                    using (var range = worksheet.Cells[1, 1, 1, 7])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                        range.Style.Font.Bold = true;
                    }

                    int rowIndex = 2;
                    foreach (var pr in approvedPurchaseRequests)
                    {
                        if (pr.Vendor1Status == "Approved")
                        {
                            worksheet.Cells[rowIndex, 1].Value = pr.PRNumber;
                            worksheet.Cells[rowIndex, 2].Value = pr.AssetType;
                            worksheet.Cells[rowIndex, 3].Value = pr.RequestedBy;
                            worksheet.Cells[rowIndex, 4].Value = pr.VendorName1;
                            worksheet.Cells[rowIndex, 5].Value = pr.VendorEmail1;
                            worksheet.Cells[rowIndex, 6].Value = pr.QuotationPrice1;
                            worksheet.Cells[rowIndex, 7].Value = pr.AttachFile1;
                            rowIndex++;
                        }

                        if (pr.Vendor2Status == "Approved")
                        {
                            worksheet.Cells[rowIndex, 1].Value = pr.PRNumber;
                            worksheet.Cells[rowIndex, 2].Value = pr.AssetType;
                            worksheet.Cells[rowIndex, 3].Value = pr.RequestedBy;
                            worksheet.Cells[rowIndex, 4].Value = pr.VendorName2;
                            worksheet.Cells[rowIndex, 5].Value = pr.VendorEmail2;
                            worksheet.Cells[rowIndex, 6].Value = pr.QuotationPrice2;
                            worksheet.Cells[rowIndex, 7].Value = pr.AttachFile2;
                            rowIndex++;
                        }

                        if (pr.Vendor3Status == "Approved")
                        {
                            worksheet.Cells[rowIndex, 1].Value = pr.PRNumber;
                            worksheet.Cells[rowIndex, 2].Value = pr.AssetType;
                            worksheet.Cells[rowIndex, 3].Value = pr.RequestedBy;
                            worksheet.Cells[rowIndex, 4].Value = pr.VendorName3;
                            worksheet.Cells[rowIndex, 5].Value = pr.VendorEmail3;
                            worksheet.Cells[rowIndex, 6].Value = pr.QuotationPrice3;
                            worksheet.Cells[rowIndex, 7].Value = pr.AttachFile3;
                            rowIndex++;
                        }
                    }

                    var stream = new MemoryStream(package.GetAsByteArray());
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SelectedPurchaseRequests.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // You can use a logging framework or simply log to the console for debugging
                Console.WriteLine(ex.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while generating the export.");
            }
        }

    }
}
