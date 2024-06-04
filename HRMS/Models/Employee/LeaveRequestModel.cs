﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class LeaveRequestModel
    {
        public string LeaveType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<DayTypeEntry> DayTypeEntries { get; set; }
        public string TeamEmail { get; set; }
        public string Reason { get; set; }
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string SubmittedBy { get; set; }
        public string DayType { get; set; }
        public decimal LeaveDays { get; set; }
        public string HalfDayCategory { get; set; }
        public string BackupResource_Name { get; set; }
        public string EmergencyContact_no { get; set; }

    }

    public class DayTypeEntry
    {
        public string Date { get; set; }
        public string DayType { get; set; }
        public string HalfType { get; set; }
    }
}