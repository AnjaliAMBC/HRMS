﻿using HRMS.Helpers;
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
            var model = new PurchaseListViewModel();
            model.PurchaseRequests = _dbContext.PurchaseRequests.ToList();
            model.PurchasRequestFolderPath = "/purchase";
            return View("~/Views/Itsupport/PurchaseListView.cshtml", model);
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


            decimal vendor1Quotation = System.Convert.ToDecimal(Request.Form["Vendor1quotation"]);
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


            decimal vendor2Quotation = !string.IsNullOrWhiteSpace(Request.Form["Vendor2quotation"]) ? System.Convert.ToDecimal(Request.Form["Vendor2quotation"]) : 0;
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

            decimal vendor3Quotation = !string.IsNullOrWhiteSpace(Request.Form["Vendor3quotation"]) ? System.Convert.ToDecimal(Request.Form["Vendor3quotation"]) : 0;

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


                    };

                    context.PurchaseRequests.Add(purchaseRequest);
                }
                else
                {

                    purchaseRequest.AssetType = assetType;
                    purchaseRequest.RequestedBy = requestedBy;
                    purchaseRequest.RequiredOn = DateTime.Parse(requiredOn);
                    purchaseRequest.VendorName1 = vendorName1;
                    purchaseRequest.QuotationPrice1 = vendor1Quotation;
                    purchaseRequest.VendorName2 = vendorName2;
                    purchaseRequest.QuotationPrice2 = vendor2Quotation;
                    purchaseRequest.VendorName3 = vendorName3;
                    purchaseRequest.QuotationPrice3 = vendor3Quotation;
                    purchaseRequest.UpdatedBy = cuserContext.LoginInfo.EmployeeName;
                    purchaseRequest.UpdatedDate = DateTime.Now;
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


        public JsonResult Getpurchaserequestdetails(int id)
        {
            var purchase = _dbContext.PurchaseRequests.Where(x => x.PurchaseRequestID == id).FirstOrDefault();
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(purchase), JsonRequestBehavior.AllowGet);
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
