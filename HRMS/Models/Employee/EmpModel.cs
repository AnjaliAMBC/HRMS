using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class EmpModel
    {
    }

    public class EmployeeDisplayModel
    {
        public string ImageSrc { get; set; }
        public string EmpShortName { get; set; }
        public emp_info EmpInfo { get; set; } = new emp_info();
    }
}