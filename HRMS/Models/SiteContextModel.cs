using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class SiteContextModel
    {
        public emplogin LoginInfo { get; set; } = new emplogin();
        public emp_info EmpInfo { get; set; } = new emp_info();
        //public tbld_ambclogininformation CheckInInfo { get; set; } = new tbld_ambclogininformation();
        public bool IsAdmin { get; set; } = false;
        public bool IsSuperAdmin { get; set; } = false;

        public bool IsAccountAdmin { get; set; } = false;
        public bool IsITAdmin { get; set; } = false;
        public List<EmployeeBasedClient> EmpBasedClients { get; set; } = new List<EmployeeBasedClient>();
        //public CheckInDetails empLastDayCheckInDetails { get; set; } = new CheckInDetails();
    }

    public class CheckInDetails
    {
        public DateTime SignInTime { get; set; }
        public DateTime SignOutTime { get; set; }
        public bool IsSignedOutOnLastCheckInDate { get; set; } = false;
        public int LoginID { get; set; }
    }
}