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
    
    public partial class LeaveBalance
    {
        public int Sno { get; set; }
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string Year { get; set; }
        public Nullable<decimal> Earned { get; set; }
        public Nullable<decimal> Emergency { get; set; }
        public Nullable<decimal> Sick { get; set; }
        public Nullable<decimal> Bereavement { get; set; }
        public Nullable<decimal> HourlyPermission { get; set; }
        public Nullable<decimal> Marriage { get; set; }
        public Nullable<decimal> Maternity { get; set; }
        public Nullable<decimal> Paternity { get; set; }
        public Nullable<decimal> CompOff { get; set; }
    }
}
