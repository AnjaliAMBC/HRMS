using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class AccountModel
    {
        public string EmployeeID { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public bool StaySignedIn { get; set; }
        public string InvalidLoginMessage { get; set; }
        public bool IsValidUser { get; set; }
        public string GCaptcha { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string ForgotEmailID { get; set; }
        public bool IsEmailSent { get; set; } = false;
        public JsonResponse JsonResponse { get; set; }
    }

    public class ResetPasswordModel
    {
        public emplogin EmpInfo { get; set; }
    }

    public class UpdatePasswordModel
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Empid { get; set; }
        public bool IsPasswordReset { get; set; } = false;
        public string ResponseMessage { get; set; }
    }
}