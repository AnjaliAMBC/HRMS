using System;

namespace HRMS.Models
{
    public class TimerModel
    {
        public string FormattedTime { get; set; }

        public TimerModel(DateTime startDate)
        {
            TimeSpan timeDifference = DateTime.Now - startDate;
            int hours = (int)timeDifference.TotalHours;
            int minutes = timeDifference.Minutes;
            int seconds = timeDifference.Seconds;

            FormattedTime = FormatTime(hours, minutes, seconds);
        }

        private string FormatTime(int hours, int minutes, int seconds)
        {
            string formattedHours = hours < 10 ? "0" + hours : hours.ToString();
            string formattedMinutes = minutes < 10 ? "0" + minutes : minutes.ToString();
            string formattedSeconds = seconds < 10 ? "0" + seconds : seconds.ToString();
            return formattedHours + ":" + formattedMinutes + ":" + formattedSeconds;
        }
    }
}