using HRMS.Models.Employee;
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

        public AvailableLeaves CalculateAvailableLeaves(emp_info employee, LeavesCategory leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            if (IsProbationaryEmployee(employee))
            {
                availableLeaves = CalculateProbationaryLeaves(employee, leaveType);
            }
            else
            {
                availableLeaves = CalculatePermanentLeaves(employee, leaveType);
            }

            return availableLeaves;
        }

        private static bool IsProbationaryEmployee(emp_info employee)
        {
            var lastDayOfProbatation = employee.DOJ.AddMonths(3);
            return lastDayOfProbatation >= DateTime.Today ? true : false;
        }

        private AvailableLeaves CalculateProbationaryLeaves(emp_info employee, LeavesCategory leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            if (leaveType.Type == "Sick Leave" || leaveType.Type == "Emergency Leave" || leaveType.Type == "Bereavement Leave")
            {
                Decimal totalSickLeaves = 3;

                DateTime december31st = new DateTime(DateTime.Today.Year, 12, 31);
                DateTime probationEndDate = employee.DOJ.AddMonths(3);

                Decimal totalEmergencyLeaves = 0;
                Decimal totalBereavementLeaves = 3;
                int monthDifference = (december31st.Year - employee.DOJ.Year) * 12 + december31st.Month - employee.DOJ.Month + (december31st.Day >= employee.DOJ.Day ? 1 : 0);

                totalEmergencyLeaves = (int)(monthDifference / 3) + ((monthDifference % 3 > 0) ? 1 : 0);

                var selectedLeaveTypeTaken = _dbContext.con_leaveupdate.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == leaveType.Type && l.leavedate >= employee.DOJ && l.leavedate <= probationEndDate && (l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Rejected")).ToList();
                decimal totalLeaveDays = selectedLeaveTypeTaken.Sum(l => l.LeaveDays);

                if (leaveType.Type == "Sick Leave")
                {
                    availableLeaves.Available = totalSickLeaves;
                }

                if (leaveType.Type == "Emergency Leave")
                {
                    availableLeaves.Available = totalEmergencyLeaves;
                }

                if (leaveType.Type == "Bereavement Leave")
                {
                    availableLeaves.Available = totalBereavementLeaves;
                }
                availableLeaves.Booked = totalLeaveDays;
                availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
                availableLeaves.Type = leaveType.Type;
                availableLeaves.ColorCode = leaveType.Colrocode;
            }
            return availableLeaves;
        }

        private AvailableLeaves CalculatePermanentLeaves(emp_info employee, LeavesCategory leaveType)
        {
            var availableLeaves = new AvailableLeaves();

            var isEmployeeProbationInThisCurrentYear = employee.DOJ.AddMonths(3);

            DateTime startDate = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endDate = new DateTime(DateTime.Today.Year, 12, 31);

            if (leaveType.Type != "Bereavement Leave" && leaveType.Type != "Emergency Leave")
            {
                if (isEmployeeProbationInThisCurrentYear.Year == DateTime.Today.Year)
                {
                    startDate = isEmployeeProbationInThisCurrentYear;
                }
            }

            if (leaveType.Type == "Hourly Permission")
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            var startMonthNumber = startDate.Month;
            var endMonthNumber = endDate.Month;

            if (startMonthNumber == 1)
            {
                startMonthNumber = 0;
            }

            int monthDifference = endMonthNumber - startMonthNumber;

            //int monthDifference = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + (endDate.Day >= startDate.Day ? 1 : 0);
            decimal earnedLeavesPerMonth = monthDifference * 1;
            decimal totalSickLeaves = (decimal)monthDifference * (decimal)0.5;
            decimal totalEmergencyLeaves = (int)(monthDifference / 3) + ((monthDifference % 3 > 0) ? 1 : 0);

            var selectedLeaveTypeTaken = _dbContext.con_leaveupdate.Where(l => l.employee_id == employee.EmployeeID && l.leavesource == leaveType.Type && l.leavedate >= startDate && l.leavedate <= endDate && (l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Rejected")).ToList();
            decimal totalLeaveDays = selectedLeaveTypeTaken.Sum(l => l.LeaveDays);

            availableLeaves.Type = leaveType.Type;
            availableLeaves.ColorCode = leaveType.Colrocode;

            if (leaveType.Type == "Sick Leave")
            {
                availableLeaves.Available = totalSickLeaves;
            }

            if (leaveType.Type == "Emergency Leave")
            {
                availableLeaves.Available = totalEmergencyLeaves;
            }

            if (leaveType.Type == "Bereavement Leave")
            {
                availableLeaves.Available = 3;
            }

            if (leaveType.Type == "Earned Leave")
            {
                availableLeaves.Available = earnedLeavesPerMonth;
            }

            if (leaveType.Type == "Marriage Leave")
            {
                availableLeaves.Available = employee.MaritalStatus == "Single" ? 10 : 0;
            }

            if (leaveType.Type == "Maternity Leave")
            {
                availableLeaves.Available = (employee.Gender == "Female" && employee.MaritalStatus == "Married") ? 180 : 0;
            }

            if (leaveType.Type == "Paternity Leave")
            {
                availableLeaves.Available = (employee.Gender == "Male" && employee.MaritalStatus == "Married") ? 5 : 0;
            }

            if (leaveType.Type == "Comp Off")
            {
                availableLeaves.Available = 5;
            }

            if (leaveType.Type == "Hourly Permission")
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


        public LeaveTypesBasedOnEmpViewModel GetLeavesByEmp(string empId)
        {
            var employess = new List<emp_info>();

            var empLeaveTypes = new LeaveTypesBasedOnEmpViewModel();

            if (!string.IsNullOrWhiteSpace(empId))
            {
                employess = _dbContext.emp_info.Where(x => x.EmployeeID == empId).ToList();
            }
            else
            {
                employess = _dbContext.emp_info.ToList();
            }

            var leaveTypes = new List<LeavesCategory>();
            leaveTypes.Add(new LeavesCategory() { Type = "Earned Leave", Colrocode = "ear_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Emergency Leave", Colrocode = "emr_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Sick Leave", Colrocode = "sick_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Bereavement Leave", Colrocode = "bev_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Hourly Permission", Colrocode = "hou_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Marriage Leave", Colrocode = "mar_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Maternity Leave", Colrocode = "mat_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Paternity Leave", Colrocode = "pat_border" });
            leaveTypes.Add(new LeavesCategory() { Type = "Comp Off", Colrocode = "com_border" });

            foreach (var emp in employess)
            {
                var employee = new LeaveEmployee();
                employee.empInfo = emp;
                foreach (var leaveType in leaveTypes)
                {
                    employee.AvailableLeaves.Add(new LeaveCalculator().CalculateAvailableLeaves(emp, leaveType));
                }
                empLeaveTypes.LeaveTypes.Add(employee);
            }

            return empLeaveTypes;
        }
    }
}