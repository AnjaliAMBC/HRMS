using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class PurchaseModel
    {
    }
    public class PurchaseRequest
    {
        public int PurchaseID { get; set; } 
        public string AssetType { get; set; } 
        public DateTime RequiredOn { get; set; } 
        public string RequestedBy { get; set; } 
        public string VendorName { get; set; } 
        public decimal QuotationPrice { get; set; } 
        public string AttachFile { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; } 
    }
}