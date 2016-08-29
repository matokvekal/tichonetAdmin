using log4net;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskEveryHours : IAbstractTask { }

    public class TaskEveryHours : AbstractTask, ITaskEveryHours
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskEveryHours));

        public override void Execute()
        {
            //logger.Info("TaskEveryHours");
            //TODO: sending service
        }
    }
}