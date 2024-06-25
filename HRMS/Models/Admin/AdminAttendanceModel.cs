using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class AdminAttendanceModel : SiteContextModel
    {
        public List<CheckInView> EmpCheckInList { get; set; } = new List<CheckInView>();
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedEndDate { get; set; }
        public List<emp_info> AllEmployees { get; set; } = new List<emp_info>();

        public List<con_leaveupdate> Leaves { get; set; } = new List<con_leaveupdate>();
    }

    public class AdminEmpIndividualAttendanceModel : SiteContextModel
    {
        public List<CheckInView> EmpCheckInList { get; set; } = new List<CheckInView>();
        public DateTime SelectedStartDate { get; set; }
        public DateTime SelectedEndDate { get; set; }
        public emp_info SelectedEmployee { get; set; } = new emp_info();

        public List<DateTime> AllDates { get; set; } = new List<DateTime>();
    }
    public class EmployeeCheckInUpdateModel
    {
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string loginID { get; set; }

    }

    public class EmployeeRecord
    {
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string Year { get; set; }
        public decimal Earned { get; set; }
        public decimal Emergency { get; set; }
        public decimal Sick { get; set; }
        public decimal Bereavement { get; set; }
        public decimal HourlyPermission { get; set; }
        public decimal Marriage { get; set; }
        public decimal Maternity { get; set; }
        public decimal Paternity { get; set; }
        public decimal CompOff { get; set; }
    }
}