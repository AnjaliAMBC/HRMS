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
    
    public partial class IT_Maintenance
    {
        public int Sno { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmailId { get; set; }
        public Nullable<System.DateTime> MaintenanceDate { get; set; }
        public Nullable<System.TimeSpan> TimeIn { get; set; }
        public Nullable<System.TimeSpan> TimeOut { get; set; }
        public Nullable<System.DateTime> RescheduleDate { get; set; }
        public string AgentName { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Acknowledge { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> AcknowledgeDate { get; set; }
        public string ProblemCategory { get; set; }
        public string IssueFacing { get; set; }
        public string NewAssetRequirement { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string AgentID { get; set; }
    }
}
