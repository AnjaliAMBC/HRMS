//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeCheckin
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeStatus { get; set; }
        public string OfficalEmailid { get; set; }
        public System.DateTime Signin_Time { get; set; }
        public Nullable<System.DateTime> Signout_Time { get; set; }
        public Nullable<int> Working_Hours { get; set; }
        public string Employee_Shift { get; set; }
        public System.DateTime Login_date { get; set; }
        public string imagepath { get; set; }
        public string Location { get; set; }
    }
}