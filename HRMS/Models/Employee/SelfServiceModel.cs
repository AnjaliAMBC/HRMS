namespace HRMS.Models.Employee
{
    public class SelfServiceViewModel : SiteContextModel
    {
    }

    public class SelfServiceEmpImageModel : SiteContextModel
    {
        public string ImageURl { get; set; }
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}