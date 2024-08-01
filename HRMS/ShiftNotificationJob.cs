using HRMS.Helpers;
using HRMS.Models.Employee;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS
{
    public class ShiftNotificationJob : IJob
    {
        private readonly HRMS_EntityFramework _dbContext;

        public ShiftNotificationJob()
        {
            _dbContext = new HRMS_EntityFramework();
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;

            // Check if today is a weekend
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                return Task.CompletedTask;
            }

            var employeesToRemindCheckin = _dbContext.emp_info
                .Where(emp => emp.EmployeeStatus == "Active")
                .AsEnumerable()
                .Where(emp =>
                {
                    // Check if the employee is on leave
                    bool isOnLeave = _dbContext.con_leaveupdate.Any(l => l.employee_id == emp.EmployeeID && l.leavedate == DateTime.Today);
                    if (isOnLeave)
                    {
                        return false;
                    }

                    // Check if today is a public holiday
                    bool isPublicHoliday = _dbContext.tblambcholidays.Any(ph => ph.holiday_date == DateTime.Today);
                    if (isPublicHoliday)
                    {
                        return false;
                    }

                    if (emp.ShiftStartTime.HasValue && emp.ShiftEndTime.HasValue)
                    {
                        var shiftStartTime = emp.ShiftStartTime.Value;
                        var timeDifference = shiftStartTime - now.TimeOfDay;
                        if (timeDifference.TotalMinutes <= 15 && timeDifference.TotalMinutes >= 0)
                        {
                            return true;
                        }
                    }
                    return false;
                });

            var employeesToRemindCheckout = _dbContext.emp_info
                .Where(emp => emp.EmployeeStatus == "Active")
                .AsEnumerable()
                .Where(emp =>
                {
                    // Check if the employee is on leave
                    bool isOnLeave = _dbContext.con_leaveupdate.Any(l => l.employee_id == emp.EmployeeID && l.leavedate == DateTime.Today);
                    if (isOnLeave)
                    {
                        return false;
                    }

                    // Check if today is a public holiday
                    bool isPublicHoliday = _dbContext.tblambcholidays.Any(ph => ph.holiday_date == DateTime.Today);
                    if (isPublicHoliday)
                    {
                        return false;
                    }

                    if (emp.ShiftStartTime.HasValue && emp.ShiftEndTime.HasValue)
                    {
                        var shiftEndTime = emp.ShiftEndTime.Value;
                        var timeDifference = shiftEndTime - now.TimeOfDay;
                        if (timeDifference.TotalMinutes <= 15 && timeDifference.TotalMinutes >= 0)
                        {
                            return true;
                        }
                    }
                    return false;
                });

            var siteURL = System.Configuration.ConfigurationManager.AppSettings["siteURL"];
            var logoURL = siteURL + "/Assets/AMBC_Logo.png";
            var companyURL = siteURL;

            // Checkin Reminder emails
            foreach (var checkinEmp in employeesToRemindCheckin)
            {
                if (checkinEmp.ShiftStartTime.HasValue)
                {
                    var shiftStartTime = new DateTime(checkinEmp.ShiftStartTime.Value.Ticks).ToString("HH:mm"); // 24-hour format
                    var checkInSubject = "Attendance Check-In Reminder";
                    var checkInBody = $@"
<html>
    <body style='font-family: Calibri, sans-serif; background-color: #f2f2f2; padding: 20px; margin: 0;'>
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto;'>
            <div style='padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='text-align: right; padding: 10px 0;'>
                            <a href='{siteURL}' target='_blank'>
                                <img src='{logoURL}' alt='AMBC Logo' width='50'>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p style='margin-top: 0; font-size: 1.1em;'>Hi <strong>{checkinEmp.EmployeeName}</strong>,</p>
                            <div style='font-family: Calibri; color: #696969; margin-top: 20px; font-size: 1.1em;'>
                                <p>Reminder for Check In</p>
                                <p>Please remember to complete your daily check in by {shiftStartTime}.</p>
                                <p>Contact [Support Email/Phone] if you need help.</p>
                                <p>Best regards,<br>PRM AMBC</p>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div style='font-family: Calibri; color: #696969; font-size: 1.1em; margin-top: 20px; text-align: center;'>
                This is an automated email, please do not reply.
                <br />
                Automated mail from <a href='https://prm.ambctechnologies.com' style='color: #2693F8;'>https://prm.ambctechnologies.com</a>
            </div>
        </div>
    </body>
</html>";

                    var emailRequest = new EmailRequest()
                    {
                        Body = checkInBody,
                        ToEmail = checkinEmp.OfficalEmailid,
                        Subject = checkInSubject
                    };

                    var sendNotification = EMailHelper.SendEmail(emailRequest);
                }
            }



            // Checkout reminder emails
            foreach (var checkoutEmp in employeesToRemindCheckout)
            {
                if (checkoutEmp.ShiftEndTime.HasValue)
                {
                    var shiftEndTime = new DateTime(checkoutEmp.ShiftEndTime.Value.Ticks).ToString("HH:mm"); // 24-hour format
                    var checkOutSubject = "Do not forget to do your Check out!";
                    var checkOutBody = $@"
<html>
    <body style='font-family: Calibri, sans-serif; background-color: #f2f2f2; padding: 20px; margin: 0;'>
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto;'>
            <div style='padding: 20px; background-color: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='text-align: right; padding: 10px 0;'>
                            <a href='{siteURL}' target='_blank'>
                                <img src='{logoURL}' alt='AMBC Logo' width='50'>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p style='margin-top: 0; font-size: 1.1em;'>Hi <strong>{checkoutEmp.EmployeeName}</strong>,</p>
                            <div style='font-family: Calibri; color: #696969; margin-top: 20px; font-size: 1.1em;'>
                                <p>Reminder for Check Out</p>
                                <p>Please remember to complete your daily check out by {shiftEndTime}.</p>
                                <p>Contact [Support Email/Phone] if you need help.</p>
                                <p>Best regards,<br>PRM AMBC</p>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div style='font-family: Calibri; color: #696969; font-size: 1.1em; margin-top: 20px; text-align: center;'>
                This is an automated email, please do not reply.
                <br />
                Automated mail from <a href='https://prm.ambctechnologies.com' style='color: #2693F8;'>https://prm.ambctechnologies.com</a>
            </div>
        </div>
    </body>
</html>";

                    var emailRequest = new EmailRequest()
                    {
                        Body = checkOutBody,
                        ToEmail = checkoutEmp.OfficalEmailid,
                        Subject = checkOutSubject
                    };

                    var sendNotification = EMailHelper.SendEmail(emailRequest);
                }
            }
            return Task.CompletedTask;

        }

    }

}
