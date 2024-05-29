using System;
using System.Collections.Generic;

namespace HRMS.Models.Admin
{
    public class AddShiftModel : SiteContextModel
    {
        public List<DropdownItem> Departments { get; set; } = new List<DropdownItem>();
        public List<emp_info> Employees { get; set; } = new List<emp_info>();
    }

    public class AjaxShiftUpdateModel
    {
        public List<string> selectedIds { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public bool notification { get; set; }
        public bool IsDepartmentBasedUpdate { get; set; } = false;

    }

    public class Shift
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}