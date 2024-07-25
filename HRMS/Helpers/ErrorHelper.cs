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
            var errorMessage = string.Empty;

            if (ex.InnerException != null)
            {
                errorMessage = ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message;
            }
            else
            {
                errorMessage = ex.Message;
            }

            // Check for primary key violation
            if (errorMessage.Contains("PRIMARY KEY constraint") && errorMessage.Contains("Cannot insert duplicate key"))
            {
                // Extract the key value causing the violation from the error message
                var duplicateKeyValue = errorMessage.Substring(errorMessage.IndexOf('(') + 1, errorMessage.IndexOf(')') - errorMessage.IndexOf('(') - 1);
                jsonResponse.Message = $"A record with the same key value ({duplicateKeyValue}) already exists. Please use a unique key value.";
            }
            else
            {
                jsonResponse.Message = errorMessage;
            }

            jsonResponse.StatusCode = 500;

            return jsonResponse;
        }

    }
}