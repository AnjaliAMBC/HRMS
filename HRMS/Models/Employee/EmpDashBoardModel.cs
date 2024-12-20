﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace HRMS.Models.Employee
{
    using static HRMS.Models.EmployeeEvent;
    public class EmpDashBoardModel : SiteContextModel
    {
        public List<AnniversaryModel> AnniversaryModel { get; set; } = new List<AnniversaryModel>();
        public List<BirthdayModel> Birthdays { get; set; } = new List<BirthdayModel>();

        public List<emp_info> NewJoiners { get; set; } = new List<emp_info>();
        public List<UpcomingHoliday> UpcomingHolidays { get; set; } = new List<UpcomingHoliday>();
        public tbld_ambclogininformation todayCheckInInfo { get; set; } = new tbld_ambclogininformation();
        public CheckInDetails empLastDayCheckInDetails { get; set; } = new CheckInDetails();
        public TimerModel timerModel { get; set; }
        public string totalCheckedInTime { get; set; } = "";
        public bool IsOnLeave { get; set; } = false;
        public LeaveTypesBasedOnEmpViewModel LeavesTypeInfo { get; set; } = new LeaveTypesBasedOnEmpViewModel();
        public List<emp_info> Employees { get; set; } = new List<emp_info>();
        public con_leaveupdate empLeaveInfo { get; set; } = new con_leaveupdate();
        public List<IT_Ticket> MyTickets { get; set; } = new List<IT_Ticket>();

    }


    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }

        public string BCCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string AttachmentPath { get; set; }

        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }

    public class HolidayModel
    {
        public List<HRMS.tblambcholiday> Holidays { get; set; } = new List<tblambcholiday>();

        public List<emp_info> Employees { get; set; } = new List<emp_info>();
    }
}