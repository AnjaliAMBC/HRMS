using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Admin
{
    public class AdminDashView : SiteContextModel
    {
        public List<EmployeeEvent.AnniversaryModel> AnniversaryModel { get; set; } = new List<EmployeeEvent.AnniversaryModel>();
        public List<EmployeeEvent.BirthdayModel> BirthModel { get; set; } = new List<EmployeeEvent.BirthdayModel>();
        public List<EmployeeEvent.UpcomingHoliday> UpcomingHolidays { get; set; } = new List<EmployeeEvent.UpcomingHoliday>();
        public AdminLeaveManagementModel LeavesInfo { get; set; } = new AdminLeaveManagementModel();
        public List<emp_info> Employees { get; set; } = new List<emp_info>();

        public Dictionary<int, int> CompoffApplied = new Dictionary<int, int>();

        public List<LeaveInfo> AllEMployeeLeaves = new List<LeaveInfo>();

        public List<IT_Ticket> AllHRTickets = new List<IT_Ticket>();
    }  
}