using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
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

        //[HttpGet]
        ////public ActionResult AddPurchaseRequest(int purchaseId = 0)
        //{
        //    var model = new PurchaseRequest();

        //    if (purchaseId != 0)
        //    {
        //        model.EditPurchase = _dbContext.PurchaseRequest.FirstOrDefault(x => x.PurchaseID == purchaseId);
        //    }

        //    model.AssetTypes = _dbContext.AssetTypes.ToList();
        //    model.RequestedByList = _dbContext.Employees.Where(x => x.IsActive).ToList();
        //    model.Vendors = _dbContext.Vendors.Where(x => x.IsActive).ToList();

        //    return View("~/Views/Itsupport/PurchaseAdd.cshtml", model);
        //}

        //[HttpPost]
        //public JsonResult AddPurchaseRequest(PurchaseRequest purchaseRequest)
        //{
        //    var jsonResponse = new JsonResponse();

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (purchaseRequest.PurchaseID == 0)
        //            {
        //                _dbContext.PurchaseRequests.Add(purchaseRequest);
        //                jsonResponse.Message = "Purchase request added successfully!";
        //            }
        //            else
        //            {
        //                var existingRequest = _dbContext.PurchaseRequests.Find(purchaseRequest.PurchaseID);
        //                if (existingRequest != null)
        //                {
        //                    existingRequest.AssetType = purchaseRequest.AssetType;
        //                    existingRequest.RequiredOn = purchaseRequest.RequiredOn;
        //                    existingRequest.RequestedBy = purchaseRequest.RequestedBy;
        //                    existingRequest.VendorName = purchaseRequest.VendorName;
        //                    existingRequest.QuotationPrice = purchaseRequest.QuotationPrice;
        //                    existingRequest.AttachFile = purchaseRequest.AttachFile;
        //                    jsonResponse.Message = "Purchase request updated successfully!";
        //                }
        //            }

        //            _dbContext.SaveChanges();
        //            jsonResponse.StatusCode = 200;
        //        }
        //        else
        //        {
        //            jsonResponse.Message = "Invalid data!";
        //            jsonResponse.StatusCode = 400;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        jsonResponse = ErrorHelper.CaptureError(ex);
        //    }

        //    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult PurchaseAdd()
        {
            return View("~/Views/Itsupport/PurchaseAdd.cshtml");
        }

        public ActionResult PurchaseImport()
        {
            return View("~/Views/Itsupport/PurchangeImport.cshtml");
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
