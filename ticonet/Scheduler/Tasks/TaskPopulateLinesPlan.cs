using Business_Logic.Services;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskPopulateLinesPlan : IAbstractTask { }

    public class TaskPopulateLinesPlan : AbstractTask, ITaskPopulateLinesPlan
    {
        public override void Execute()
        {
            var scheduleService = new ScheduleService();
            //var result = scheduleService.PopulateLinesPlan();
        }
    }
}