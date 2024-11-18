using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Services
{
    public class LeaveSummary
    {
        public int TotalLeaves { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }

        public int TotalHolidays { get; set; }
    }

    public class LeaveService
    {
        private readonly HRMS_EntityFramework _context;

        public LeaveService(HRMS_EntityFramework context)
        {
            _context = context;
        }

        public string GetLeaveSummary(string employeeId, string location, int month, int year)
        {
            var logins = _context.tbld_ambclogininformation
                .Where(l => l.Employee_Code == employeeId.ToString() &&
                            l.Login_date.Month == month && l.Login_date.Year == year)
                .ToList();

            var leaves = _context.con_leaveupdate
                .Where(l => l.employee_id == employeeId &&
                            l.Location == location &&
                            l.leavedate.HasValue &&
                            l.leavedate.Value.Month == month &&
                            l.leavedate.Value.Year == year)
                .ToList();

            var holidays = _context.tblambcholidays
                .Where(h => h.holiday_date.HasValue &&
                            h.holiday_date.Value.Month == month &&
                            h.holiday_date.Value.Year == year &&
                            h.region.Contains(location))
                .ToList();

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var totalPresent = 0;
            var totalWeekdays = 0;
            var totalHolidays = 0;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);

                // Only consider weekdays
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalWeekdays++;

                    // Check if employee logged in
                    if (logins.Any(l => l.Login_date.Date == date))
                    {
                        totalPresent++;
                    }
                    // Check if it’s a leave day
                    else if (leaves.Any(l => l.leavedate.HasValue && l.leavedate.Value.Date == date))
                    {
                        // Leave day - skip further checks
                        continue;
                    }
                    // Check if it's a holiday
                    else if (holidays.Any(h => h.holiday_date.HasValue && h.holiday_date.Value.Date == date))
                    {
                        totalHolidays++;
                    }
                }
            }

            var totalLeaves = leaves.Count;
            var totalAbsent = totalWeekdays - (totalPresent + totalLeaves + totalHolidays);

            var summary = new LeaveSummary
            {
                TotalLeaves = totalLeaves,
                TotalPresent = totalPresent,
                TotalAbsent = totalAbsent,
                TotalHolidays = totalHolidays
            };

            return JsonConvert.SerializeObject(summary);
        }



    }
}