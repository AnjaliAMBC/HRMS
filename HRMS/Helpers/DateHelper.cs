﻿using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class DateHelper
    {
        public static List<DateTime> GetAllDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();

            // Add start date
            allDates.Add(startDate);

            // Loop until start date reaches end date
            while (startDate < endDate)
            {
                // Increment start date by one day
                startDate = startDate.AddDays(1);
                // Add current date to the list
                allDates.Add(startDate);
            }

            return allDates;
        }

        public static string FormatTimeDifference(TimeSpan timeDifference)
        {
            int days = (int)timeDifference.TotalDays;
            int hours = timeDifference.Hours;
            int minutes = timeDifference.Minutes;

            return $"{days} days {hours} hours {minutes} minutes";
        }

        public static List<MonthInfo> GetAllMonths()
        {          
            List<MonthInfo> months = new List<MonthInfo>();           
            for (int i = 0; i < 12; i++)
            {
                months.Add(new MonthInfo(i + 1, new DateTime(1, i + 1, 1).ToString("MMMM")));
            }
            return months;
        }
    }
}