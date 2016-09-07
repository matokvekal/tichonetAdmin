using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business_Logic.MessagesModule.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects.Tests {
    [TestClass()]
    public class SendProviderRestrictionDataLog_Tests {
        [TestMethod()]
        public void SendProviderRestrictionDataLog_Test() {
            new SendProviderRestrictionDataLog();
        }

        [TestMethod()]
        public void RegisterJob_Test() {
            var log = new SendProviderRestrictionDataLog()
                .RegisterJob(new DateTimePeriod(DateTime.Now, DateTime.Now.AddDays(1)), 1);
        }

        [TestMethod()]
        public void ForgotJobsEndedEarlierThen_Test() {
            var Date = DateTime.Now;
            var log = new SendProviderRestrictionDataLog()
                .RegisterJob(new DateTimePeriod(Date, Date.AddDays(1)), 1)
                .RegisterJob(new DateTimePeriod(Date.AddDays(1), Date.AddDays(2)), 1)
                .RegisterJob(new DateTimePeriod(Date.AddDays(2), Date.AddDays(3)), 1)
                .RegisterJob(new DateTimePeriod(Date.AddDays(3), Date.AddDays(5)), 1);
            log.ForgotJobsEndedEarlierThen(Date.AddDays(2));
            Assert.AreEqual(2, log.Jobs.Count);
        }

        [TestMethod()]
        public void GetMessagesSendedForPeriodCount_Test() {
            var Date = DateTime.Now;
            var log = new SendProviderRestrictionDataLog()
                .RegisterJob(new DateTimePeriod(Date, Date.AddDays(1)), 1)
                .RegisterJob(new DateTimePeriod(Date.AddDays(2), Date.AddDays(3)), 1)
                .RegisterJob(new DateTimePeriod(Date.AddDays(3), Date.AddDays(5)), 1);
            int result = log.GetMessagesSendedForPeriodCount
                (new DateTimePeriod(Date.AddDays(1).AddMinutes(1), Date.AddDays(6)));
            Assert.AreEqual(2, result);
            result = log.GetMessagesSendedForPeriodCount
                (new DateTimePeriod(Date.AddDays(10).AddMinutes(1), Date.AddDays(16)));
            Assert.AreEqual(0, result);
        }
    }
}