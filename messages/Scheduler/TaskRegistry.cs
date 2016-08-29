using FluentScheduler;
using ticonet.Scheduler.Tasks;

namespace ticonet.Scheduler
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            // Task Running Every Hours
            Schedule<ITaskEveryHours>().ToRunEvery(1).Hours();
        }
    }
}