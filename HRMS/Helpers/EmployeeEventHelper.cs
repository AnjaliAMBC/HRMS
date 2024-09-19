using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    using static HRMS.Models.EmployeeEvent;
    public class EmployeeEventHelper
    {
        // GET: AdminDash
        private readonly HRMS_EntityFramework _dbContext;

        // Constructor to initialize database context
        public EmployeeEventHelper()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }

        public List<AnniversaryModel> Anniversary()
        {
            var today = DateTime.Today;
            var anniversaries = _dbContext.emp_info.Where(e => e.DOJ.Month == today.Month && e.DOJ.Day == today.Day).Select(e => new AnniversaryModel
            {
                EmpName = e.EmployeeName,
                Designation = e.Designation,
                imagePath = e.imagepath,
                years = today.Year - e.DOJ.Year,
                EmpEmail = e.OfficalEmailid,
            }).ToList();

            return anniversaries;
        }
        public List<BirthdayModel> Birthday()
        {
            var today = DateTime.Today;
            var birthdays = _dbContext.emp_info.Where(e => e.DOB.Month == today.Month && e.DOB.Day == today.Day).Select(e => new BirthdayModel
            {
                EmpName = e.EmployeeName,
                imagePath = e.imagepath,
                Designation = e.Designation,
                EmpEmail = e.OfficalEmailid
            }).ToList();

            return birthdays;
        }
        public List<UpcomingHoliday> GetUpcomingHolidays(string empLocation)
        {
            if (!string.IsNullOrEmpty(empLocation))
            {
                empLocation = empLocation.ToLowerInvariant();
            }

            var today = empLocation == string.Empty ? new DateTime(DateTime.Now.Year, 1, 1) : DateTime.Today;
            var query = _dbContext.tblambcholidays
                .Where(h => h.holiday_date >= today && h.region.ToLower().Contains(empLocation));


            var upcomingHolidays = query
                .OrderBy(h => h.holiday_date)
                .Select(h => new UpcomingHoliday
                {
                    HolidayDate = (DateTime)h.holiday_date,
                    HolidayName = h.holiday_name,
                    Region = h.region,
                    HolidayNo = h.holidayno
                }).ToList();

            return upcomingHolidays;
        }

    }
}