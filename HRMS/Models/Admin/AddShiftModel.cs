using System.Collections.Generic;

namespace HRMS.Models.Admin
{
    public class AddShiftModel : SiteContextModel
    {
        public List<DropdownItem> Departments { get; set; } = new List<DropdownItem>();
        public List<emp_info> Employees { get; set; } = new List<emp_info>();
    }
}