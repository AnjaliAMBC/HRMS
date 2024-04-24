namespace HRMS.Models
{
    public class JsonResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }
    }
}