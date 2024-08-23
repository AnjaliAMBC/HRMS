using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class VendorViewModel
    {
        public List<VendorList> Allvendors { get; set; } = new List<VendorList>();

        public JsonResponse JsonResponse { get; set; } = new JsonResponse();

        public JsonResponse NewVendorEmailResponse { get; set; } = new JsonResponse();

        public List<VendorType> vendorTypes { get; set; } = new List<VendorType>();

        public string HeadLine { get; set; }

        public bool IsAddAction { get; set; } = false;

        public string LastCreatedVendorID { get; set; }

        public List<emp_info> ITDeptEmployees { get; set; } = new List<emp_info>();

        public VendorList EditVendor { get; set; } = new VendorList();

    }

        public class VendorTypeDropdown
    {
        public string Id { get; set; }
        public string VendorType { get; set; }
    }

    public class VendorEmailRequest
    {
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }

        public string BCCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string FromEmail { get; set; }

        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}