using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static Business_Logic.MessagesModule.Mechanisms.BatchCreationManager;
using Business_Logic_Tests;
using Business_Logic.SqlContext;
using System.Collections.Generic;
using Business_Logic.MessagesModule.DataObjects;
using Newtonsoft.Json;

namespace Business_Logic.MessagesModule.Mechanisms.Tests {
    [TestClass]
    public class BatchCreationManagerTests {

        [TestMethod]
        public void PeriodSpliting() {
            var addedDays = 10;
            var p = new DateTimePeriod(DateTime.Now, DateTime.Now.AddDays(addedDays));
            var result = p.SplitToPeriodsByDays();
            Assert.IsTrue(result != null && result.Count() == addedDays + 1);
        }

        [TestMethod]
        public void PeriodSpliting_NoExceptions() {
            var p = new DateTimePeriod(new DateTime(2016, 9, 3), new DateTime(2016, 9, 5));
            var result = p.SplitToPeriodsByDays();
        }


        [TestMethod]
        public void SchedulesGetting_NoExceptions() {
            using (var logic = new MessagesModuleLogic(new MessageContext(Settings.MessageContextConnectionString))) {
                //SqlLogic isn't needed in this test.
                using (var Bcm = NewInstance(new DateTime(2015,9,3), new DateTime(2017, 9, 5), null, logic)) {
                    Bcm.DontMakeTimeStamp();
                    var schedules = Bcm.GetActualMessageSchedules();
                }
            }
        }

        [TestMethod]
        public void BigTask_Example() {
            using (var logic = new MessagesModuleLogic(new MessageContext(Settings.MessageContextConnectionString))) {
                using (var sqlLogic = new SqlLogic(new SqlConnectionFactory(Settings.SqlConnectionString))) {
                    using (var manager = NewInstance(new DateTime(2015, 9, 3), new DateTime(2017, 9, 5), sqlLogic, logic)) {
                        var shedules = manager.GetActualMessageSchedules();
                        var creator = new BatchCreator(manager);
                        var results = new List<BatchCreationResult>();
                        foreach (var sched in shedules)
                            results.Add(creator.CreateBatch(sched, 0));
                        //manager.SaveResultsToDB(results);
                    }
                }
            }
        }

        [TestMethod]
        public void DateTimePeriodSerializationTest () {
            DateTimePeriod per = new DateTimePeriod(DateTime.Now, DateTime.Now.AddDays(1));
            string result = JsonConvert.SerializeObject(per);

            var per2 = JsonConvert.DeserializeObject<DateTimePeriod>(result);

            Assert.IsTrue(per.Start == per2.Start && per.End == per2.End);
        }



    }


}
