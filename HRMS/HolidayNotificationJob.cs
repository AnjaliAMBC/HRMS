using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using HRMS.Models.Admin;
using HRMS.Helpers;
using HRMS.Models.Employee;
using System.Data.Entity;

namespace HRMS
{
    public class HolidayNotificationJob : IJob
    {
        private readonly HRMS_EntityFramework _dbContext;

        public HolidayNotificationJob()
        {
            _dbContext = new HRMS_EntityFramework();
        }


        public Task Execute(IJobExecutionContext context)
        {
            var today = DateTime.Today;

            // Fetch holidays that are exactly 10 days from today
            var upcomingHolidays = _dbContext.tblambcholidays
                                             .Where(h => DbFunctions.DiffDays(today, h.holiday_date) == 10)
                                             .ToList();

            // Fetch group email addresses from the configuration
            var groupEmailsConfig = ConfigurationManager.AppSettings["GroupEmails"];
            var groupEmailMappings = groupEmailsConfig.Split(',')
                                                       .Select(g => g.Split('@'))
                                                       .ToDictionary(g => g[0].Replace("ambc", ""), g => $"ambc{g[0]}@ambconline.com");

            // Group holidays by region and send emails
            var groupedHolidays = upcomingHolidays.GroupBy(h => h.region);
            foreach (var group in groupedHolidays)
            {
                if (groupEmailMappings.TryGetValue(group.Key.ToLower(), out var groupEmail))
                {
                    var holidays = group.ToList();
                    SendHolidayNotification(groupEmail, holidays);
                }
            }

            return Task.CompletedTask;
        }

        private void SendHolidayNotification(string groupEmail, List<tblambcholiday> holidays)
        {
            var siteURL = ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";
            var holidayDescriptions = holidays.Select(h => $@"
                <p>Please note, {h.holiday_date:dd MMMM yyyy} ({h.holiday_date:dddd}) will be observed as holiday for {h.holiday_name}.</p>
                <p>Kindly let us know your work status on or before {h.holiday_date?.AddDays(-10):dd MMMM yyyy} to avail comp off.</p>").Aggregate((a, b) => a + b);

            var body = $@"
<html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='overflow: auto; margin-bottom: 20px;'>
            <img src='{logoURL}' alt='AMBC Logo' style='max-width: 50px; float: right; margin-left: 10px;'>
            <p style='margin: 0; font-size: 18px;'>Hello All,</p>
        </div>
        <p style='font-size: 16px;'>Greetings of the day!</p>
        <div style='font-size: 16px;'>
            {holidayDescriptions}
        </div>
        <p style='font-size: 16px;'>Please submit your request through our portal at least 10 days before the deadline.</p>
        <p style='font-size: 16px;'><a href='{siteURL}' style='color: #007bff;'>[PASTE THE REDIRECTION LINK HERE]</a></p>
    </body>
</html>";

            var emailRequest = new EmailRequest()
            {
                ToEmail = groupEmail,
                Subject = $"Holiday Notification for Region {holidays.First().region} on {holidays.First().holiday_date:dd MMMM yyyy}",
                Body = body
            };

            EMailHelper.SendEmail(emailRequest);
        }
    }
}