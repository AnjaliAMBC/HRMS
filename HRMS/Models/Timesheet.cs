using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class TimesheetEntry
    {
        public int Id { get; set; } // Primary key
        public DateTime Date { get; set; }
        public string Client { get; set; }
        public string Category { get; set; }
        public string IncidentTaskName { get; set; }
        public string IncidentTaskDescription { get; set; }
        public string Requester { get; set; }
        public Decimal HoursSpent { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmailId { get; set; }// Employee ID or User ID for tracking
    }

    public class TimesheetEntryViewModel
    {
        public string Client { get; set; }
        public string Category { get; set; }
        public string IncidentTaskName { get; set; }
        public string IncidentTaskDescription { get; set; }
        public string Requester { get; set; }
        public string HoursSpent { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }

    public class Timesheet
    {
        public int Weeknumber { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }
        public List<DaySpecifcData> WeekInfo { get; set; }

        public SiteContextModel SiteContext { get; set; }
        public string Client { get; set; }

        public DateTime SelectedDate { get; set; }
    }

    public class DaySpecifcData
    {
        public DateTime Date { get; set; }
        public List<TimeSheet> TimeSheets { get; set; } = new List<TimeSheet>();
        public List<con_leaveupdate> Leaves { get; set; } = new List<con_leaveupdate>();
        public tbld_ambclogininformation CheckInInfo { get; set; } = new tbld_ambclogininformation();
        public Compoff CompoffInfo { get; set; } = new Compoff();
        public tblambcholiday HolidayInfo { get; set; } = new tblambcholiday();
        public bool IsWeekend { get; set; } = false;
        public bool TimeSheetSubmitted { get; set; } = false;
        public List<TimeSheetCategory> Categories { get; set; } = new List<TimeSheetCategory>();
        public List<Client> Clients { get; set; } = new List<Client>();
        public int FullDayeWorkingHours { get; set; }
        public int HalfDayeWorkingHours { get; set; }

        // Adding dataPoints as properties
        public List<Graph> DataPoints1 { get; set; } = new List<Graph>();
        public List<Graph> DataPoints2 { get; set; } = new List<Graph>();
        public List<Graph> DataPoints3 { get; set; } = new List<Graph>();
        public List<Graph> DataPoints4 { get; set; } = new List<Graph>();
        public List<Graph> DataPoints5 { get; set; } = new List<Graph>();

        public int AllowedHours { get; set; }

        public Decimal HoursSpent { get; set; }
        public Decimal OverTime { get; set; }
        public bool DateValidated { get; set; }


    }

    public class TimesheetDayModel
    {
        public int IndexNumber { get; set; }
        public DateTime Date { get; set; }
        public int Weeknumber { get; set; }
        public string BlockNumber { get; set; }
        public int StartIndexNumber { get; set; }
        public int EndIndexNumber { get; set; }
        public bool AddNewRow { get; set; } = false;
        public DaySpecifcData DaySpecifcInfo { get; set; } = new DaySpecifcData();

    }
}