using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class LeaveCalculator
    {
        private readonly HRMS_EntityFramework _dbContext;
        public LeaveCalculator()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public class Employee
        {
            public int EmployeeID { get; set; }
            public DateTime DOJ { get; set; }
        }

        public class Leave
        {
            public int LeaveNo { get; set; }
            public DateTime LeaveDate { get; set; }
            public int EmployeeID { get; set; }
            public int LeaveDays { get; set; }
        }

        public class AvailableLeaves
        {
            public decimal Available { get; set; }
            public decimal Booked { get; set; }
            public decimal Balance { get; set; }

        }

        public AvailableLeaves CalculateAvailableLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            if (IsProbationaryEmployee(employee))
            {
                availableLeaves = CalculateProbationaryLeaves(employee, leaves, leaveType);
            }
            else
            {
                availableLeaves = CalculatePermanentLeaves(employee, leaves, leaveType);
            }

            return availableLeaves;
        }

        private static bool IsProbationaryEmployee(emp_info employee)
        {
            var lastDayOfProbatation = employee.DOJ.AddMonths(3);
            return lastDayOfProbatation >= DateTime.Today ? true : false;
        }

        private AvailableLeaves CalculateProbationaryLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            if (leaveType == "Sick Leave" || leaveType == "Emergency Leave" || leaveType == "Bereavement Leave")
            {
                Decimal totalSickLeaves = 3;

                DateTime december31st = new DateTime(DateTime.Today.Year, 12, 31);
                DateTime probationEndDate = employee.DOJ.AddMonths(3);

                Decimal totalEmergencyLeaves = 0;
                Decimal totalBereavementLeaves = 3;
                int monthDifference = (december31st.Year - employee.DOJ.Year) * 12 + december31st.Month - employee.DOJ.Month + (december31st.Day >= employee.DOJ.Day ? 1 : 0);

                totalEmergencyLeaves = (int)(monthDifference / 3) + ((monthDifference % 3 > 0) ? 1 : 0);

                var selectedLeaveTypeTaken = _dbContext.con_leaveupdate.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == leaveType && l.leavedate >= employee.DOJ && l.leavedate <= probationEndDate).ToList();
                decimal totalLeaveDays = selectedLeaveTypeTaken.Sum(l => l.LeaveDays);

                if (leaveType == "Sick Leave")
                {
                    availableLeaves.Available = totalSickLeaves;
                }

                if (leaveType == "Emergency Leave")
                {
                    availableLeaves.Available = totalEmergencyLeaves;
                }

                if (leaveType == "Bereavement Leave")
                {
                    availableLeaves.Available = totalBereavementLeaves;
                }
                availableLeaves.Booked = totalLeaveDays;
                availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
            }
            return availableLeaves;
        }

        private AvailableLeaves CalculatePermanentLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            var isEmployeeProbationInThisCurrentYear = employee.DOJ.AddMonths(3);

            DateTime startDate = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endDate = new DateTime(DateTime.Today.Year, 12, 31);

            if (leaveType != "Bereavement Leave" && leaveType != "Emergency Leave")
            {
                if (isEmployeeProbationInThisCurrentYear.Year == DateTime.Today.Year)
                {
                    startDate = isEmployeeProbationInThisCurrentYear;
                }
            }

            if (leaveType == "Hourly Permission")
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            var startMonthNumber = startDate.Month;
            var endMonthNumber = endDate.Month;

            if(startMonthNumber == 1)
            {
                startMonthNumber = 0;
            }

            int monthDifference = endMonthNumber - startMonthNumber;

            //int monthDifference = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + (endDate.Day >= startDate.Day ? 1 : 0);
            decimal earnedLeavesPerMonth = monthDifference * 1;
            decimal totalSickLeaves = (decimal)monthDifference * (decimal)0.5;
            decimal totalEmergencyLeaves = (int)(monthDifference / 3) + ((monthDifference % 3 > 0) ? 1 : 0);

            var selectedLeaveTypeTaken = _dbContext.con_leaveupdate.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == leaveType && l.leavedate >= startDate && l.leavedate <= endDate).ToList();
            decimal totalLeaveDays = selectedLeaveTypeTaken.Sum(l => l.LeaveDays);

            if (leaveType == "Sick Leave")
            {
                availableLeaves.Available = totalSickLeaves;
            }

            if (leaveType == "Emergency Leave")
            {
                availableLeaves.Available = totalEmergencyLeaves;
            }

            if (leaveType == "Bereavement Leave")
            {
                availableLeaves.Available = 3;
            }

            if (leaveType == "Earned Leave")
            {
                availableLeaves.Available = earnedLeavesPerMonth;
            }

            if (leaveType == "Marriage Leave")
            {
                availableLeaves.Available = employee.MaritalStatus == "Single" ? 10 : 0;
            }

            if (leaveType == "Maternity Leave")
            {
                availableLeaves.Available = (employee.Gender == "Female" && employee.MaritalStatus == "Married") ? 180 : 0;
            }

            if (leaveType == "Paternity Leave")
            {
                availableLeaves.Available = (employee.Gender == "Male" && employee.MaritalStatus == "Married") ? 5 : 0;
            }

            if (leaveType == "Comp Off")
            {
                availableLeaves.Available = 5;
            }

            if (leaveType == "Hourly Permission")
            {
                availableLeaves.Available = 2;
                if (selectedLeaveTypeTaken != null && selectedLeaveTypeTaken.Count() > 0)
                {
                    availableLeaves.Booked = totalLeaveDays;
                    availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
                }
                else
                {
                    availableLeaves.Booked = 0;
                    availableLeaves.Balance = 2;
                }
                return availableLeaves;
            }

            availableLeaves.Booked = totalLeaveDays;
            availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
            return availableLeaves;
        }
    }
}