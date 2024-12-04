using System.Collections.Generic;

namespace HRMS.Models.Employee
{
    public class SelfServiceViewModel : SiteContextModel
    {
        public Asset empAsset { get; set; } = new Asset(); // Changed to List
        public List<IT_Maintenance> empMaintanance { get; set; } = new List<IT_Maintenance>();

        public IT_Maintenance empmaintananceRecord = new IT_Maintenance();

        public List<emp_info> ITEmployees { get; set; } = new List<emp_info>();

        public IT_Maintenance EditableRecord = new IT_Maintenance();

        public List<EmployeeBasedClient> Empbasedclients { get; set; } = new List<EmployeeBasedClient>();

    }

    public class SelfServiceEmpImageModel : SiteContextModel
    {
        public string ImageURl { get; set; }
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}