using System;
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
        public List<UpcomingHoliday> UpcomingHolidays { get; set; } = new List<UpcomingHoliday>();
        public tbld_ambclogininformation todayCheckInInfo { get; set; } = new tbld_ambclogininformation();
        public CheckInDetails empLastDayCheckInDetails { get; set; } = new CheckInDetails();
        public TimerModel timerModel { get; set; }

        public string totalCheckedInTime { get; set; } = "";

        public bool IsOnLeave { get; set; } = false;
    }


    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string FromEmail { get; set; }

        public JsonResponse JsonResponse { get; set; } = new JsonResponse();
    }
}