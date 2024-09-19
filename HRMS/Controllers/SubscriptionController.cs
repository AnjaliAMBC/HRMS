using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Models;
using System.IO;
using System.Configuration;
using OfficeOpenXml;
using System.Data.Entity;
using HRMS.Helpers;
using HRMS.Models.Employee;

namespace HRMS.Controllers
{
    using static HRMS.Helpers.PartialViewHelper;
    public class SubscriptionController : Controller
    {
        
        private readonly HRMS_EntityFramework _dbContext;

        public SubscriptionController()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public ActionResult AddSubscription(int? subid)
        {
            var model = new SubscriptionViewModel();

            model.ITDeptEmployees = _dbContext.emp_info.Where(x => x.Department == "IT").ToList();

            model.Headline= "Add Subscription";
            if (subid.HasValue)
            {
                model.Headline = "Edit Subscription";
                model.Editsubscription = _dbContext.Subscriptions
                                                   .FirstOrDefault(x => x.SubscriptionID == subid.Value);

                if (model.Editsubscription == null)
                {
                    return HttpNotFound();
                }
            }        
            
            var lastsubscriptionId = _dbContext.Subscriptions
                                                   .OrderByDescending(x => x.SubscriptionID)
                                                   .FirstOrDefault();

            if (lastsubscriptionId != null)            {
                
                model.NewSubscriptionId = "S#" + (lastsubscriptionId.SubscriptionID + 1);
                           }
            else
            {                
                model.NewSubscriptionId = "S#1";
            }

            return View("~/Views/Itsupport/AddSubscription.cshtml", model);
        }

        public JsonResult AddUpdateSubscription()
        {
            try
            {                
                var subscriptionName = Request.Form["SubscriptionName"];
                HttpPostedFileBase file = Request.Files["SubscriptionLogo"];
                var category = Request.Form["SubscriptionCategory"];
                var purchaseDate = Convert.ToDateTime(Request.Form["SubscriptionPurchasedate"]);
                var renewalDate = DateTime.Parse(Request.Form["SubscriptionRenewaldate"]);
                var license = Request.Form["SubscriptionLicense"];
                var amount = !string.IsNullOrWhiteSpace(Request.Form["SubscriptionAmount"]) ? System.Convert.ToDecimal(Request.Form["SubscriptionAmount"]) : 0;
                var paymentMethod = Request.Form["SubscriptionPaymentMethod"];
                var remarks = Request.Form["SubscriptionRemarks"];
                var createdBy = Request.Form["SubscriptionAddedBy"];
                var createdDate = Convert.ToDateTime(Request.Form["SubscriptionAddeddate"]);               
                var subscriptionID = !string.IsNullOrWhiteSpace(Request.Form["EditRecordID"]) ? System.Convert.ToInt32(Request.Form["EditRecordID"]) : 0;

                // Handle file upload
                string logoFilePath = null;
                if (file != null && file.ContentLength > 0)
                {
                    if ((file.ContentType == "image/jpeg" || file.ContentType == "image/png") && file.ContentLength <= 2097152)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var TicketingFolderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
                        var subscriptionFolderPath = Path.Combine(TicketingFolderPath, "Subscription");
                        logoFilePath = Path.Combine(subscriptionFolderPath, fileName);
                        file.SaveAs(logoFilePath);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid file type or size." });
                    }
                }

                Subscription subscription;
                if (subscriptionID == 0)
                {
                    subscription = new Subscription
                    {
                        SubscriptionName = subscriptionName,
                        SubscriptionLogo = logoFilePath,
                        Category = category,
                        PurchaseDate = purchaseDate,
                        RenewalDate = renewalDate,
                        License = license,
                        Amount = amount,
                        PaymentMethod = paymentMethod,
                        Remarks = remarks,
                        CreatedBy = createdBy,
                        CreatedDate = createdDate
                    };

                    _dbContext.Subscriptions.Add(subscription);
                }
                else
                {
                    subscription = _dbContext.Subscriptions.FirstOrDefault(x => x.SubscriptionID == subscriptionID);
                    if (subscription == null)
                    {
                        return Json(new { success = false, message = "Subscription not found." });
                    }
                    var subscriptionHistory = new SubscriptionHistory
                    {
                        SubscriptionID = subscription.SubscriptionID,
                        SubscriptionName = subscription.SubscriptionName,
                        SubscriptionLogo = subscription.SubscriptionLogo,
                        Category = subscription.Category,
                        PurchaseDate = subscription.PurchaseDate,
                        RenewalDate = subscription.RenewalDate,
                        License = subscription.License,
                        Amount = subscription.Amount,
                        PaymentMethod = subscription.PaymentMethod,
                        Remarks = subscription.Remarks,
                        CreatedBy = subscription.CreatedBy,
                        CreatedDate = subscription.CreatedDate,
                    };

                    _dbContext.SubscriptionHistories.Add(subscriptionHistory);
                    subscription.PurchaseDate = purchaseDate;
                    subscription.RenewalDate = renewalDate;
                    subscription.License = license;
                    subscription.Amount = amount;
                    subscription.PaymentMethod = paymentMethod;
                    subscription.Remarks = remarks;
                    subscription.CreatedBy = createdBy;
                    subscription.CreatedDate = createdDate;
                    _dbContext.Entry(subscription).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();

                return Json(new { success = true, message = "Subscription processed successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult DeleteSubscription(int id)
        {
            var subscription = _dbContext.Subscriptions.FirstOrDefault(s => s.SubscriptionID == id);
            if (subscription != null)
            {
                _dbContext.Subscriptions.Remove(subscription);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Subscription deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Subscription not found." });
            }
        }
       
        public ActionResult SubscriptionInfo()
        {
            var model = new SubscriptionViewModel();

            var subscriptions = _dbContext.Subscriptions.ToList();

            foreach (var subscription in subscriptions)
            {
                var daysUntilRenewal = (subscription.RenewalDate.HasValue)
                    ? (subscription.RenewalDate.Value - DateTime.Now).Days
                    : 0;

                subscription.DaysUntilRenewal = daysUntilRenewal;

                if (daysUntilRenewal < 0)
                {
                    subscription.SubscriptionStatus = "Expired";
                    subscription.SubscriptionStatusClass = "subscriptioninfo-list-expired";
                }
                else if (daysUntilRenewal <= 30)
                {
                    subscription.SubscriptionStatus = $"Due in {daysUntilRenewal} Days";
                    subscription.SubscriptionStatusClass = "subscriptioninfo-list-due";


                    if (daysUntilRenewal <= 15 && (subscription.EmailSendBool == false))
                    {
                        var emailBody = RenderPartialToString(this, "_SubscriptionReminderEmail", subscription, ViewData, TempData);

                        var emailRequest = new EmailRequest()
                        {
                            Body = emailBody,
                            ToEmail = ConfigurationManager.AppSettings["SubscriptionEmailsTo"], 
                            Subject = "Subscription Renewal Reminder of " + subscription.SubscriptionName + " ",
                        };

                        EMailHelper.SendEmail(emailRequest);

                        subscription.EmailSendBool = true;

                        _dbContext.Entry(subscription).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }
            }

            model.Subscriptions = subscriptions.OrderByDescending(x => x.SubscriptionID).ToList();

            return View("~/Views/Itsupport/SubscriptionInfo.cshtml", model);
        }

        public ActionResult SubscriptionHistory(int id)
        {
            // Retrieve the current subscription based on ID
            var subscription = _dbContext.Subscriptions.Find(id);

            // Retrieve all history records for the given subscription ID
            var subscriptionHistories = _dbContext.SubscriptionHistories
                .Where(s => s.SubscriptionID == id)
                .OrderByDescending(s => s.RecordeupdatedDate) // Order by the most recent update
                .ToList();

            // Identify the most recently updated record
            var latestHistory = subscriptionHistories.FirstOrDefault();

            // Create the view model
            var model = new SubscriptionDetailsViewModel
            {
                CurrentSubscription = subscription,
                History = subscriptionHistories,
                LatestHistory = latestHistory 
            };

            return View("~/Views/Itsupport/SubscriptionHistory.cshtml", model);
        }


        [HttpPost]
        public ActionResult ExportSubscriptionsToExcel(List<int> subscriptionIds)
        {
            if (subscriptionIds == null || subscriptionIds.Count == 0)
            {
                return new HttpStatusCodeResult(400, "No subscriptions selected.");
            }

            var subscriptions = _dbContext.Subscriptions
        .AsNoTracking() // This disables caching for this query
        .Where(s => subscriptionIds.Contains(s.SubscriptionID))
        .ToList();


            if (!subscriptions.Any())
            {
                throw new Exception("No subscriptions found with the provided IDs.");
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Subscriptions");

                ws.Cells[1, 1].Value = "SubscriptionID";
                ws.Cells[1, 2].Value = "Subscription Name";
                ws.Cells[1, 3].Value = "Purchase Date";
                ws.Cells[1, 4].Value = "Renewal Date";
                ws.Cells[1, 5].Value = "Amount";
                ws.Cells[1, 6].Value = "Category";
                ws.Cells[1, 7].Value = "Payment Method";
                ws.Cells[1, 8].Value = "Created By";
                ws.Cells[1, 9].Value = "Created Date";


                for (int i = 0; i < subscriptions.Count; i++)
                {
                    var subscription = subscriptions[i];
                    ws.Cells[i + 2, 1].Value = subscription.SubscriptiionNumber;
                    ws.Cells[i + 2, 2].Value = subscription.SubscriptionName;
                    ws.Cells[i + 2, 3].Value = subscription.PurchaseDate;
                    ws.Cells[i + 2, 4].Value = subscription.RenewalDate;
                    ws.Cells[i + 2, 5].Value = subscription.Amount;
                    ws.Cells[i + 2, 6].Value = subscription.Category;
                    ws.Cells[i + 2, 7].Value = subscription.PaymentMethod;
                    ws.Cells[i + 2, 8].Value = subscription.CreatedBy;
                    ws.Cells[i + 2, 9].Value = subscription.CreatedDate;
                    // Add more fields as per your data
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelFileName = "Subscriptions_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
            }
        }
    }
}