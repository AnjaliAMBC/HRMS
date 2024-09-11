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
        public DateTime? SubscriptionPurchasedate { get; set; }
        public decimal? SubscriptionAmount { get; set; }  // Optional
        public DateTime? SubscriptionRenewaldate { get; set; }
        public string SubscriptionPaymentMethod { get; set; }  // Optional
        public string SubscriptionRemarks { get; set; }  // Optional
        public string SubscriptionAddedBy { get; set; }
        public DateTime? SubscriptionAddeddate { get; set; }
    }

}