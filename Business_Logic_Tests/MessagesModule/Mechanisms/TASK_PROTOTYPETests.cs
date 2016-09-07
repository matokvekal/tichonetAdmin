using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business_Logic.MessagesModule.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.SqlContext;
using Business_Logic_Tests;

namespace Business_Logic.MessagesModule.Mechanisms.Tests {
    [TestClass()]
    public class TASK_PROTOTYPETests {
        [TestMethod()]
        public void RunScheduledBatchCreationTest() {
            Assert.Fail();
        }

        [TestMethod()]
        public void RunImmediateBatchCreationTest() {
            using (var logic = new MessagesModuleLogic(new MessageContext(Settings.MessageContextConnectionString))) {
                using (var sqlLogic = new SqlLogic(new SqlConnectionFactory(Settings.SqlConnectionString))) {
                    using (var Bcm = BatchCreationManager.NewInstance(new DateTime(2016, 9, 3), new DateTime(2016, 9, 5), sqlLogic, logic)) {
                        //TASK_PROTOTYPE.RunImmediateBatchCreation(logic.Get<tblMessageSchedule>(1), 1, sqlLogic, logic);
                    }
                }
            }
        }

        [TestMethod()]
        public void RunScheduledBatchSendingTest() {
            using (var logic = new MessagesModuleLogic(new MessageContext(Settings.MessageContextConnectionString))) {
                //TASK_PROTOTYPE.RunScheduledBatchSending(logic);
            }
            //TASK_PROTOTYPE.RunScheduledBatchSending(null);

        }
    }
}