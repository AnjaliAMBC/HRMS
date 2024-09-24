using HRMS.Helpers;
using HRMS.Models;
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
            var model = new AssetListModel();

            model.Assets = _dbContext.Assets.ToList();
            model.TotalAssets = model.Assets.Count();
            model.AssetsInUse = model.Assets.Where(x => x.AllocatedStatus == "Assigned").ToList().Count();
            model.AssetsInScrap = model.Assets.Where(x => x.AllocatedStatus == "Scrap").ToList().Count();
            model.HydAssets = model.Assets.Where(x => x.AllocatedStatus == "Assigned" && x.Location == "Hyderabad").ToList().Count();
            model.MaduraiAssets = model.Assets.Where(x => x.AllocatedStatus == "Assigned" && x.Location == "Madurai").ToList().Count();

            model.AssetModel.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.AssetModel.allVendors = _dbContext.VendorLists.Where(x => x.Status == "Approved").ToList();

            return View("~/Views/Itsupport/AssetInfo.cshtml", model);
        }

        public ActionResult AddAsset(int sno = 0)
        {
            var model = new AssetModel();

            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.allVendors = _dbContext.VendorLists.Where(x => x.Status == "Approved").ToList();
            if (sno != 0)
            {
                model.EditAssets = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();
            }

            return View("~/Views/Itsupport/AddAsset.cshtml", model);
        }

        [HttpPost]
        public JsonResult AddAssetAllocation()
        {
            var response = new JsonResponse();
            var cuserContext = SiteContext.GetCurrentUserContext(); // Assuming this gets the current user

            try
            {
                // Retrieving form data
                int sno = System.Convert.ToInt32(Request.Form["AssetSNo"]);
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
                HttpPostedFileBase assetImage1 = Request.Files["AssetImage"];
                HttpPostedFileBase assetImage2 = Request.Files["AssetImage-1"];
                HttpPostedFileBase assetImage3 = Request.Files["AssetImage-2"];
                HttpPostedFileBase assetImage4 = Request.Files["AssetImage-3"];
                HttpPostedFileBase mainImage = Request.Files["MainImage"];

                string assetImage1Path = "";
                string assetImage2Path = "";
                string assetImage3Path = "";
                string assetImage4Path = "";
                string mainImagePath = "";

                // Example of saving the files
                if (assetImage1 != null)
                {
                    assetImage1Path = SaveFile(assetImage1, assetID);
                }
                if (assetImage2 != null)
                {
                    assetImage2Path = SaveFile(assetImage2, assetID);
                }
                if (assetImage3 != null)
                {
                    assetImage3Path = SaveFile(assetImage3, assetID);
                }
                if (assetImage4 != null)
                {
                    assetImage4Path = SaveFile(assetImage4, assetID);
                }
                if (mainImage != null)
                {
                    mainImagePath = SaveFile(mainImage, assetID);
                }

                if (sno == 0)
                {
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
                        AssetType1Image = assetImage1Path,
                        AssetType2Image = assetImage2Path,
                        AssetType3Image = assetImage3Path,
                        AssetType4Image = assetImage4Path,
                        AssetMainImage = mainImagePath,
                        CreatedBy = cuserContext.EmpInfo.EmployeeName,
                        CreatedDate = DateTime.Now

                    };
                    _dbContext.Assets.Add(assetModel);
                }
                else
                {
                    var existingAsset = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();

                    if (existingAsset != null)
                    {
                        existingAsset.AssetID = assetID;
                        existingAsset.AssetType1 = assetType;
                        existingAsset.AssetType2 = assetType1;
                        existingAsset.AssetType3 = assetType2;
                        existingAsset.AssetType4 = assetType3;
                        existingAsset.Manufacturer = manufacture;
                        existingAsset.Model = model;
                        existingAsset.BarCode = barcode;
                        existingAsset.RAM = ram;
                        existingAsset.WarrantyStartDate = string.IsNullOrWhiteSpace(warrantyStartDate) ? (DateTime?)null : System.Convert.ToDateTime(warrantyStartDate);
                        existingAsset.WarrantyEndDate = string.IsNullOrWhiteSpace(warrantyEndDate) ? (DateTime?)null : System.Convert.ToDateTime(warrantyEndDate);
                        existingAsset.VendorName = vendorName;
                        existingAsset.PONumber = poNumber;
                        existingAsset.PurchaseDate = string.IsNullOrWhiteSpace(purchaseDate) ? (DateTime?)null : System.Convert.ToDateTime(purchaseDate);
                        existingAsset.InvoiceDate = string.IsNullOrWhiteSpace(invoiceDate) ? (DateTime?)null : System.Convert.ToDateTime(invoiceDate);
                        existingAsset.Location = location;
                        existingAsset.AllocatedEmployeeName = employee;
                        existingAsset.AssignedBy = assignedBy;
                        existingAsset.AssignedDate = string.IsNullOrWhiteSpace(assignedDate) ? (DateTime?)null : System.Convert.ToDateTime(assignedDate);
                        existingAsset.AllocatedStatus = allocateStatus;
                        existingAsset.Remarks = remarks;
                        existingAsset.AssetType1Image = string.IsNullOrWhiteSpace(assetImage1Path) ? existingAsset.AssetType1Image : assetImage1Path;
                        existingAsset.AssetType2Image = string.IsNullOrWhiteSpace(assetImage2Path) ? existingAsset.AssetType2Image : assetImage2Path;
                        existingAsset.AssetType3Image = string.IsNullOrWhiteSpace(assetImage3Path) ? existingAsset.AssetType3Image : assetImage3Path;
                        existingAsset.AssetType4Image = string.IsNullOrWhiteSpace(assetImage4Path) ? existingAsset.AssetType4Image : assetImage4Path;
                        existingAsset.AssetMainImage = string.IsNullOrWhiteSpace(mainImagePath) ? existingAsset.AssetMainImage : mainImagePath;
                        existingAsset.UpdatedBy = cuserContext.EmpInfo.EmployeeName;
                        existingAsset.UpdatedDate = DateTime.Now;
                    }
                }
                _dbContext.SaveChanges();
                response.StatusCode = 200;
                response.Message = "Asset allocation data successfully submitted!";
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

                var assetFolderID = assetID.Split('#')[1];
                string assetFolderPath = Path.Combine(assetsFolderPath, assetFolderID);
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

        public ActionResult AssetView()
        {
            return View("~/Views/Itsupport/AssetViewInfo.cshtml");
        }

    }
}