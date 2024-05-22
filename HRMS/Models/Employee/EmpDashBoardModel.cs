﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.Employee
{
    public class EmpDashBoardModel : SiteContextModel
    {
        public List<AnniversaryModel> AnniversaryModel { get; set; } = new List<AnniversaryModel>();
        public List<BirthdayModel> BirthModel { get; set; } = new List<BirthdayModel>();

        public List<UpcomingHoliday> UpcomingHolidays { get; set; }
    }
    public class AnniversaryModel
    {
        public string Empid { get; set; }
        public string EmpName { get; set; }
        public int years { get; set; }
        public string Designation { get; set; }
        public DateTime JoiningDate { get; set; }
        public string imagePath { get; set; }
        public string EmpEmail { get; set; }
    }
    public class BirthdayModel
    {
        public string Empid { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public DateTime BirthDate { get; set; }
        public string imagePath { get; set; }
        public string EmpEmail { get; set; }
    }
    public class UpcomingHoliday
    {
        public int HolidayNo { get; set; }
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string Region { get; set; }
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