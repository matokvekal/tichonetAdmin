using FluentScheduler;

namespace ticonet.Scheduler
{
    public interface IAbstractTask : ITask { }

    public abstract class AbstractTask : IAbstractTask
    {
        public virtual void Execute()
        {
            //;
        }
    }
}