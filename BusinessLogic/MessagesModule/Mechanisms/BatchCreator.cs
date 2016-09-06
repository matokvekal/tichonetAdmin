using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.InnerLibs.Text2Graph;
using Business_Logic.SqlContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class BatchCreator : BatchCreationComponent {
        readonly BatchCreationManager _manager;

        public BatchCreator(BatchCreationManager manager) : base (manager) {
            _manager = manager;
        }
        /// <summary>
        /// Creates Batch, creates messages assigned to this batch, puts messages to send queque.
        /// returns entities NOT WRITTEN to db.
        /// 'sendPriority': int.min - lowest priory, int.max - highest.
        /// </summary>
        public BatchCreationResult CreateBatch (tblMessageSchedule schedule, int sendPriority) {
            var result = new BatchCreationResult();
            var batch = logic.Create<tblMessageBatch>();
            batch.CreatedOn = DateTime.Now;
            batch.tblMessageSchedule = schedule;

            var dataCollector = new MessageDataCollector(_manager);
            var msgData = dataCollector.Collect(schedule);

            var markSpecs = new DefaultMarkUpSpecification { NewLineSymbol = "\n" };
            var msgProducer = new MessageProducer(schedule.MsgHeader, schedule.MsgBody, null, markSpecs);

            var messages = new List<tblMessage>();
            var sendQueues = new List<tblPendingMessagesQueue>();

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
                    messages.Add(msg);
                    var pmq = logic.Create<tblPendingMessagesQueue>();
                    pmq.tblMessage = msg;
                    pmq.Priority = sendPriority;
                    sendQueues.Add(pmq);
                }
            }
            result.Batch = batch;
            result.Messages = messages;
            result.SendQueue = sendQueues;
            return result;
        }

    }
}
