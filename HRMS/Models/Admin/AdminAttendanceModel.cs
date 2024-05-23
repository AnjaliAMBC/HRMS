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
        public List<emp_info> AllEmployees { get; set; } = new List<emp_info>();
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
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
       
    }

}