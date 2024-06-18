using System;
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
        public string hourPermission { get; set; }
        public JsonResponse jsonResponse { get; set; } = new JsonResponse();
        public string Location { get; set; }
        public string OfficalEmailid { get; set; }
        public string ActionType { get; set; }
        public string EditRequestName { get; set; }
    }

    public class DayTypeEntry
    {
        public string Date { get; set; }
        public string DayType { get; set; }
        public string HalfType { get; set; }
    }

    public class ApplyLeaveViewModel
    {
        public List<emp_info> employees { get; set; } = new List<emp_info>();
        public con_leaveupdate EditLeaveInfo { get; set; } = new con_leaveupdate();
        public string Leaves { get; set; }

        public List<string> leaveTypes = new List<string>();

    }


    public class AvailableLeaves
    {
        public string Type { get; set; }
        public decimal Available { get; set; }
        public decimal Booked { get; set; }
        public decimal Balance { get; set; }
        public string ColorCode { get; set; }

        public string DashBoardColorCode { get; set; }
        public string ShortName { get; set; }

    }

    public class LeaveEmployee
    {
        public emp_info empInfo { get; set; } = new emp_info();
        public List<AvailableLeaves> AvailableLeaves { get; set; } = new List<AvailableLeaves>();
    }

    public class LeaveTypesBasedOnEmpViewModel
    {
        public List<LeaveEmployee> LeaveTypes { get; set; } = new List<LeaveEmployee>();

    }

    public class LeavesCategory
    {
        public string Type { get; set; }
        public string Colrocode { get; set; }
        public string DashBoardColorCode { get; set; }
        public string ShortName { get; set; }

    }


}