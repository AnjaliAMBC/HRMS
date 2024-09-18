using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class SubscriptionModel
    {
       
    }

    public class SubscriptionViewModel
    {
        public int SubscriptionID { get; set; }
        public string SubscriptionName { get; set; }
        public HttpPostedFileBase SubscriptionLogo { get; set; }  // File upload
        public string SubscriptionCategory { get; set; }
        public string License { get; set; }
        public DateTime? SubscriptionPurchasedate { get; set; }
        public decimal? SubscriptionAmount { get; set; }  // Optional
        public DateTime? SubscriptionRenewaldate { get; set; }
        public string SubscriptionPaymentMethod { get; set; }  // Optional
        public string SubscriptionRemarks { get; set; }  // Optional
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int UniqueSubkey { get; set; }
        public string SubscriptionFrequency { get; set; }
        public DateTime SubscriptionRenewalDate { get; set; }
        public int DaysUntilRenewal { get; set; } 
        public string SubscriptionStatus { get; set; }

        public string SubscriptionStatusClass { get; set; }
        public string LastSubscriptionID { get; set; }

        public string NewSubscriptionId { get; set; }
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public List<emp_info> ITDeptEmployees { get; set; } = new List<emp_info>();
        public Subscription Editsubscription { get; set; } = new Subscription();
        
    }

    public class SubscriptionUpdateViewModel
    {
        public int SubscriptionID { get; set; }
        public string SubscriptionName { get; set; }
        public string SubscriptionLogo { get; set; }
        public string Category { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string License { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpadtedDate { get; set; }
    }

    public class SubscriptionDetailsViewModel
    {
        public Subscription CurrentSubscription { get; set; }

        // Represents the latest subscription history
        public SubscriptionHistory LatestHistory { get; set; }        
        public List<SubscriptionHistory> History { get; set; }
    }

}