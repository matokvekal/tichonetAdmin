using Business_Logic;
using Business_Logic.Services;
using log4net;
using System;

namespace ticonet.Scheduler.Tasks
{
    public interface ITaskPopulateLinesPlan : IAbstractTask { }

    public class TaskPopulateLinesPlan : AbstractTask, ITaskPopulateLinesPlan
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskPopulateLinesPlan));

        public override void Execute()
        {
            var scheduleService = new ScheduleService();
            
            using (var logic = new tblSettingLogic())
            {
                if(logic.PopulateLinesIsActive)
                {
                    logger.Info("PopulateLinesPlan");
                    var result = scheduleService.PopulateLinesPlan();
                    if (result)
                        logic.PopulateLinesLastRun = DateTime.UtcNow;
                }
            }
        }
    }
}