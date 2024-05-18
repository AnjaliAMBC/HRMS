﻿using System;
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

    public class AdminEmpIndividualAttendanceModel : SiteContextModel
    {
        public List<EmployeeCheckin> EmpCheckInList { get; set; } = new List<EmployeeCheckin>();
        public DateTime SelectedStartDate { get; set; }
        public DateTime SelectedEndDate { get; set; }
        public emp_info SelectedEmployee { get; set; } = new emp_info();

        public List<DateTime> AllDates { get; set; } = new List<DateTime>();
    }


}