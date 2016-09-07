using FluentScheduler;
using ticonet.Scheduler.Tasks;

namespace ticonet.Scheduler
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            // Task Running Every Hours
            Schedule<ITaskSending>().ToRunEvery(5).Seconds();
        }
    }
}