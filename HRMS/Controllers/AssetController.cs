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
                string assetType4 = Request.Form["AssetType-4"];
                string assetType5 = Request.Form["AssetType-5"];
                string assetType6 = Request.Form["AssetType-6"];

                string assetTypesno1 = Request.Form["AssetType-sno1"];
                string assetTypesno2 = Request.Form["AssetType-sno2"];
                string assetTypesno3 = Request.Form["AssetType-sno3"];
                string assetTypesno4 = Request.Form["AssetType-sno4"];
                string assetTypesno5 = Request.Form["AssetType-sno5"];
                string assetTypesno6 = Request.Form["AssetType-sno6"];
                string assetTypesno7 = Request.Form["AssetType-sno7"];

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
                HttpPostedFileBase assetImage5 = Request.Files["AssetImage-4"];
                HttpPostedFileBase assetImage6 = Request.Files["AssetImage-5"];
                HttpPostedFileBase assetImage7 = Request.Files["AssetImage-6"];

                HttpPostedFileBase mainImage = Request.Files["MainImage"];

                string assetImage1Path = "";
                string assetImage2Path = "";
                string assetImage3Path = "";
                string assetImage4Path = "";
                string assetImage5Path = "";
                string assetImage6Path = "";
                string assetImage7Path = "";

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
                if (assetImage5 != null)
                {
                    assetImage5Path = SaveFile(assetImage5, assetID);
                }
                if (assetImage6 != null)
                {
                    assetImage6Path = SaveFile(assetImage6, assetID);
                }
                if (assetImage7 != null)
                {
                    assetImage7Path = SaveFile(assetImage7, assetID);
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
                        AssetType5 = assetType4,
                        AssetType6 = assetType5,
                        //AssetType7 = assetType6,

                        //AssetTypeSno1 = assetTypesno1,
                        //AssetTypeSno2 = assetTypesno2,
                        //AssetTypeSno3 = assetTypesno3,
                        //AssetTypeSno4 = assetTypesno4,
                        //AssetTypeSno5 = assetTypesno5,
                        //AssetTypeSno6 = assetTypesno6,
                        //AssetTypeSno7 = assetTypesno7,

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
                        AssetType5Image = assetImage5Path,
                        AssetType6Image = assetImage5Path,
                        //AssetType7Image = assetImage6Path,

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

                        existingAsset.AssetType5 = assetType4;
                        existingAsset.AssetType6 = assetType5;
                        //existingAsset.AssetType7 = assetType6;


                        //existingAsset.AssetTypeSno1 = assetTypesno1,
                        //existingAsset.AssetTypeSno2 = assetTypesno2,
                        //existingAsset.AssetTypeSno3 = assetTypesno3,
                        //existingAsset.AssetTypeSno4 = assetTypesno4,
                        //existingAsset.AssetTypeSno5 = assetTypesno5,
                        //existingAsset.AssetTypeSno6 = assetTypesno6,
                        //existingAsset.AssetTypeSno7 = assetTypesno7,

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

                        existingAsset.AssetType5Image = string.IsNullOrWhiteSpace(assetImage5Path) ? existingAsset.AssetType5Image : assetImage5Path;
                        existingAsset.AssetType6Image = string.IsNullOrWhiteSpace(assetImage6Path) ? existingAsset.AssetType6Image : assetImage6Path;
                        //existingAsset.AssetType7Image = string.IsNullOrWhiteSpace(assetImage7Path) ? existingAsset.AssetType7Image : assetImage7Path;

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

        public ActionResult AssetTransfer(int sno = 0)
        {
            var model = new AssetModel();

            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.ITEmployees = model.Employees.Where(x => x.Department == "ÏT").ToList();
            if (sno != 0)
            {
                model.EditAssets = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();
                if (model.EditAssets != null)
                {
                    model.AllocatedEmpInfo = model.Employees.Where(x => x.EmployeeID == model.EditAssets.AllocatedEmployeeID).FirstOrDefault();
                }
            }


            return PartialView("~/Views/Itsupport/_AssetTransfer.cshtml", model);
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