using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class LoginViewModel
    {
        public int EmployeeID { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public bool StaySignedIn { get; set; }
        public string InvalidLoginMessage { get; set; }
        public bool IsValidUser { get; set; }
    }
    public class ForgotModel
    {
        public string ForgotEmailID { get; set; }
        public bool IsEmailSent { get; set; } = false;
    }

    public class ResetModel
    {
        public empinfo empInfo { get; set; }
    }

    public class UpdatePasswordModel
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Empid { get; set; }
        public bool IsPasswordReset { get; set; } = false;
        public string ResponseMessage { get; set; }
    }
}