using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using HRMS.Models.Admin;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;
using OfficeOpenXml;
using HRMS.Models.Employee;
using HRMS.Helpers;
using System.Configuration;

namespace HRMS
{
    public class GenerateAndSendReportJob : IJob
    {
        private readonly HRMS_EntityFramework _dbContext;

        public GenerateAndSendReportJob()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public Task Execute(IJobExecutionContext context)
        {
            var today = DateTime.Today;

            // Get all active employees
            var activeEmployees = _dbContext.emp_info
                                    .Where(e => e.EmployeeStatus == "Active")
                                    .ToList();

            // Get today's login info
            var loginInfos = _dbContext.tbld_ambclogininformation
                                    .Where(l => l.Login_date == today)
                                    .ToList();

            // Combine active employees with their login info
            var combinedInfos = activeEmployees.GroupJoin(
                loginInfos,
                e => e.EmployeeID,
                l => l.Employee_Code,
                (e, ls) => new { Employee = e, LoginInfos = ls.DefaultIfEmpty() })
                .SelectMany(
                    x => x.LoginInfos.Select(l => new EmployeeLoginInfo
                    {
                        Employee = x.Employee,
                        LoginInfo = l
                    }))
                .ToList();

            // Convert data to table format
            string tableHtml = GenerateHtmlTable(combinedInfos);

            var toEmails = ConfigurationManager.AppSettings["AttedenceReportTo"];
            var CCEmails = ConfigurationManager.AppSettings["AttedenceReportCC"];

            var emailRequest = new EmailRequest()
            {
                Body = tableHtml,
                ToEmail = toEmails,
                CCEmail = CCEmails,
                Subject = "AMBC attendance report for Current date   - " + today.ToString("ddd, dd MMMM yyyy"),
            };

            var attedenceReportNotification = EMailHelper.SendEmail(emailRequest);

            return Task.CompletedTask;
        }

        public static string GenerateHtmlTable(List<EmployeeLoginInfo> combinedInfos)
        {
            var currentDate = DateTime.Now.ToString("ddd, MMM dd, yyyy");
            var siteURL = ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";

            string todayloginDate = DateTime.Now.ToString("yyyy-MM-dd");

            var table = @"
<html>
    <body style='font-family: Arial, sans-serif;'>
        <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;'>
            <p style='margin: 0;'>AMBC attendance report for " + currentDate + @"</p>
            <a href='" + siteURL + @"' target='_blank' style='margin-left: auto;'>
                <img src='" + logoURL + @"' alt='AMBC Logo' width='70' style='float: right;'>
            </a>
        </div>
        <table border='1' style='border-collapse: collapse; width: 100%; margin-top: 20px;'>
            <thead style='background-color: #87CEEB; color: white;'>
                <tr>
                    <th>Employee ID</th>
                    <th>Employee Name</th>                   
                    <th>Employee Shift</th>
                    <th>Login Date</th>
                    <th>CheckIn Time</th>
                    <th>CheckOut Time</th>
                    <th>Working Hours</th>
                </tr>
            </thead>
            <tbody>";

            foreach (var info in combinedInfos)
            {
                var loginInfo = info.LoginInfo;
                TimeSpan workingHours = TimeSpan.Zero;

                if (loginInfo != null)
                {
                    if (loginInfo.Signout_Time.HasValue)
                    {
                        // Calculate working hours using the sign-out time
                        workingHours = loginInfo.Signout_Time.Value - loginInfo.Signin_Time;
                    }
                    else
                    {
                        // Calculate working hours using the current time
                        workingHours = DateTime.Now - loginInfo.Signin_Time;
                    }
                }

              

                table += $@"
        <tr>
            <td>{info.Employee.EmployeeID}</td>
            <td>{info.Employee.EmployeeName}</td>            
            <td>{info.Employee.ShiftTimings}
            <td>{todayloginDate}</td> <!-- Using the current date here -->
            <td>{loginInfo?.Signin_Time.ToString(@"hh\:mm") ?? ""}</td>
            <td>{loginInfo?.Signout_Time?.ToString(@"hh\:mm") ?? ""}</td>
            <td>{workingHours.ToString(@"hh\:mm")}</td>
        </tr>";
            }

            table += @"
        </tbody>
    </table>
    <div style='font-family: Calibri; color: #696969; font-size: 1.1em; margin-top: 20px;'>
        This is an automated email, please do not reply.
        <br />
        Automated mail from <a href='https://prm.ambctechnologies.com' style='color: #2693F8;'>https://prm.ambctechnologies.com</a>
    </div>
    <p>Thank you.</p>
</body>
</html>";


            return table;
        }


        public class EmployeeLoginInfo
        {
            public emp_info Employee { get; set; }
            public tbld_ambclogininformation LoginInfo { get; set; }
        }

    }
}