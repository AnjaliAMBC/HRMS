using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
            return View("~/Views/Itsupport/PurchaseListView.cshtml");
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
            model.allVendors = _dbContext.VendorLists.ToList();
            model.ITDeptEmployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();
            var lastPurchaseRequestId = _dbContext.PurchaseRequests
                                                   .OrderByDescending(x => x.PurchaseRequestID)
                                                   .Select(x => x.PurchaseRequestID)
                                                   .FirstOrDefault();
            model.LastePurchaseId = lastPurchaseRequestId;
            model.NewPurchaseId = model.LastePurchaseId + 1;


            return View("~/Views/Itsupport/PurchaseAdd.cshtml", model);
        }

        [HttpPost]
        public JsonResult AddPurchaseRequest(PurchaseRequest purchaseRequest, HttpPostedFileBase attachFile1, HttpPostedFileBase attachFile2, HttpPostedFileBase attachFile3)
        {
            var model = new JsonResponse();

            try
            {
                if (string.IsNullOrEmpty(purchaseRequest.VendorName1))
                {
                    model.Message = "Vendor Name is required.";
                    model.StatusCode = 400; // Bad Request
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

               
                if (purchaseRequest.QuotationPrice1 <= 0) 
                {
                    model.Message = "Quotation Price must be greater than zero.";
                    model.StatusCode = 400; // Bad Request
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
             
                if (attachFile1 == null || attachFile1.ContentLength == 0)
                {
                    model.Message = "Attach File is required.";
                    model.StatusCode = 400; // Bad Request
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

                var existingPurchaseRequest = _dbContext.PurchaseRequests.Find(purchaseRequest.PurchaseRequestID);

                if (existingPurchaseRequest == null)
                {
                    // New purchase request
                    purchaseRequest.Status = "Pending";
                    
                    if (attachFile1 != null && attachFile1.ContentLength > 0)
                    {
                        var filePath1 = SaveFile(attachFile1);
                        purchaseRequest.AttachFile1 = filePath1;
                    }

                    if (attachFile2 != null && attachFile2.ContentLength > 0)
                    {
                        var filePath2 = SaveFile(attachFile2);
                        purchaseRequest.AttachFile2 = filePath2;
                    }

                    if (attachFile3 != null && attachFile3.ContentLength > 0)
                    {
                        var filePath3 = SaveFile(attachFile3);
                        purchaseRequest.AttachFile3 = filePath3;
                    }

                    _dbContext.PurchaseRequests.Add(purchaseRequest);
                    model.Message = "Purchase request added successfully!";
                }
                else
                {
                    // Update existing purchase request
                    existingPurchaseRequest.AssetType = purchaseRequest.AssetType;
                    existingPurchaseRequest.RequestedBy = purchaseRequest.RequestedBy;
                    existingPurchaseRequest.RequiredOn = purchaseRequest.RequiredOn;
                    existingPurchaseRequest.VendorName1 = purchaseRequest.VendorName1;
                    existingPurchaseRequest.QuotationPrice1 = purchaseRequest.QuotationPrice1;
                    existingPurchaseRequest.AttachFile1 = purchaseRequest.AttachFile1;
                    existingPurchaseRequest.VendorName2 = purchaseRequest.VendorName2;
                    existingPurchaseRequest.QuotationPrice2 = purchaseRequest.QuotationPrice2;
                    existingPurchaseRequest.AttachFile2 = purchaseRequest.AttachFile2;
                    existingPurchaseRequest.VendorName3 = purchaseRequest.VendorName3;
                    existingPurchaseRequest.QuotationPrice3 = purchaseRequest.QuotationPrice3;
                    existingPurchaseRequest.AttachFile3 = purchaseRequest.AttachFile3;

                    model.Message = "Purchase request updated successfully!";
                }

                _dbContext.SaveChanges();

                var emailBody = RenderPartialToString(this, "_PurchaseRequestEmailNotification", purchaseRequest, ViewData, TempData);

                var emailRequest = new EmailRequest
                {
                    Body = emailBody,
                    ToEmail = ConfigurationManager.AppSettings["PurchaseRequestEmailsTo"],
                    CCEmail = ConfigurationManager.AppSettings["PurchaseRequestEmailsCC"],
                    Subject = "New Purchase Request Approval",
                };

                EMailHelper.SendEmail(emailRequest);

                model.StatusCode = 200; 
            }
            catch (Exception ex)
            {
                model = ErrorHelper.CaptureError(ex);
                model.StatusCode = 500; // Internal Server Error
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

    
    public ActionResult PurchaseAdd()
        {
            return View("~/Views/Itsupport/PurchaseAdd.cshtml");
        }

        public ActionResult PurchaseImport()
        {
            return View("~/Views/Itsupport/PurchangeImport.cshtml");
        }

        public ActionResult PurchaseAccountSubmit()
        {
            return View("~/Views/Itsupport/PurchaseAccountSubmit.cshtml");
        }

        public ActionResult PurchaseSuperApproval()
        {
            return View("~/Views/Itsupport/PurchaseSuperApprovalView.cshtml");
        }
       
        public ActionResult PurchasesuperAdmin()
        {
            return View("~/Views/Itsupport/PurchaseSuperAdminView.cshtml");
        }

        public ActionResult PurchaseAccountAdmin()
        {
            return View("~/Views/Itsupport/PurchaseAccountAdmin.cshtml");
        }
    }
}
