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
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='border: 1px solid #ddd; padding: 20px; max-width: 600px; margin: 0 auto;'>                        
                                <div style='padding: 20px;'>
                                    <div style='display: flex; align-items: center;'>
                                        <p style='margin: 0;'>Hi {checkinEmp.EmployeeName},</p>
                                        <a href='{logoURL}' target='_blank' style='margin-left: 10px;'>
                                            <img src='{logoURL}' alt='AMBC Logo' style='max-width: 50px;'>
                                        </a>
                                    </div>
                                    <p>Reminder for Check-In</p>
                                    <p>Please remember to complete your daily check-in by {shiftStartTime}.</p>
                                    <p>Contact [Support Email/Phone] if you need help.</p>
                                    <p>Best regards,<br>PRM AMBC</p>
                                </div>
                            </div>
                            <div style='text-align: center; padding: 10px; border-top: 1px solid #ddd;'>
                                <a href='{siteURL}' target='_blank'>{siteURL}</a>
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
                    var checkOutSubject = "Do not forget to do your Check-out!";
                    var checkOutBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='border: 1px solid #ddd; padding: 20px; max-width: 600px; margin: 0 auto;'>                        
                                <div style='padding: 20px;'>
                                    <div style='display: flex; align-items: center;'>
                                        <p style='margin: 0;'>Hi {checkoutEmp.EmployeeName},</p>
                                        <a href='{logoURL}' target='_blank' style='margin-left: 10px;'>
                                            <img src='{logoURL}' alt='AMBC Logo' style='max-width: 50px;'>
                                        </a>
                                    </div>
                                    <p>Reminder for Check-Out</p>
                                    <p>Please remember to complete your daily check-out by {shiftEndTime}.</p>
                                    <p>Contact [Support Email/Phone] if you need help.</p>
                                    <p>Best regards,<br>PRM AMBC</p>
                                </div>
                            </div>
                            <div style='text-align: center; padding: 10px; border-top: 1px solid #ddd;'>
                                <a href='{siteURL}' target='_blank'>{siteURL}</a>
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
