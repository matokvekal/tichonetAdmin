using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business_Logic.MessagesModule.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Tests;
using Business_Logic.SqlContext;

namespace Business_Logic.MessagesModule.Mechanisms.Tests {
    [TestClass()]
    public class MessageDataCollectorTests {
        [TestMethod()]
        public void MessageDataCollectorTest() {
        }

        [TestMethod()]
        public void CollectTest() {
            using (var logic = new MessagesModuleLogic(new MessageContext(Settings.MessageContextConnectionString))) {
                using (var sqlLogic = new SqlLogic(new SqlConnectionFactory(Settings.SqlConnectionString))) {
                    using (var Bcm = BatchCreationManager.NewInstance(new DateTime(2016, 9, 3), new DateTime(2016, 9, 5), sqlLogic, logic)) {
                        Bcm.DontMakeTimeStamp();
                        var sced = logic.Get<tblMessageSchedule>(1);
                        
                        var Collector = new MessageDataCollector(Bcm);
                        var result = Collector.Collect(sced);
                    }
                }
            }
        }
    }
}