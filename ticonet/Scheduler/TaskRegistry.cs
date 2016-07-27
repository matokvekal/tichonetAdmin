using FluentScheduler;
using ticonet.Scheduler.Tasks;

namespace ticonet.Scheduler
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            // Run every week on Saturday at 23:00
            Schedule<ITaskPopulateLinesPlan>().ToRunEvery(1).Weeks().On(System.DayOfWeek.Saturday).At(23, 00);

            //TODO for test only
            //Schedule<ITaskPopulateLinesPlan>().ToRunNow();
        }
    }
}