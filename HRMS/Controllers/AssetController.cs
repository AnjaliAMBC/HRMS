using HRMS.Helpers;
using HRMS.Models;
using HRMS.Models.Employee;
using HRMS.Models.ITsupport;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;

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

            model.Assets = _dbContext.Assets.OrderByDescending(x => x.CreatedDate).ToList();
            model.TotalAssets = model.Assets.Count();
            model.AssetsInUse = model.Assets.Where(x => x.AllocatedStatus.Trim() == "Assigned").ToList().Count();
            model.AssetsInScrap = model.Assets.Where(x => x.AllocatedStatus.Trim() == "Scrap").Count();
            model.HydAssets = model.Assets.Where(x => x.AllocatedStatus.Trim() == "Assigned" && x.Location.Trim() == "Hyderabad").ToList().Count();
            model.MaduraiAssets = model.Assets.Where(x => x.AllocatedStatus.Trim() == "Assigned" && x.Location.Trim() == "Madurai").ToList().Count();
            model.AssetModel.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.AssetModel.allVendors = _dbContext.VendorLists.Where(x => x.Status == "Approved").ToList();
            model.AssetModel.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department.Contains("IT")).ToList();
            return View("~/Views/Itsupport/AssetInfo.cshtml", model);
        }
        public ActionResult AddAsset(int sno = 0)
        {
            var model = new AssetModel();

            model.Employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active").ToList();
            model.ITEmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Department.Contains("IT")).ToList();
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

                string assetTypesno1 = Request.Form["AssetSno1"];
                string assetTypesno2 = Request.Form["AssetSno2"];
                string assetTypesno3 = Request.Form["AssetSno3"];
                string assetTypesno4 = Request.Form["AssetSno4"];
                string assetTypesno5 = Request.Form["AssetSno5"];
                string assetTypesno6 = Request.Form["AssetSno6"];
                string assetTypesno7 = Request.Form["AssetSno7"];

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
                string employeeID = Request.Form["AssetAllocate-Employee"];
                string employeeName = Request.Form["AssetAllocate-EmployeeName"];
                string acknowledgeEmail = Request.Form["AssetAllocate-AcknowledgeEmail"];
                string assignedByID = Request.Form["AssetAllocate-AssignedBy"];
                string assignedByEmpName = Request.Form["AssetAllocate-AssignedByName"];
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
                        AssetType7 = assetType6,

                        AssetSerialNo1 = assetTypesno1,
                        AssetSerialNo2 = assetTypesno2,
                        AssetSerialNo3 = assetTypesno3,
                        AssetSerialNo4 = assetTypesno4,
                        AssetSerialNo5 = assetTypesno5,
                        AssetSerialNo6 = assetTypesno6,
                        AssetSerialNo7 = assetTypesno7,

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
                        AllocatedEmployeeName = employeeName,
                        AllocatedEmployeeID = employeeID,
                        AssignedBy = assignedByEmpName,
                        AssignedByEmpID = assignedByID,
                        AssignedDate = string.IsNullOrWhiteSpace(assignedDate) ? (DateTime?)null : System.Convert.ToDateTime(assignedDate),
                        AllocatedStatus = allocateStatus,
                        Remarks = remarks,
                        AssetType1Image = assetImage1Path,
                        AssetType2Image = assetImage2Path,
                        AssetType3Image = assetImage3Path,
                        AssetType4Image = assetImage4Path,
                        AssetType5Image = assetImage5Path,
                        AssetType6Image = assetImage5Path,
                        AssetType7Image = assetImage6Path,

                        AssetMainImage = mainImagePath,
                        CreatedBy = cuserContext.EmpInfo.EmployeeName,
                        CreatedDate = DateTime.Now

                    };
                    _dbContext.Assets.Add(assetModel);

                    if (!string.IsNullOrWhiteSpace(assignedByID))
                    {
                        var assetTransferModel = new AssetTransfer_();
                        assetTransferModel.AssetID = assetID;
                        assetTransferModel.AllocatedEmpID = employeeID;
                        assetTransferModel.AllocatedEmpName = employeeName;
                        assetTransferModel.AssignedByEmpID = assignedByID;
                        assetTransferModel.AssignedByName = assignedByEmpName;
                        assetTransferModel.AssignedDate = string.IsNullOrWhiteSpace(assignedDate) ? (DateTime?)null : System.Convert.ToDateTime(assignedDate);
                        assetTransferModel.CreatedBy = cuserContext.EmpInfo.EmployeeName;
                        assetTransferModel.CreatedDate = DateTime.Now;
                    }

                    if (Request.Form["sendack"] == "true")
                    {
                        // Prepare the email
                        var emailSubject = $"Asset allocation Confirmation: [" + assetID + "]";
                        var emailBody = RenderPartialToString(this, "AssetCreationEmail", assetModel, ViewData, TempData);

                        var allocatedEmp = !string.IsNullOrWhiteSpace(employeeID) ?
                             _dbContext.emp_info.Where(x => x.EmployeeID == employeeID).FirstOrDefault()
                             : null;

                        var toemail = "";

                        if (allocatedEmp != null)
                        {
                            toemail = allocatedEmp.OfficalEmailid + "," + ConfigurationManager.AppSettings["AssetEmailTo"];
                        }
                        else
                        {
                            toemail = ConfigurationManager.AppSettings["AssetEmailTo"];
                        }

                        var emailRequest = new EmailRequest()
                        {
                            Body = emailBody,
                            ToEmail = toemail,
                            CCEmail = ConfigurationManager.AppSettings["AssetEmailCC"],
                            Subject = emailSubject
                        };

                        var sendNotification = EMailHelper.SendEmail(emailRequest);
                    }

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
                        existingAsset.AssetType7 = assetType6;

                        existingAsset.AssetSerialNo1 = assetTypesno1;
                        existingAsset.AssetSerialNo2 = assetTypesno2;
                        existingAsset.AssetSerialNo3 = assetTypesno3;
                        existingAsset.AssetSerialNo4 = assetTypesno4;
                        existingAsset.AssetSerialNo5 = assetTypesno5;
                        existingAsset.AssetSerialNo6 = assetTypesno6;
                        existingAsset.AssetSerialNo7 = assetTypesno7;

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
                        existingAsset.AllocatedEmployeeName = employeeName;
                        existingAsset.AllocatedEmployeeID = employeeID;
                        existingAsset.AssignedBy = assignedByEmpName;
                        existingAsset.AssignedByEmpID = assignedByID;
                        existingAsset.AssignedDate = string.IsNullOrWhiteSpace(assignedDate) ? (DateTime?)null : System.Convert.ToDateTime(assignedDate);
                        existingAsset.AllocatedStatus = allocateStatus;
                        existingAsset.Remarks = remarks;
                        existingAsset.AssetType1Image = string.IsNullOrWhiteSpace(assetImage1Path) ? existingAsset.AssetType1Image : assetImage1Path;
                        existingAsset.AssetType2Image = string.IsNullOrWhiteSpace(assetImage2Path) ? existingAsset.AssetType2Image : assetImage2Path;
                        existingAsset.AssetType3Image = string.IsNullOrWhiteSpace(assetImage3Path) ? existingAsset.AssetType3Image : assetImage3Path;
                        existingAsset.AssetType4Image = string.IsNullOrWhiteSpace(assetImage4Path) ? existingAsset.AssetType4Image : assetImage4Path;

                        existingAsset.AssetType5Image = string.IsNullOrWhiteSpace(assetImage5Path) ? existingAsset.AssetType5Image : assetImage5Path;
                        existingAsset.AssetType6Image = string.IsNullOrWhiteSpace(assetImage6Path) ? existingAsset.AssetType6Image : assetImage6Path;
                        existingAsset.AssetType7Image = string.IsNullOrWhiteSpace(assetImage7Path) ? existingAsset.AssetType7Image : assetImage7Path;

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

        [HttpGet]
        public JsonResult GetEmployeesByLocation(string Location)
        {
            var emmployees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active" && x.Location == Location).ToList();
            return Json(emmployees, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesByLocationAsset(string Location)
        {
            var employees = _dbContext.emp_info
                             .Where(x => x.EmployeeStatus == "Active" && x.Location == Location)
                             .Select(x => new { x.EmployeeID, x.EmployeeName })
                             .ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AssetTransfer(int sno = 0)
        {
            var model = new AssetModel();
            var employees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active");
            model.Employees = employees.ToList();
            model.ITEmployees = _dbContext.emp_info.Where(x => x.Department.Contains("IT")).ToList();
            if (sno != 0)
            {
                model.EditAssets = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();
                if (model.EditAssets != null)
                {
                    model.AllocatedEmpInfo = model.Employees.Where(x => x.EmployeeID == model.EditAssets.AllocatedEmployeeID).FirstOrDefault();
                }


            }

            return Json(JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssetTransferSubmit(AssetTransferPostModel assetTransferPostModel)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                var cuserContext = SiteContext.GetCurrentUserContext();
                var model = new AssetModel();

                if (assetTransferPostModel.sno != 0)
                {
                    var transerAssetRecord = _dbContext.Assets.Where(x => x.SNo == assetTransferPostModel.sno).FirstOrDefault(); ;
                    model.EditAssets = transerAssetRecord;
                    if (model.EditAssets != null)
                    {
                        model.AllocatedEmpInfo = model.Employees.Where(x => x.EmployeeID == model.EditAssets.AllocatedEmployeeID).FirstOrDefault();

                        var assetTransferModel = new AssetTransfer_();
                        assetTransferModel.AssetID = model.EditAssets.AssetID;
                        assetTransferModel.AllocatedEmpID = assetTransferPostModel.allocatedempid;
                        assetTransferModel.AllocatedEmpName = assetTransferPostModel.allocatedempname;
                        assetTransferModel.AssignedByEmpID = assetTransferPostModel.assignedbyid;
                        assetTransferModel.AssignedByName = assetTransferPostModel.assignedbyname;
                        assetTransferModel.Location = assetTransferPostModel.location;
                        assetTransferModel.AssignedDate = string.IsNullOrWhiteSpace(assetTransferPostModel.transferdate) ? (DateTime?)null : System.Convert.ToDateTime(assetTransferPostModel.transferdate);
                        assetTransferModel.CreatedBy = cuserContext.EmpInfo.EmployeeName;
                        assetTransferModel.CreatedDate = DateTime.Now;
                        _dbContext.AssetTransfer_.Add(assetTransferModel);
                        _dbContext.SaveChanges();

                        jsonResponse.StatusCode = 200;
                        jsonResponse.Message = "Asset transferred successfully!";

                        transerAssetRecord.AllocatedEmployeeID = assetTransferPostModel.allocatedempid;
                        transerAssetRecord.AllocatedEmployeeName = assetTransferPostModel.allocatedempname;
                        transerAssetRecord.AssignedBy = assetTransferPostModel.assignedbyname;
                        transerAssetRecord.AssignedByEmpID = assetTransferPostModel.assignedbyid;
                        transerAssetRecord.AssignedDate = string.IsNullOrWhiteSpace(assetTransferPostModel.transferdate) ? (DateTime?)null : System.Convert.ToDateTime(assetTransferPostModel.transferdate);
                        transerAssetRecord.AllocatedStatus = "Assigned";
                        transerAssetRecord.Location = assetTransferPostModel.location;
                        transerAssetRecord.UpdatedBy = cuserContext.EmpInfo.EmployeeName;
                        transerAssetRecord.UpdatedDate = DateTime.Now;
                        _dbContext.SaveChanges();


                        // Prepare the email
                        var emailSubject = $"Asset allocation Confirmation: [" + transerAssetRecord.AssetID + "]";
                        var emailBody = RenderPartialToString(this, "AssetCreationEmail", model.EditAssets, ViewData, TempData);

                        var allocatedEmp = !string.IsNullOrWhiteSpace(transerAssetRecord.AllocatedEmployeeID) ?
                             _dbContext.emp_info.Where(x => x.EmployeeID == transerAssetRecord.AllocatedEmployeeID).FirstOrDefault()
                             : null;

                        var toemail = "";

                        if (allocatedEmp != null)
                        {
                            toemail = allocatedEmp.OfficalEmailid + "," + ConfigurationManager.AppSettings["AssetEmailTo"];
                        }
                        else
                        {
                            toemail = ConfigurationManager.AppSettings["AssetEmailTo"];
                        }

                        var emailRequest = new EmailRequest()
                        {
                            Body = emailBody,
                            ToEmail = toemail,
                            CCEmail = ConfigurationManager.AppSettings["AssetEmailCC"],
                            Subject = emailSubject
                        };

                        var sendNotification = EMailHelper.SendEmail(emailRequest);

                    }
                }

                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonResponse.StatusCode = 500;
                jsonResponse.Message = ex.Message;
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult AssetBulkImport()
        {
            return View("~/Views/Itsupport/AssetBulk.cshtml");
        }

        public ActionResult AssetView(int sno = 0)
        {
            var model = new AssetModel();

            if (sno != 0)
            {
                model.EditAssets = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();

                if (model.EditAssets != null)
                {
                    model.AssetTransfers = _dbContext.AssetTransfer_.Where(x => x.AssetID == model.EditAssets.AssetID).ToList();

                    int venderNumber = !string.IsNullOrWhiteSpace(model.EditAssets.VendorName) && model.EditAssets.VendorName != "null" ? System.Convert.ToInt32(model.EditAssets.VendorName) : 0;
                    model.VendorInfo = _dbContext.VendorLists.Where(x => x.VedorID == venderNumber).FirstOrDefault();

                    foreach (var assetTransfer in model.AssetTransfers)
                    {
                        var assetTransferEmp = _dbContext.emp_info.Where(x => x.EmployeeID == assetTransfer.AllocatedEmpID).FirstOrDefault();
                        model.AssetTransferEmployees.Add(assetTransferEmp);
                    }
                    model.AllocatedEmpInfo = _dbContext.emp_info.Where(x => x.EmployeeID == model.EditAssets.AllocatedEmployeeID).FirstOrDefault();
                }
            }

            return View("~/Views/Itsupport/AssetViewInfo.cshtml", model);
        }

        public ActionResult DeleteAsset(int sno = 0)
        {
            var jsonResponse = new JsonResponse();
            try
            {
                if (sno != 0)
                {
                    var deleteAsset = _dbContext.Assets.Where(x => x.SNo == sno).FirstOrDefault();
                    if (deleteAsset != null)
                    {
                        _dbContext.Assets.Remove(deleteAsset);
                    }
                    _dbContext.SaveChanges();
                    jsonResponse.StatusCode = 200;
                    jsonResponse.Message = "Asset Deleted successfully!";

                }
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonResponse.StatusCode = 500;
                jsonResponse.Message = ex.Message;
                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ImportAssets(HttpPostedFileBase file)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            var model = new JsonResponse();

            try
            {
                // Check if a file is uploaded
                if (file != null && file.ContentLength > 0)
                {
                    List<Asset> assetList = new List<Asset>();

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
                            Asset asset = new Asset
                            {
                                AssetID = dataRow["AssetID"].ToString(),
                                AllocatedEmployeeName = dataRow["AllocatedEmployeeName"].ToString(),
                                AllocatedEmployeeID = dataRow["AllocatedEmployeeID"].ToString(),
                                Location = dataRow["Location"].ToString(),
                                AllocatedStatus = dataRow["AllocatedStatus"].ToString(),
                                AssignedBy = dataRow["AssignedBy"].ToString(),
                                AssignedByEmpID = dataRow["AssignedByEmpID"].ToString(),
                                AssignedDate = string.IsNullOrWhiteSpace(dataRow["AssignedDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dataRow["AssignedDate"]),
                                Remarks = dataRow["Remarks"].ToString(),
                                AssetType1 = dataRow["AssetType1"].ToString(),
                                AssetSerialNo1 = dataRow["AssetSerialNo1"].ToString(),
                                AssetType2 = dataRow["AssetType2"].ToString(),
                                AssetSerialNo2 = dataRow["AssetSerialNo2"].ToString(),
                                AssetType3 = dataRow["AssetType3"].ToString(),
                                AssetSerialNo3 = dataRow["AssetSerialNo3"].ToString(),
                                AssetType4 = dataRow["AssetType4"].ToString(),
                                AssetSerialNo4 = dataRow["AssetSerialNo4"].ToString(),
                                AssetType5 = dataRow["AssetType5"].ToString(),
                                AssetSerialNo5 = dataRow["AssetSerialNo5"].ToString(),
                                AssetType6 = dataRow["AssetType6"].ToString(),
                                AssetSerialNo6 = dataRow["AssetSerialNo6"].ToString(),
                                AssetType7 = dataRow["AssetType7"].ToString(),
                                AssetSerialNo7 = dataRow["AssetSerialNo7"].ToString(),
                                Manufacturer = dataRow["Manufacturer"].ToString(),
                                Model = dataRow["Model"].ToString(),
                                BarCode = dataRow["BarCode"].ToString(),
                                RAM = dataRow["RAM"].ToString(),
                                WarrantyStartDate = string.IsNullOrWhiteSpace(dataRow["WarrantyStartDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dataRow["WarrantyStartDate"]),
                                WarrantyEndDate = string.IsNullOrWhiteSpace(dataRow["WarrantyEndDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dataRow["WarrantyEndDate"]),
                                VendorName = dataRow["VendorName"].ToString(),
                                PONumber = dataRow["PONumber"].ToString(),
                                PurchaseDate = string.IsNullOrWhiteSpace(dataRow["PurchaseDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dataRow["PurchaseDate"]),
                                InvoiceDate = string.IsNullOrWhiteSpace(dataRow["InvoiceDate"].ToString()) ? (DateTime?)null : Convert.ToDateTime(dataRow["InvoiceDate"]),
                                CreatedBy = cuserContext.EmpInfo.EmployeeName,
                                CreatedDate = DateTime.Now
                            };

                            assetList.Add(asset);
                        }
                    }

                    if (assetList.Count > 0)
                    {
                        using (var context = new HRMS_EntityFramework())
                        {
                            context.Assets.AddRange(assetList);
                            context.SaveChanges();

                            model.Message = "Assets imported successfully";
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

        [HttpPost]
        public ActionResult ExportSelectedAssets(List<int> selectedAssetIds)
        {
            // Fetch the selected assets from the database
            var assets = _dbContext.Assets
                    .Where(a => selectedAssetIds.Contains(a.SNo))
                    .ToList();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Assets");

                // Add header row
                worksheet.Cells[1, 1].Value = "Asset ID";
                worksheet.Cells[1, 2].Value = "Allocated Employee Name";
                worksheet.Cells[1, 3].Value = "Allocated Employee ID";
                worksheet.Cells[1, 4].Value = "Location";
                worksheet.Cells[1, 5].Value = "Allocated Status";
                worksheet.Cells[1, 6].Value = "Assigned By";
                worksheet.Cells[1, 7].Value = "Assigned By Employee ID";
                worksheet.Cells[1, 8].Value = "Assigned Date";
                worksheet.Cells[1, 9].Value = "Remarks";
                worksheet.Cells[1, 10].Value = "Asset Type 1";
                worksheet.Cells[1, 11].Value = "Asset Serial No 1";
                worksheet.Cells[1, 12].Value = "Asset Type 2";
                worksheet.Cells[1, 13].Value = "Asset Serial No 2";
                worksheet.Cells[1, 14].Value = "Asset Type 3";
                worksheet.Cells[1, 15].Value = "Asset Serial No 3";
                worksheet.Cells[1, 16].Value = "Asset Type 4";
                worksheet.Cells[1, 17].Value = "Asset Serial No 4";
                worksheet.Cells[1, 18].Value = "Asset Type 5";
                worksheet.Cells[1, 19].Value = "Asset Serial No 5";
                worksheet.Cells[1, 20].Value = "Asset Type 6";
                worksheet.Cells[1, 21].Value = "Asset Serial No 6";
                worksheet.Cells[1, 22].Value = "Asset Type 7";
                worksheet.Cells[1, 23].Value = "Asset Serial No 7";
                worksheet.Cells[1, 24].Value = "Manufacturer";
                worksheet.Cells[1, 25].Value = "Model";
                worksheet.Cells[1, 26].Value = "Bar Code";
                worksheet.Cells[1, 27].Value = "RAM";
                worksheet.Cells[1, 28].Value = "Warranty Start Date";
                worksheet.Cells[1, 29].Value = "Warranty End Date";
                worksheet.Cells[1, 30].Value = "Vendor Name";
                worksheet.Cells[1, 31].Value = "PO Number";
                worksheet.Cells[1, 32].Value = "Purchase Date";
                worksheet.Cells[1, 33].Value = "Invoice Date";

                // Styling the header row
                using (var range = worksheet.Cells[1, 1, 1, 33])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.Font.Bold = true;
                }

                // Add data rows
                for (int i = 0; i < assets.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = assets[i].AssetID;
                    worksheet.Cells[i + 2, 2].Value = assets[i].AllocatedEmployeeName;
                    worksheet.Cells[i + 2, 3].Value = assets[i].AllocatedEmployeeID;
                    worksheet.Cells[i + 2, 4].Value = assets[i].Location;
                    worksheet.Cells[i + 2, 5].Value = assets[i].AllocatedStatus;
                    worksheet.Cells[i + 2, 6].Value = assets[i].AssignedBy;
                    worksheet.Cells[i + 2, 7].Value = assets[i].AssignedByEmpID;
                    worksheet.Cells[i + 2, 8].Value = assets[i].AssignedDate.HasValue ? assets[i].AssignedDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    worksheet.Cells[i + 2, 9].Value = assets[i].Remarks;
                    worksheet.Cells[i + 2, 10].Value = assets[i].AssetType1;
                    worksheet.Cells[i + 2, 11].Value = assets[i].AssetSerialNo1;
                    worksheet.Cells[i + 2, 12].Value = assets[i].AssetType2;
                    worksheet.Cells[i + 2, 13].Value = assets[i].AssetSerialNo2;
                    worksheet.Cells[i + 2, 14].Value = assets[i].AssetType3;
                    worksheet.Cells[i + 2, 15].Value = assets[i].AssetSerialNo3;
                    worksheet.Cells[i + 2, 16].Value = assets[i].AssetType4;
                    worksheet.Cells[i + 2, 17].Value = assets[i].AssetSerialNo4;
                    worksheet.Cells[i + 2, 18].Value = assets[i].AssetType5;
                    worksheet.Cells[i + 2, 19].Value = assets[i].AssetSerialNo5;
                    worksheet.Cells[i + 2, 20].Value = assets[i].AssetType6;
                    worksheet.Cells[i + 2, 21].Value = assets[i].AssetSerialNo6;
                    worksheet.Cells[i + 2, 22].Value = assets[i].AssetType7;
                    worksheet.Cells[i + 2, 23].Value = assets[i].AssetSerialNo7;
                    worksheet.Cells[i + 2, 24].Value = assets[i].Manufacturer;
                    worksheet.Cells[i + 2, 25].Value = assets[i].Model;
                    worksheet.Cells[i + 2, 26].Value = assets[i].BarCode;
                    worksheet.Cells[i + 2, 27].Value = assets[i].RAM;
                    worksheet.Cells[i + 2, 28].Value = assets[i].WarrantyStartDate.HasValue ? assets[i].WarrantyStartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    worksheet.Cells[i + 2, 29].Value = assets[i].WarrantyEndDate.HasValue ? assets[i].WarrantyEndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    worksheet.Cells[i + 2, 30].Value = assets[i].VendorName;
                    worksheet.Cells[i + 2, 31].Value = assets[i].PONumber;
                    worksheet.Cells[i + 2, 32].Value = assets[i].PurchaseDate.HasValue ? assets[i].PurchaseDate.Value.ToString("yyyy-MM-dd") : string.Empty; // Updated line
                    worksheet.Cells[i + 2, 33].Value = assets[i].InvoiceDate.HasValue ? assets[i].InvoiceDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                }


                // Return the generated Excel file
                var stream = new MemoryStream();
                excelPackage.SaveAs(stream);
                stream.Position = 0; // Reset the stream position to the beginning

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SelectedAssets.xlsx");
            }
        }


    }
}