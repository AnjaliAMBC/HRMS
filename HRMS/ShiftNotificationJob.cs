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

            //Checkin Remainder emails
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
                        <div style='padding: 10px; border-bottom: 1px solid #ddd; background-color: #f8f8f8;'>
                            <strong style='color: red;'>WARNING:</strong> This email originated outside of AMBC. 
                            <strong>DO NOT CLICK</strong> links or attachments unless you recognize the sender and know the content is safe.
                        </div>
                        <div style='padding: 20px;'>
                            <p>Hi {checkinEmp.EmployeeName},</p>
                            <p>This is a reminder to check-in.</p>
                            <p>Your shift begins at {shiftStartTime}. Ensure you have marked your attendance.</p>
                            <p>Best regards,<br>PRM AMBC</p>
                        </div>
                        <div style='text-align: center; padding: 10px; border-top: 1px solid #ddd;'>
                            <img src='https://prm.ambctechnologies.com/logo.png' alt='AMBC Logo' style='max-width: 100px;'>
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
            foreach (var checkinEmp in employeesToRemindCheckout)
            {
                var shiftEndTime = checkinEmp.ShiftEndTime?.ToString("hh:mm tt");
                var checkOutSubject = "Do not forget to do your Check-out!";
                var checkOutBody = $@"
                Dear {checkinEmp.EmployeeName},
                This is a reminder to check-out.
                Your shift ends at {shiftEndTime}. Ensure you have checked out.
                Best regards,
                PRM AMBC.";

                var emailRequest = new EmailRequest()
                {
                    Body = checkOutBody,
                    ToEmail = checkinEmp.OfficalEmailid,
                    Subject = checkOutSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);
            }
            return Task.CompletedTask;
        }
    }
}