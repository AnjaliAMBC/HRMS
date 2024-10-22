using System;

namespace HRMS.Models
{
    public class EmployeeEvent
    {
        public class AnniversaryModel
        {
            public string Empid { get; set; }
            public string EmpName { get; set; }
            public string EmpEmail { get; set; }
            public int years { get; set; }
            public string Designation { get; set; }
            public DateTime JoiningDate { get; set; }
            public string imagePath { get; set; }
            public emp_info EmpInfo { get; set; }
        }
        public class BirthdayModel
        {
            public string Empid { get; set; }
            public string EmpName { get; set; }
            public string Designation { get; set; }
            public DateTime BirthDate { get; set; }
            public string imagePath { get; set; }
            public string EmpEmail { get; set; }
            public emp_info EmpInfo { get; set; }
        }
        public class UpcomingHoliday
        {
            public int HolidayNo { get; set; }
            public DateTime HolidayDate { get; set; }
            public string HolidayName { get; set; }
            public string Region { get; set; }
        }
    }
}