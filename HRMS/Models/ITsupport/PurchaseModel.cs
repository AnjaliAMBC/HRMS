using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class PurchaseViewModel
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

        public int LastePurchaseId { get; set; }

        public int NewPurchaseId { get; set; }
        public List<VendorType> Assettypes { get; set; } = new List<VendorType>();

        public List<VendorList> allVendors { get; set; } = new List<VendorList>();

        public PurchaseRequest EditPurchase { get; set; } = new PurchaseRequest();

        public List<emp_info> ITDeptEmployees { get; set; } = new List<emp_info>();

       
    }

}