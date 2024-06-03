using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class LeaveRequestModel
    {
        public int LeaveType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<DayTypeEntry> DayTypeEntries { get; set; }
        public string TeamEmail { get; set; }
        public string Reason { get; set; }
    }

    public class DayTypeEntry
    {
        public string Date { get; set; }
        public string DayType { get; set; }
        public string HalfType { get; set; }
    }
}