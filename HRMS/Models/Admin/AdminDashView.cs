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
    }
    //public class AnniversaryModel
    //{
    //    public string Empid { get; set; }
    //    public string EmpName { get; set; }
    //    public int years { get; set; }
    //    public string Designation { get; set; }
    //    public DateTime JoiningDate { get; set; }
    //    public string imagePath { get; set; }
    //}
    //public class BirthdayModel
    //{
    //    public string Empid { get; set; }
    //    public string EmpName { get; set; }
    //    public string Designation { get; set; }
    //    public DateTime BirthDate { get; set; }
    //    public string imagePath { get; set; }
    //}
    //public class UpcomingHoliday
    //{
    //    public int HolidayNo { get; set; }
    //    public DateTime HolidayDate { get; set; }
    //    public string HolidayName { get; set; }
    //    public string Region { get; set; }
    //}
}