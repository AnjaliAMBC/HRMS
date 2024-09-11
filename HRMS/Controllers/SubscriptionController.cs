using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMS.Models;
using System.IO;
using System.Configuration;

namespace HRMS.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly HRMS_EntityFramework _dbContext;

        public SubscriptionController()
        {
            _dbContext = new HRMS_EntityFramework();
        }
                
        public ActionResult AddSubscription()
        {
            return View("~/Views/Itsupport/AddSubscription.cshtml");
        }

        [HttpPost]
        public ActionResult AddSubscription(SubscriptionViewModel model)
        {
            // Ensure model is not null or corrupted
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid subscription data." });
            }

            try
            {
                // Create a new subscription object
                var subscription = new Subscription
                {
                    SubscriptionID = model.SubscriptionID,
                    SubscriptionName = model.SubscriptionName,
                    SubscriptionLogo = model.SubscriptionLogo != null ? SaveFile(model.SubscriptionLogo) : null,  
                    Category = model.SubscriptionCategory,
                    PurchaseDate = model.SubscriptionPurchasedate,
                    Amount = model.SubscriptionAmount,
                    RenewalDate = model.SubscriptionRenewaldate,
                    PaymentMethod = model.SubscriptionPaymentMethod,
                    Remarks = model.SubscriptionRemarks,
                    CreatedBy = model.SubscriptionAddedBy,
                    CreatedDate = model.SubscriptionAddeddate
                };

                _dbContext.Subscriptions.Add(subscription);
                _dbContext.SaveChanges();

                // Return success response
                return Json(new { success = true, message = "Subscription added successfully!" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

       
        private string SaveFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                return null;  
            }            
            var folderPath = ConfigurationManager.AppSettings["TicketingFolderPath"];
            
            var subfolderPath = Path.Combine(folderPath, "Subscription");
                        
            if (!Directory.Exists(Server.MapPath(subfolderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(subfolderPath));
            }
                        
            var filePath = Path.Combine(Server.MapPath(subfolderPath), Path.GetFileName(file.FileName));
                       
            file.SaveAs(filePath);
                        
            return Path.Combine("/Uploads/Subscription", Path.GetFileName(file.FileName));
        }


        public ActionResult SubscriptionInfo()
        {
            return View("~/Views/Itsupport/SubscriptionInfo.cshtml");
        }

        public ActionResult History()
        {
            return View("~/Views/Itsupport/SubscriptionHistory.cshtml");
        }
    }
}