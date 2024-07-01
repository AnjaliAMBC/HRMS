using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class AdminLeaveManagementModel : SiteContextModel
    {
        public List<con_leaveupdate> LeavesInfo = new List<con_leaveupdate>();
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedEndDate { get; set; }

        public List<LeaveInfo> LeavesInfoBasedOnFromAndTodate = new List<LeaveInfo>();
    }


    public class AdminLeaveBalanceUpdateModel : SiteContextModel
    {
        public List<emp_info> Employees = new List<emp_info>();

        public LeaveBalance EmpLeaveBalance = new LeaveBalance();
        public List<AvailableLeaves> AvailableLeaves { get; set; } = new List<AvailableLeaves>();
    }

    public class LeaveBalanceExport
    {
        public emp_info Employees = new emp_info();

        public LeaveBalance EmpLeaveBalance = new LeaveBalance();
        public List<AvailableLeaves> AvailableLeaves { get; set; } = new List<AvailableLeaves>();
    }

    public class AdminLeaveHistoryViewModel
    {
        public List<LeaveInfo> AllEMployeeLeaves = new List<LeaveInfo>();

        public List<emp_info> Employees = new List<emp_info>();

        public List<DropdownItem> Departments = new List<DropdownItem>();

    }

    public class AdminLeaveEmpCalenderViewModel
    {

        public List<emp_info> Employees = new List<emp_info>();
    }
}