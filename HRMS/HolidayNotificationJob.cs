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
            var compoffEmailTriggerDuration = System.Convert.ToInt32(ConfigurationManager.AppSettings["compoffemailtriggerdayduration"]);
            var today = DateTime.Today;

            // Fetch holidays that are exactly 10 days from today
            var upcomingHolidays = _dbContext.tblambcholidays
                                              .Where(h => DbFunctions.DiffDays(today, h.holiday_date) == compoffEmailTriggerDuration
                 && (h.holiday_date.Value.DayOfWeek != DayOfWeek.Saturday
                 && h.holiday_date.Value.DayOfWeek != DayOfWeek.Sunday))
                  .ToList();

            // Fetch group email addresses from the configuration
            var groupEmailsConfig = ConfigurationManager.AppSettings["GroupEmails"].ToLowerInvariant();
            var groupEmailMappings = groupEmailsConfig.Trim().Split(',');

            var requiredGroupEmail = "";

            // Group holidays by region and send emails
            //var groupedHolidays = upcomingHolidays.GroupBy(h => h.region);
            foreach (var upcomingHoliday in upcomingHolidays)
            {
                foreach (var groupemail in groupEmailMappings)
                {
                    foreach (var holidayRegion in upcomingHoliday.region.ToLowerInvariant().Split(','))
                    {
                        if (groupemail.ToLowerInvariant().Contains(holidayRegion.ToLowerInvariant()))
                        {
                            requiredGroupEmail += groupemail.ToLowerInvariant() + ",";
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(requiredGroupEmail) && upcomingHolidays.Any())
            {
                var holidays = upcomingHolidays.FirstOrDefault();
                SendHolidayNotification(requiredGroupEmail.Trim().TrimEnd(','), holidays, compoffEmailTriggerDuration);
            }
            return Task.CompletedTask;
        }

        private void SendHolidayNotification(string groupEmail, tblambcholiday holidays, int compoffEmailTriggerDuration)
        {
            var siteURL = ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";
            var holidayDescriptions = $@"
                <p>Please note, {holidays.holiday_date:dd MMMM yyyy} ({holidays.holiday_date:dddd}) will be observed as holiday for {holidays.holiday_name}.</p>
                <p>Kindly let us know your work status on or before {holidays.holiday_date?.AddDays(-compoffEmailTriggerDuration):dd MMMM yyyy} to avail comp off.</p>";

            var body = $@"
<html>
    <body style='font-family: Arial, sans-serif;'>
    <table width='100 %' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff;'>
        <tr>
            <td style = 'padding: 15px 0;'>
                <p style = 'margin: 0;'> Hi Team </p>
            </td>
            <td align = 'right' style = 'padding: 15px 0;'>
                <a href = '{siteURL}' target = '_blank'>
                    <img src = '{logoURL}' alt = 'AMBC Logo' width = '80' style = 'display: block;'>
                </a>
            </td>
        </tr>
    </table>
        <p style='font-size: 16px;'>Greetings of the day!</p>
        <div style='font-size: 16px;'>
            {holidayDescriptions}
        </div>
        <p style='font-size: 16px;'>Please submit your request through our portal at least 10 days before the deadline.</p>
        <p style='font-size: 16px;'><a href='{siteURL}' style='color: #007bff;'>https://prm.ambctechnologies.com</a></p>
    </body>
</html>";

            var emailRequest = new EmailRequest()
            {
                ToEmail = groupEmail,
                Subject = $"Holiday Notification for Region {holidays.region} on {holidays.holiday_date:dd MMMM yyyy}",
                Body = body
            };

            EMailHelper.SendEmail(emailRequest);
        }
    }
}