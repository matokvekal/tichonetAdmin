using System;
using System.Collections.Generic;
using Business_Logic.SqlContext;

namespace Business_Logic.MessagesModule.Mechanisms {

    public static class TASK_PROTOTYPE {

        /// <summary>
        /// ISqlLogic sqlLogic = kernel.Get<ISqlLogic>();
        /// </summary>
        public static void RunBatchCreation(int minutesPeriod, ISqlLogic sqlLogic) {
            if (minutesPeriod < 1)
                throw new ArgumentOutOfRangeException("minutesPeriod should be greater or equals then 1");

            using (var manager = BatchCreationManager.NewInstance(minutesPeriod, sqlLogic)) {
                var shedules = manager.GetActualMessageSchedules();
                var creator = new BatchCreator(manager);
                var results = new List<BatchCreationResult>();
                foreach (var sched in shedules)
                    //TODO SET PRIORITY HERE:
                    results.Add(creator.CreateBatch(sched,0));

                manager.SaveResultsToDB(results);
            }

        }
    }
}
