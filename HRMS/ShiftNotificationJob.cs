using HRMS.Helpers;
using HRMS.Models.Employee;
using Quartz;
using System;
using System.Linq;
using System.Net.Mail;
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

            var employeesToRemindCheckin = _dbContext.emp_info
            .Where(emp => emp.EmployeeStatus == "Active")
            .AsEnumerable()
            .Where(emp =>
            {
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
            var companyURL = siteURL; // Assuming company URL is the same as site URL, adjust if needed

            // Checkin Reminder emails
            foreach (var checkinEmp in employeesToRemindCheckin)
            {
                if (checkinEmp.ShiftStartTime != null && checkinEmp.ShiftStartTime.HasValue)
                {
                    DateTime time = DateTime.ParseExact(checkinEmp.ShiftStartTime.Value.ToString(), "HH:mm:ss", null);
                    string shiftStartTime = time.ToString("hh:mm tt");
                    var checkInSubject = "Do not forget to do your Check-in!";
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
                            <p>This is a reminder to check-in.</p>
                            <p>Your shift begins at {shiftStartTime}. Ensure you have marked your attendance.</p>
                            <p>Best regards,<br>PRM AMBC</p>
                        </div>
                        <div style='text-align: center; padding: 10px; border-top: 1px solid #ddd;'>
                            <a href='{siteURL}' target='_blank'>{siteURL}</a>
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



            //Checkout remainder emails
            foreach (var checkoutEmp in employeesToRemindCheckout)
            {
                if (checkoutEmp.ShiftEndTime != null && checkoutEmp.ShiftEndTime.HasValue)
                {
                    DateTime time = DateTime.ParseExact(checkoutEmp.ShiftEndTime.Value.ToString(), "HH:mm:ss", null);
                    string shiftEndTime = time.ToString("hh:mm tt");
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
                            <p>This is a reminder to check-out.</p>
                            <p>Your shift ends at {shiftEndTime}. Ensure you have checked out.</p>
                            <p>Best regards,<br>PRM AMBC</p>
                        </div>
                        <div style='text-align: center; padding: 10px; border-top: 1px solid #ddd;'>
                            <a href='{siteURL}' target='_blank'>{siteURL}</a>
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