using System.Collections.Generic;

namespace HRMS.Models.Employee
{
    public class SelfServiceViewModel : SiteContextModel
    {
        public Asset empAsset { get; set; } = new Asset(); // Changed to List
        public List<IT_Maintenance> empMaintanance { get; set; } = new List<IT_Maintenance>();
    }

    public class SelfServiceEmpImageModel : SiteContextModel
    {
        public string ImageURl { get; set; }
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}