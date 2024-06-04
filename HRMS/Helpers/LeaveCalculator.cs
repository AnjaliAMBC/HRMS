using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class LeaveCalculator
    {
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

        public static AvailableLeaves CalculateAvailableLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();
            //int availableLeaves = 0;

            // Check if the employee is on probation
            if (IsProbationaryEmployee(employee))
            {
                // Calculate leaves for probationary employees
                availableLeaves = CalculateProbationaryLeaves(employee, leaves, leaveType);
            }
            else
            {
                // Calculate leaves for permanent employees
                availableLeaves = CalculatePermanentLeaves(employee, leaves, leaveType);
            }

            return availableLeaves;
        }

        private static bool IsProbationaryEmployee(emp_info employee)
        {
            // Calculate months since joining
            int monthsSinceJoining = (DateTime.Today.Year - employee.DOJ.Year) * 12 + DateTime.Today.Month - employee.DOJ.Month;
            return monthsSinceJoining < 3; // Assuming probation period is 3 months
        }

        private static AvailableLeaves CalculateProbationaryLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            if (leaveType == "Sick Leave" || leaveType == "Emergency Leave" || leaveType == "Bereavement Leave")
            {
                // Calculate leaves for probationary employees
                Decimal totalSickLeaves = 3; // Total sick leaves for probationary employees
               
                DateTime december31st = new DateTime(DateTime.Today.Year, 12, 31);             
                DateTime probationEndDate = (employee.DOJ.Year == DateTime.Today.Year) ? december31st : new DateTime(employee.DOJ.Year, 12, 31);

                Decimal totalEmergencyLeaves = 0;
                Decimal totalBereavementLeaves = 3;

                int monthDifference = (december31st.Year - employee.DOJ.Year) * 12 + december31st.Month - employee.DOJ.Month;

                // Calculate total emergency leaves based on the month difference
                totalEmergencyLeaves = (int)(monthDifference / 3) + ((monthDifference % 3 > 0) ? 1 : 0);

                var selectedLeaveTypeTaken = leaves.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == leaveType).ToList();
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

        private static AvailableLeaves CalculatePermanentLeaves(emp_info employee, List<con_leaveupdate> leaves, string leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            // Calculate leaves for permanent employees
            int earnedLeavesPerMonth = 12;
            int sickLeavesPerMonth = 6;
            int emergencyLeavesPerYear = 4;

            // Calculate total leaves taken
            int earnedLeavesTaken = leaves.Where(l => l.employee_id == employee.EmployeeID && l.DayType == "Earned Leave").Count();
            int sickLeavesTaken = leaves.Where(l => l.employee_id == employee.EmployeeID && l.DayType == "Sick Leave").Count();
            int emergencyLeavesTaken = leaves.Where(l => l.employee_id == employee.EmployeeID && l.DayType == "Emergency Leave").Count();
            int BereavementLeave = leaves.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == "Bereavement Leave").Count();


            // Calculate available leaves
            int availableEarnedLeaves = earnedLeavesPerMonth - earnedLeavesTaken;
            int availableSickLeaves = sickLeavesPerMonth - sickLeavesTaken;
            int availableEmergencyLeaves = emergencyLeavesPerYear - emergencyLeavesTaken;
            int availaleBereavementLeaves = BereavementLeave - 3;

            //availableLeaves.EarnedLeave = Math.Max(availableEarnedLeaves, 0);
            //availableLeaves.SickLeave = Math.Max(availableSickLeaves, 0);
            //availableLeaves.EmergencyLeave = Math.Max(availableEmergencyLeaves, 0);
            //availableLeaves.BereavementLeave = Math.Max(availaleBereavementLeaves, 0);

            return availableLeaves;
            //return Math.Max(availableEarnedLeaves, 0) + Math.Max(availableSickLeaves, 0) + Math.Max(availableEmergencyLeaves, 0);
        }
    }
}