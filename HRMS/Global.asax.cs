using HRMS.Filters;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HRMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new CustomExceptionFilter());

            if (ConfigurationManager.AppSettings["DisableAutoNotification"] == "true")
            {
                return;
            }

            StartQuartzScheduler();
        }

        private void StartQuartzScheduler()
        {
            // Create a scheduler factory
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            // Get a scheduler
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            
            IJobDetail job = JobBuilder.Create<ShiftNotificationJob>()
                .WithIdentity("shiftNotificationJob", "group1")
                .Build();

           
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("shiftNotificationTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                .Build();


           
            IJobDetail job2 = JobBuilder.Create<GenerateAndSendReportJob>()
                .WithIdentity("dailycheckinreportjob", "group2")
                .Build();

            //Trigger the job to run every minute
            ITrigger trigger2 = TriggerBuilder.Create()
               .WithIdentity("DailycheckinreportjobTrigger", "group2")
              .WithCronSchedule("0 0 11,15 ? * MON-FRI")
               .Build();


            IJobDetail job3 = JobBuilder.Create<GenerateAndSendReportJob>()
               .WithIdentity("dailycheckinreportjob3", "group3")
               .Build();

            //Trigger the job to run every minute
            ITrigger trigger3 = TriggerBuilder.Create()
              .WithIdentity("DailycheckinreportjobTrigger_530PM", "group3")
              .WithCronSchedule("0 30 17 ? * MON-FRI")  // At 5:30 PM
              .Build();


            // Define the job and tie it to our ShiftNotificationJob class
            //IJobDetail job4 = JobBuilder.Create<HolidayNotificationJob>()
            //    .WithIdentity("CompOffIntimationmailJob", "group4")
            //    .Build();


            //ITrigger trigger4 = TriggerBuilder.Create()
            //  .WithIdentity("CompOffIntimationmailJobTrigger", "group4")
            //  .WithCronSchedule("0 0 10 * * ?")
            //  .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
            scheduler.ScheduleJob(job2, trigger2).Wait();
            scheduler.ScheduleJob(job3, trigger3).Wait();

            //scheduler.ScheduleJob(job4, trigger4).Wait();
        }
    }
}
