﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class EmployeeManagementViewModel : SiteContextModel
    {
        public List<emp_info> EmpList { get; set; } = new List<emp_info>();
        public string EmpListJson { get; set; }
        public emp_info EditableEmpInfo { get; set; } = new emp_info();
        public List<DropdownItem> Departments { get; set; } = new List<DropdownItem>();
    }
}