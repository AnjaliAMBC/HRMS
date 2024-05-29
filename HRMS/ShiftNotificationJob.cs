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
                var shiftStartTime = checkinEmp.ShiftStartTime?.ToString("hh:mm tt");
                var checkInSubject = "Reminder: Your Shift Starts Soon at " + shiftStartTime;
                var checkInBody = $@"
                Dear {checkinEmp.EmployeeName},

                This is a reminder to check-in.

                Your shift begins at {shiftStartTime}. Ensure you have marked your attendance.

                Best regards,
                PRM AMBC.";

                var emailRequest = new EmailRequest()
                {
                    Body = checkInBody,
                    ToEmail = checkinEmp.OfficalEmailid,
                    Subject = checkInSubject
                };

                var sendNotification = EMailHelper.SendEmail(emailRequest);
            }


            //Checkout remainder emails
            foreach (var checkinEmp in employeesToRemindCheckout)
            {
                var shiftEndTime = checkinEmp.ShiftEndTime?.ToString("hh:mm tt");
                var checkOutSubject = "Reminder: Your Shift Ends Soon at 20:30 " + shiftEndTime;
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