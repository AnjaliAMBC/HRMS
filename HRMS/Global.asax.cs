﻿using HRMS.Filters;
using Quartz;
using Quartz.Impl;
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

            StartQuartzScheduler();
        }

        private void StartQuartzScheduler()
        {
            // Create a scheduler factory
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            // Get a scheduler
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            // Define the job and tie it to our ShiftNotificationJob class
            IJobDetail job = JobBuilder.Create<ShiftNotificationJob>()
                .WithIdentity("shiftNotificationJob", "group1")
                .Build();

            // Trigger the job to run every minute
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("shiftNotificationTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(15).RepeatForever())
                .Build();



            // Define the job and tie it to our ShiftNotificationJob class
            IJobDetail job2 = JobBuilder.Create<GenerateAndSendReportJob>()
                .WithIdentity("dailycheckinreportjob", "group2")
                .Build();

            // Trigger the job to run every minute
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("DailycheckinreportjobTrigger", "group2")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                .Build();

            // Define the job and tie it to our ShiftNotificationJob class
            IJobDetail job3 = JobBuilder.Create<HolidayNotificationJob>()
                .WithIdentity("CompOffIntimationmailJob", "group3")
                .Build();

            // Trigger the job to run every minute
            ITrigger trigger3 = TriggerBuilder.Create()
                .WithIdentity("CompOffIntimationmailJobTrigger", "group3")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                .Build();

            //scheduler.ScheduleJob(job, trigger).Wait();
            //scheduler.ScheduleJob(job2, trigger2).Wait();
            //scheduler.ScheduleJob(job3, trigger3).Wait();
        }
    }
}
