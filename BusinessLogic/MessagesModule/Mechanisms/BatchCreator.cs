using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.InnerLibs.Text2Graph;
using Business_Logic.SqlContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class BatchCreationResult {
        public tblMessageBatch Batch;
        public IEnumerable<tblMessage> Messages;
    }

    public class BatchCreator : BatchCreationComponent {
        readonly BatchCreationManager _manager;
        readonly ISqlLogic _sqlLogic;

        public BatchCreator(BatchCreationManager manager, ISqlLogic sqlLogic) : base (manager) {
            _manager = manager;
            _sqlLogic = sqlLogic;
        }
        /// <summary>
        /// Creates Batch, creates messages assigned to this batch.
        /// returns entities NOT WRITTEN to db,
        /// </summary>
        public BatchCreationResult CreateBatch (tblMessageSchedule schedule) {
            var result = new BatchCreationResult();
            var batch = logic.Create<tblMessageBatch>();
            batch.CreatedOn = DateTime.Now;
            batch.tblMessageSchedule = schedule;

            var dataCollector = new MessageDataCollector(_manager, _sqlLogic);
            var msgData = dataCollector.Collect(schedule);

            var markSpecs = new DefaultMarkUpSpecification { NewLineSymbol = "\n" };
            var msgProducer = new MessageProducer(schedule.MsgHeader, schedule.MsgBody, null, markSpecs);

            var messages = new List<tblMessage>();
            foreach(var data in msgData) {
                msgProducer.ChangeWildCards(data.wildCards);
                foreach (var textData in data.TextProductionData) {
                    var msgRaw = msgProducer.Produce(textData, schedule.IsSms ? MessageType.Sms : MessageType.Sms);
                    var msg = logic.Create<tblMessage>();
                    msg.Adress = msgRaw.Adress;
                    msg.Body = msgRaw.Body;
                    msg.Header = msgRaw.Header;
                    msg.IsSms = schedule.IsSms;
                    msg.tblMessageBatch = batch;
                }
            }
            result.Batch = batch;
            result.Messages = messages;
            return result;
        }

    }
}
