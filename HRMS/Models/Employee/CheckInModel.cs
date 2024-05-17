using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class CheckInOutModel
    {
        public tbld_ambclogininformation CheckInInfo { get; set; } = new tbld_ambclogininformation();
        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}