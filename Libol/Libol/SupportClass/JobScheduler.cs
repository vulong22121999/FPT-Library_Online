using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace Libol.SupportClass
{
    public class JobScheduler
    {
        public static void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = schedFact.GetScheduler().GetAwaiter().GetResult();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<BorrowedOrderJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            //Test WithIntervalInSeconds = 120s

            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("trigger1", "group1")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(120)
            //        .RepeatForever())
            //    .Build();

            //scheduler.ScheduleJob(job, trigger);
        }
    }
}