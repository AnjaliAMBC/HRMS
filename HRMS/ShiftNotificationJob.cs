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

        // Constructor to initialize database context
        public ShiftNotificationJob()
        {
            _dbContext = new HRMS_EntityFramework(); // Replace YourDbContext with your actual DbContext class
        }


        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now.TimeOfDay;
            var checkinTime = now.Add(TimeSpan.FromMinutes(15));
            var checkoutTime = now.Add(TimeSpan.FromMinutes(-15));

            var checkinTimeRequired = checkinTime.Hours + ":" + checkinTime.Minutes;
            var checkoutRequiredTime = checkoutTime.Hours + ":" + checkoutTime.Minutes;

            var assicoiatedEmplyees = _dbContext.emp_info.Where(x => x.EmployeeStatus == "Active");
            foreach (var emp in assicoiatedEmplyees)
            {

                var checkInSubject = "TEST Reminder: Your Shift Starts Soon at 10:30";
                var checkInBody = $@"
            Dear {emp.EmployeeName},

            This is a friendly reminder that your shift starts soon at 10:30. Please make sure to check in by 10:30.

            Shift Details:
            - Start Time: 10:30
            - End Time: 20:30

            If you have any questions or need assistance, please contact your supervisor.

            Best regards,
            PRM AMBC.";


                var checkOutSubject = "TEST Reminder: Your Shift Ends Soon at 20:30";
                var checkOutBody = $@"
            Dear {emp.EmployeeName},

            This is a friendly reminder that your shift ends soon at 20:30. Please make sure to check out by 20:30.

            Shift Details:
            - Start Time: 10:30
            - End Time: 20:30

            If you have any questions or need assistance, please contact your supervisor.

            Thank you for your hard work today.

            Best regards,
            PRM AMBC.";

                var emailRequest = new EmailRequest()
                {
                    Body = checkOutBody,
                    ToEmail = emp.OfficalEmailid,
                    Subject = checkOutSubject
                };
                var sendNotification = EMailHelper.SendEmail(emailRequest);
            }
            return Task.CompletedTask;
        }
    }
}