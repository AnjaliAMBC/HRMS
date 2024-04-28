using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class ErrorHelper
    {
        public static JsonResponse CaptureError(Exception ex)
        {
            var jsonResponse = new JsonResponse();
            if (ex.InnerException != null)
            {
                jsonResponse.Message = ex.InnerException.Message;
            }
            else
            {
                jsonResponse.Message = ex.Message;
            }

            jsonResponse.StatusCode = 500;

            return jsonResponse;
        }
    }
}