using HRMS.Helpers;
using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    public class AssetController : Controller
    {
        // Database context
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public AssetController()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        // GET: Asset
        public ActionResult AssetInfo()
        {
            return View("~/Views/Itsupport/AssetInfo.cshtml");
        }

        public ActionResult AddAsset()
        {
            return View("~/Views/Itsupport/AddAsset.cshtml");
        }

        [HttpPost]
        public JsonResult AddAssetAllocation()
        {
            var response = new JsonResponse();
            var cuserContext = SiteContext.GetCurrentUserContext(); // Assuming this gets the current user

            try
            {
                // Retrieving form data
                string assetID = Request.Form["AssetID"];
                string assetType = Request.Form["AssetType"];
                string assetType1 = Request.Form["AssetType-1"];
                string assetType2 = Request.Form["AssetType-2"];
                string assetType3 = Request.Form["AssetType-3"];
                string manufacture = Request.Form["AssetManufacture"];
                string model = Request.Form["AssetModel"];
                string barcode = Request.Form["AssetBarcode"];
                string ram = Request.Form["AssetRAM"];
                string warrantyStartDate = Request.Form["AssetWarrentyStartDate"];
                string warrantyEndDate = Request.Form["AssetWarrentyEndDate"];
                string vendorName = Request.Form["AssetPurchase-Vendorname"];
                string poNumber = Request.Form["AssetPurchase-PONumber"];
                string purchaseDate = Request.Form["AssetPurchase-PurchaseDate"];
                string invoiceDate = Request.Form["AssetPurchase-InvoiceDate"];
                string location = Request.Form["AssetAllocate-Location"];
                string employee = Request.Form["AssetAllocate-Employee"];
                string acknowledgeEmail = Request.Form["AssetAllocate-AcknowledgeEmail"];
                string assignedBy = Request.Form["AssetAllocate-AssignedBy"];
                string assignedDate = Request.Form["AssetAllocate-AssignedDate"];
                string allocateStatus = Request.Form["AssetAllocate-AllocateStatus"];
                string remarks = Request.Form["AssetAllocate-Remarks"];

                // Handle file uploads
                HttpPostedFileBase assetImage = Request.Files["AssetImage"];
                HttpPostedFileBase assetImage1 = Request.Files["AssetImage-1"];
                HttpPostedFileBase assetImage2 = Request.Files["AssetImage-2"];
                HttpPostedFileBase assetImage3 = Request.Files["AssetImage-3"];
                HttpPostedFileBase mainImage = Request.Files["MainImage"];

                // Example of saving the files
                string assetImagePath = SaveFile(assetImage, assetID);
                string assetImage1Path = SaveFile(assetImage1, assetID);
                string assetImage2Path = SaveFile(assetImage2, assetID);
                string assetImage3Path = SaveFile(assetImage3, assetID);
                string mainImagePath = SaveFile(mainImage, assetID);

                // Create and populate the model
                Asset assetModel = new Asset
                {
                    AssetID = assetID,
                    AssetType1 = assetType,
                    AssetType2 = assetType1,
                    AssetType3 = assetType2,
                    AssetType4 = assetType3,
                    Manufacturer = manufacture,
                    Model = model,
                    BarCode = barcode,
                    RAM = ram,
                    WarrantyStartDate = string.IsNullOrWhiteSpace(warrantyStartDate) ? (DateTime?)null : System.Convert.ToDateTime(warrantyStartDate),
                    WarrantyEndDate = string.IsNullOrWhiteSpace(warrantyEndDate) ? (DateTime?)null : System.Convert.ToDateTime(warrantyEndDate),
                    VendorName = vendorName,
                    PONumber = poNumber,
                    PurchaseDate = string.IsNullOrWhiteSpace(purchaseDate) ? (DateTime?)null : System.Convert.ToDateTime(purchaseDate),
                    InvoiceDate = string.IsNullOrWhiteSpace(invoiceDate) ? (DateTime?)null : System.Convert.ToDateTime(invoiceDate),
                    Location = location,
                    AllocatedEmployeeName = employee,
                    AssignedBy = assignedBy,
                    AssignedDate = string.IsNullOrWhiteSpace(assignedDate) ? (DateTime?)null : System.Convert.ToDateTime(assignedDate),
                    AllocatedStatus = allocateStatus,
                    Remarks = remarks,
                    AssetType1Image = assetImagePath,
                    AssetType2Image = assetImage1Path,
                    AssetType3Image = assetImage2Path,
                    AssetType4Image = assetImage3Path,
                    AssetMainImage = mainImagePath
                };

                _dbContext.Assets.Add(assetModel);
                _dbContext.SaveChanges();
                response.StatusCode = 200;
                response.Message = "Asset allocation data successfully added!";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "An error occurred: " + ex.Message;
            }

            return Json(response);
        }

        // Helper method to save files
        private string SaveFile(HttpPostedFileBase file, string assetID)
        {
            if (file != null && file.ContentLength > 0)
            {
                var ticketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                string assetsFolderPath = Path.Combine(ticketingFolderPath, "Assets");

                if (!Directory.Exists(assetsFolderPath))
                {
                    Directory.CreateDirectory(assetsFolderPath);
                }

                string assetFolderPath = Path.Combine(assetsFolderPath, assetID);
                if (!Directory.Exists(assetFolderPath))
                {
                    Directory.CreateDirectory(assetFolderPath);
                }

                string filePath = Path.Combine(assetFolderPath, Path.GetFileName(file.FileName));
                file.SaveAs(filePath);

                return file.FileName;
            }
            return null;
        }



        public ActionResult AssetBulkImport()
        {
            return View("~/Views/Itsupport/AssetBulk.cshtml");
        }
    }
}