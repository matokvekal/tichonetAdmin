using Business_Logic.MessagesModule;
using Business_Logic.MessagesModule.Mechanisms;
using log4net;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskSending : IAbstractTask { }

    public class TaskSending : AbstractTask, ITaskSending
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskSending));

        static readonly object _lock = new object();

        public override void Execute(){
            lock (_lock) {
                using (var logic = new MessagesModuleLogic(new MessageContext()))
                {
                    TASK_PROTOTYPE.RunScheduledBatchSending(logic);
                }
            }
        }
    }
}