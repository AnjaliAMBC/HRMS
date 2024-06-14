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
    }
}