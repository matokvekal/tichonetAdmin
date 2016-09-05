using System;
using System.Collections.Generic;
using Business_Logic.SqlContext;

namespace Business_Logic.MessagesModule.Mechanisms {

    public static class TASK_PROTOTYPE {

        /// <summary>
        /// SAVE RESULTED MESSAGES TO PENDING DATABASE
        /// 
        /// int minutesPeriod = 5;
        /// ISqlLogic sqlLogic = kernel.Get<ISqlLogic>();
        /// TASK_PROTOTYPE.Run(minutesPeriod, sqlLogic);
        /// </summary>
        /// <param name="minutesPeriod">minutesPeriod should be greater on equals then 1</param>
        /// <param name="sqlLogic">ISqlLogic sqlLogic = kernel.Get<ISqlLogic>();</param>
        public static void Run(int minutesPeriod, ISqlLogic sqlLogic) {
            if (minutesPeriod < 1)
                throw new ArgumentOutOfRangeException("minutesPeriod should be greater on equals then 1");

            using (var manager = BatchCreationManager.NewInstance(minutesPeriod)) {
                var shedules = manager.GetActualMessageSchedules();
                var creator = new BatchCreator(manager, sqlLogic);
                var results = new List<BatchCreationResult>();
                foreach (var sched in shedules)
                    results.Add(creator.CreateBatch(sched));
                //TODO SAVE RESULTED MESSAGES TO PENDING DATABASE!
                manager.SaveResultsToDB(results);
            }

        }
    }
}
