using Business_Logic.MessagesModule;
using Business_Logic.MessagesModule.Mechanisms;
using log4net;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskSending : IAbstractTask { }

    public class TaskSending : AbstractTask, ITaskSending
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskSending));
        private bool enabled = true;

        public override void Execute()
        {
            //logger.Info("");
            if (enabled)
            {
                enabled = false;

                
                using (var logic = new MessagesModuleLogic(new MessageContext()))
                {
                    TASK_PROTOTYPE.RunScheduledBatchSending(logic);
                }
                enabled = true;
            }
        }
    }
}