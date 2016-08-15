using log4net;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskExample : IAbstractTask { }

    public class TaskExample : AbstractTask, ITaskExample
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskExample));

        public override void Execute()
        {
            logger.Info("ExampleTask");
        }
    }
}