//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscriptionHistory
    {
        public int HistoryID { get; set; }
        public int SubscriptionID { get; set; }
        public string SubscriptiionNumber { get; set; }
        public string SubscriptionName { get; set; }
        public string SubscriptionLogo { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> RenewalDate { get; set; }
        public string Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Remarks { get; set; }
        public string SubscriptionStatus { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string License { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpadtedDate { get; set; }
        public Nullable<System.DateTime> RecordeupdatedDate { get; set; }
    }
}
