using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class AdminAttendanceModel : SiteContextModel
    {
        public List<EmployeeCheckin> EmpCheckInList { get; set; } = new List<EmployeeCheckin>();
        public DateTime SelectedDate { get; set; }
        public List<emp_info> AllEmployees { get; set; } = new List<emp_info>();
    }
}