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
    
    public partial class IT_Ticket
    {
        public int TicketNo { get; set; }
        public string TicketType { get; set; }
        public string Category { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string OfficialEmailID { get; set; }
        public string Description { get; set; }
        public string AttatchimageFile { get; set; }
        public string Status { get; set; }
        public string isacknowledge { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public Nullable<System.DateTime> Created_date { get; set; }
        public Nullable<System.DateTime> Closed_date { get; set; }
        public string Closedby { get; set; }
        public Nullable<System.TimeSpan> ResponseTime { get; set; }
        public string Location { get; set; }
    }
}
