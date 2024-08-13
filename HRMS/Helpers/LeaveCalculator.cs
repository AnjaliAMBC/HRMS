using HRMS.Models.Admin;
using HRMS.Models.Employee;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            //var lastDayOfProbatation = employee.DOJ.AddMonths(3);
            //return lastDayOfProbatation >= DateTime.Today ? true : false;
            return employee.EmployeeType == "Probation" ? true : false;
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


                if (leaveType.Type == "Comp Off")
                {
                    var employeeCompoffs = _dbContext.Compoffs
                    .Where(x => x.EmployeeID == employee.EmployeeID && x.CampOffDate.Year == DateTime.Today.Year && x.addStatus == "Approved")
                    .ToList();

                    if (employeeCompoffs != null && employeeCompoffs.Count() > 0)
                    {
                        availableLeaves.Available = employeeCompoffs.Count();
                    }
                    else
                    {
                        availableLeaves.Available = 0;
                    }
                }

                availableLeaves.Booked = totalLeaveDays;
                availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
                availableLeaves.Type = leaveType.Type;
                availableLeaves.ColorCode = leaveType.Colrocode;
                availableLeaves.DashBoardColorCode = leaveType.DashBoardColorCode;
                availableLeaves.ShortName = leaveType.ShortName;
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
                var employeeCompoffs = _dbContext.Compoffs
                .Where(x => x.EmployeeID == employee.EmployeeID && x.CampOffDate.Year == DateTime.Today.Year && x.addStatus == "Approved")
                .ToList();

                if (employeeCompoffs != null && employeeCompoffs.Count() > 0)
                {
                    availableLeaves.Available = employeeCompoffs.Count();
                }
                else
                {
                    availableLeaves.Available = 0;
                }
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
            availableLeaves.DashBoardColorCode = leaveType.DashBoardColorCode;
            availableLeaves.ShortName = leaveType.ShortName;
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

            var leaveTypes = LeaveCategories();

            var currentYear = DateTime.Today.Year.ToString();

            foreach (var emp in employess)
            {
                var employee = new LeaveEmployee();
                employee.empInfo = emp;
                var isEmpLeaveBalanceExists = _dbContext.LeaveBalances.Where(x => x.EmpID == emp.EmployeeID && x.Year == currentYear).FirstOrDefault();

                if (isEmpLeaveBalanceExists != null)
                {

                    var empLeaveBalance = new LeaveBalance()
                    {
                        Bereavement = isEmpLeaveBalanceExists.Bereavement,
                        CompOff = isEmpLeaveBalanceExists.CompOff,
                        EmployeeName = isEmpLeaveBalanceExists.EmployeeName,
                        Earned = isEmpLeaveBalanceExists.Earned,
                        Emergency = isEmpLeaveBalanceExists.Emergency,
                        EmpID = isEmpLeaveBalanceExists.EmpID,
                        HourlyPermission = isEmpLeaveBalanceExists.HourlyPermission,
                        Marriage = isEmpLeaveBalanceExists.Marriage,
                        Maternity = isEmpLeaveBalanceExists.Maternity,
                        Paternity = isEmpLeaveBalanceExists.Paternity,
                        Sick = isEmpLeaveBalanceExists.Sick,
                        Year = isEmpLeaveBalanceExists.Year
                    };

                    foreach (var leaveType in leaveTypes)
                    {
                        employee.AvailableLeaves.Add(new LeaveCalculator().GetEmpBasedAvailableLeaves(emp, empLeaveBalance, leaveType));
                    }
                    empLeaveTypes.LeaveTypes.Add(employee);
                }
                else
                {
                    var empAvailableLeave = new AvailableLeaves();
                    var empLeaveBalance = new LeaveBalance();

                    var currentYearFirstDate = new DateTime(DateTime.Now.Year, 1, 1);
                    var lastYearFirstDate = currentYearFirstDate.AddMonths(-12);

                    var lastYear = lastYearFirstDate.Year.ToString();
                    decimal? lastYearEarnedLeaveBalance = 0;

                    if (lastYearFirstDate >= emp.DOJ)
                    {
                        var lastYearLeaveBalance = _dbContext.LeaveBalances.Where(x => x.EmpID == emp.EmployeeID && x.Year == lastYear).FirstOrDefault();

                        if (lastYearLeaveBalance != null)
                        {
                            lastYearEarnedLeaveBalance = lastYearLeaveBalance.Earned >= 5 ? 5 : 0;
                        }
                    }

                    foreach (var leaveType in leaveTypes)
                    {
                        empAvailableLeave = new LeaveCalculator().CalculateAvailableLeaves(emp, leaveType);

                        if (leaveType.Type == "Earned Leave")
                        {
                            empLeaveBalance.Earned = empAvailableLeave.Available + lastYearEarnedLeaveBalance;
                            empAvailableLeave.Available = empAvailableLeave.Available + lastYearEarnedLeaveBalance;
                            empAvailableLeave.Balance = empAvailableLeave.Available - empAvailableLeave.Booked;
                        }
                        if (leaveType.Type == "Emergency Leave")
                        {
                            empLeaveBalance.Emergency = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Sick Leave")
                        {
                            empLeaveBalance.Sick = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Bereavement Leave")
                        {
                            empLeaveBalance.Bereavement = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Hourly Permission")
                        {
                            empLeaveBalance.HourlyPermission = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Marriage Leave")
                        {
                            empLeaveBalance.Marriage = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Maternity Leave")
                        {
                            empLeaveBalance.Maternity = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Paternity Leave")
                        {
                            empLeaveBalance.Paternity = empAvailableLeave.Available;
                        }
                        if (leaveType.Type == "Comp Off")
                        {
                            empLeaveBalance.CompOff = empAvailableLeave.Available;
                        }

                        employee.AvailableLeaves.Add(empAvailableLeave);
                    }
                    empLeaveBalance.EmpID = employee.empInfo.EmployeeID;
                    empLeaveBalance.EmployeeName = employee.empInfo.EmployeeName;
                    empLeaveBalance.Year = currentYear;
                    empLeaveTypes.LeaveTypes.Add(employee);
                    _dbContext.LeaveBalances.Add(empLeaveBalance);
                    _dbContext.SaveChanges();
                }
            }

            return empLeaveTypes;
        }

        public AdminLeaveManagementModel GetLeavesInfoBasedonStartandEndDate(string selectedStartDate, string SelectedEndDate, AdminLeaveManagementModel model, string empID, bool fromDashboard = false)
        {
            var cuserContext = SiteContext.GetCurrentUserContext();
            model.EmpInfo = cuserContext.EmpInfo;
            model.LoginInfo = cuserContext.LoginInfo;

            model.SelectedDate = DateTime.Today;
            model.SelectedEndDate = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(selectedStartDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedDate = DateTime.ParseExact(selectedStartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedDate = DateTime.Parse(selectedStartDate);
            }

            if (!string.IsNullOrWhiteSpace(SelectedEndDate))
            {
                if (!selectedStartDate.Contains('-'))
                    model.SelectedEndDate = DateTime.ParseExact(SelectedEndDate, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                else
                    model.SelectedEndDate = DateTime.Parse(SelectedEndDate);
            }

            var selectedDateLeaves = new List<con_leaveupdate>();



            if (string.IsNullOrWhiteSpace(empID))
            {
                if (fromDashboard)
                {
                    selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.createddate >= model.SelectedDate && x.createddate <= model.SelectedEndDate).ToList();
                }
                else
                {
                    selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate).ToList();
                }

            }
            else
            {
                if (fromDashboard)
                {
                    selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.createddate >= model.SelectedDate && x.createddate <= model.SelectedEndDate && x.employee_id == empID).ToList();
                }
                else
                {
                    selectedDateLeaves = _dbContext.con_leaveupdate.Where(x => x.leavedate >= model.SelectedDate && x.leavedate <= model.SelectedEndDate && x.employee_id == empID).ToList();
                }
            }

            model.LeavesInfo = selectedDateLeaves.OrderBy(x => x.leavedate).ToList();

            return model;
        }

        public List<LeaveInfo> EmpLeaveInfo(string empId, int year)
        {
            int currentYear = year == 0 ? DateTime.Now.Year : year;
            string january1stString = $"{currentYear}-01-01";
            string december31stString = $"{currentYear}-12-31";

            DateTime january1st = DateTime.ParseExact(january1stString, "yyyy-MM-dd", null);
            DateTime december31st = DateTime.ParseExact(december31stString, "yyyy-MM-dd", null);

            // Query leave data and group by LeaveRequestName
            var leaves = _dbContext.con_leaveupdate
                .Where(x => x.employee_id == empId && x.leavedate >= january1st && x.leavedate <= december31st)
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new LeaveInfo
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            //// Query Comp Off data for the specified year
            //var compoffsApplied = _dbContext.Compoffs
            //    .Where(x => x.EmployeeID == empId && x.CampOffDate.Year == year)
            //    .ToList();

            //// Convert Comp Off data to LeaveInfo format and add to leaves list
            //foreach (var compoff in compoffsApplied)
            //{
            //    var compOffLeave = new LeaveInfo
            //    {
            //        LeaveRequestName = compoff.concatinatestring,
            //        Fromdate = compoff.CampOffDate,
            //        Todate = compoff.CampOffDate,
            //        TotalLeaveDays = 1, // Assuming each comp off is 1 day
            //        LatestLeave = new con_leaveupdate
            //        {
            //            leavedate = compoff.CampOffDate,
            //            LeaveRequestName = compoff.concatinatestring,
            //            LeaveStatus = compoff.addStatus,
            //            leavesource = "CompOff"
            //        }
            //    };

            //    leaves.Add(compOffLeave);
            //}

            // If you need to return leaves list as a specific type, convert it accordingly
            var result = leaves
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();
            return result;
        }

        public List<LeaveInfo> EmpLeaveInfoBasedonFromAndToDatesWithLeaveDate(
    string startDate,
    string endDate,
    string empId,
    string department,
    string location,
    string status)
        {
            DateTime dateStart = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
            DateTime dateEnd = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);

            var query = _dbContext.con_leaveupdate.AsQueryable();

            if (!string.IsNullOrWhiteSpace(empId))
            {
                query = query.Where(x => x.employee_id == empId);
            }

            query = query.Where(x => x.leavedate >= dateStart && x.leavedate <= dateEnd);

            if (department != "null" && !string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(x => x.Department == department);
            }

            if (location != "null" && !string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(x => x.Location == location);
            }

            if (status != "null" && !string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.LeaveStatus == status);
            }

            var leaves = query
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new LeaveInfo
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            return leaves;
        }

        public List<LeaveInfo> EmpLeaveInfoBasedonBasedOnTodayDate(
    string startDate,
    string endDate,
    string empId,
    string department,
    string location,
    string status)
        {
            DateTime dateStart = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
            DateTime dateEnd = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);

            var query = _dbContext.con_leaveupdate.AsQueryable();

            if (!string.IsNullOrWhiteSpace(empId))
            {
                query = query.Where(x => x.employee_id == empId);
            }

            query = query.Where(x => x.createddate >= dateStart && x.createddate <= dateEnd);

            if (department != "null" && !string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(x => x.Department == department);
            }

            if (location != "null" && !string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(x => x.Location == location);
            }

            if (status != "null" && !string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.LeaveStatus == status);
            }

            var leaves = query
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new LeaveInfo
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            return leaves;
        }

        public List<LeaveInfo> EmpLeaveInfoBasedonDate(string startdate, string enddate)
        {

            DateTime dateStart = DateTime.ParseExact(startdate, "dd-MM-yyyy", null);

            // Query leave data and group by LeaveRequestName
            var leaves = _dbContext.con_leaveupdate
                .Where(x => x.createddate == dateStart)
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new LeaveInfo
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            // If you need to return leaves list as a specific type, convert it accordingly
            var result = leaves
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();
            return result;
        }

        public List<LeaveInfo> EmpLeaveInfoBasedonFromAndToDateWithCreatedDate(string startdate, string enddate)
        {

            DateTime dateStart = DateTime.ParseExact(startdate, "dd-MM-yyyy", null);
            DateTime dateEnd = DateTime.ParseExact(enddate, "dd-MM-yyyy", null);

            // Query leave data and group by LeaveRequestName
            var leaves = _dbContext.con_leaveupdate
                .Where(x => x.createddate >= dateStart && x.createddate <= dateEnd)
                .GroupBy(x => x.LeaveRequestName)
                .Select(g => new LeaveInfo
                {
                    LeaveRequestName = g.Key,
                    Fromdate = g.Min(x => x.Fromdate),
                    Todate = g.Max(x => x.Todate),
                    TotalLeaveDays = g.Sum(x => x.LeaveDays),
                    LatestLeave = g.OrderByDescending(x => x.leavedate).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();

            // If you need to return leaves list as a specific type, convert it accordingly
            var result = leaves
                .OrderByDescending(x => x.LatestLeave.leavedate)
                .ToList();
            return result;
        }


        public AvailableLeaves GetEmpBasedAvailableLeaves(emp_info employee, LeaveBalance leaveBalanceInfo, LeavesCategory leaveType)
        {
            var availableLeaves = new AvailableLeaves();
            var LeaveBalanceCheckDates = new LeaveBalanceCheckDates();
            if (IsProbationaryEmployee(employee))
            {
                LeaveBalanceCheckDates = GetProbationaryEmpLeaveDates(employee, leaveType);
            }
            else
            {
                LeaveBalanceCheckDates = GetPermanentEmpLeaveDates(employee, leaveType);
            }

            var selectedLeaveTypeTaken = _dbContext.con_leaveupdate.Where(l => l.employee_id == leaveBalanceInfo.EmpID && l.leavesource == leaveType.Type && l.leavedate >= LeaveBalanceCheckDates.StartDate && l.leavedate <= LeaveBalanceCheckDates.EndDate && (l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Rejected")).ToList();
            decimal totalLeaveDays = selectedLeaveTypeTaken.Sum(l => l.LeaveDays);

            availableLeaves.Type = leaveType.Type;
            availableLeaves.ColorCode = leaveType.Colrocode;

            if (leaveType.Type == "Sick Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Sick;
            }

            if (leaveType.Type == "Emergency Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Emergency;
            }

            if (leaveType.Type == "Bereavement Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Bereavement;
            }

            if (leaveType.Type == "Earned Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Earned;
            }

            if (leaveType.Type == "Marriage Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Marriage;
            }

            if (leaveType.Type == "Maternity Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Maternity;
            }

            if (leaveType.Type == "Paternity Leave")
            {
                availableLeaves.Available = leaveBalanceInfo.Paternity;
            }

            if (leaveType.Type == "Comp Off")
            {
                availableLeaves.Available = leaveBalanceInfo.CompOff;

                var employeeCompoffs = _dbContext.Compoffs
                .Where(x => x.EmployeeID == leaveBalanceInfo.EmpID && x.CampOffDate.Year == DateTime.Today.Year && x.addStatus == "Approved")
                .ToList();

                if (employeeCompoffs != null && employeeCompoffs.Count() > 0)
                {
                    availableLeaves.Available = availableLeaves.Available + employeeCompoffs.Count();
                }
                else
                {
                    availableLeaves.Available = availableLeaves.Available;
                }

                availableLeaves.Booked = totalLeaveDays;


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
                availableLeaves.DashBoardColorCode = leaveType.DashBoardColorCode;
                availableLeaves.ShortName = leaveType.ShortName;
                return availableLeaves;
            }

            availableLeaves.Booked = totalLeaveDays;
            availableLeaves.Balance = availableLeaves.Available - availableLeaves.Booked;
            availableLeaves.DashBoardColorCode = leaveType.DashBoardColorCode;
            availableLeaves.ShortName = leaveType.ShortName;
            return availableLeaves;
        }


        private LeaveBalanceCheckDates GetProbationaryEmpLeaveDates(emp_info employee, LeavesCategory leaveType)
        {
            var leaveBalanceDates = new LeaveBalanceCheckDates();

            if (leaveType.Type == "Sick Leave" || leaveType.Type == "Emergency Leave" || leaveType.Type == "Bereavement Leave")
            {
                leaveBalanceDates.StartDate = employee.DOJ;
                leaveBalanceDates.EndDate = employee.DOJ.AddMonths(3);
            }
            return leaveBalanceDates;
        }

        private LeaveBalanceCheckDates GetPermanentEmpLeaveDates(emp_info employee, LeavesCategory leaveType)
        {
            var leaveBalanceDates = new LeaveBalanceCheckDates();

            var isEmployeeProbationInThisCurrentYear = employee.DOJ.AddMonths(3);

            DateTime startDate = new DateTime(DateTime.Today.Year, 01, 01);
            DateTime endDate = new DateTime(DateTime.Today.Year, 12, 31);

            if (leaveType.Type != "Bereavement Leave" && leaveType.Type != "Emergency Leave")
            {
                if (isEmployeeProbationInThisCurrentYear.Year == DateTime.Today.Year)
                {
                    var probationempYear = isEmployeeProbationInThisCurrentYear.Year;
                    var probationempMonth = isEmployeeProbationInThisCurrentYear.Month;
                    var probationempDate = isEmployeeProbationInThisCurrentYear.Day;

                    startDate = new DateTime(probationempYear, probationempMonth, probationempDate);
                }
            }

            if (leaveType.Type == "Hourly Permission")
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            leaveBalanceDates.StartDate = startDate;
            leaveBalanceDates.EndDate = endDate;
            return leaveBalanceDates;
        }

        public List<LeavesCategory> LeaveCategories()
        {
            var leaveTypes = new List<LeavesCategory>();
            leaveTypes.Add(new LeavesCategory() { Type = "Earned Leave", Colrocode = "ear_border", DashBoardColorCode = "bg-earned", ShortName = "EL" });
            leaveTypes.Add(new LeavesCategory() { Type = "Emergency Leave", Colrocode = "emr_border", DashBoardColorCode = "bg-emergency", ShortName = "EML" });
            leaveTypes.Add(new LeavesCategory() { Type = "Sick Leave", Colrocode = "sick_border", DashBoardColorCode = "bg-danger", ShortName = "SL" });
            leaveTypes.Add(new LeavesCategory() { Type = "Bereavement Leave", Colrocode = "bev_border", DashBoardColorCode = "bg-bereavement", ShortName = "BL" });
            leaveTypes.Add(new LeavesCategory() { Type = "Hourly Permission", Colrocode = "hou_border", DashBoardColorCode = "bg-hourly", ShortName = "HP" });
            leaveTypes.Add(new LeavesCategory() { Type = "Marriage Leave", Colrocode = "mar_border", DashBoardColorCode = "bg-marriage", ShortName = "ML" });
            leaveTypes.Add(new LeavesCategory() { Type = "Maternity Leave", Colrocode = "mat_border", DashBoardColorCode = "bg-maternity", ShortName = "MTL" });
            leaveTypes.Add(new LeavesCategory() { Type = "Paternity Leave", Colrocode = "pat_border", DashBoardColorCode = "bg-paternity", ShortName = "PL" });
            leaveTypes.Add(new LeavesCategory() { Type = "Comp Off", Colrocode = "com_border", DashBoardColorCode = "bg-compensatory", ShortName = "CO" });

            return leaveTypes;
        }

    }
}