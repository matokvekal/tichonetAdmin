using Business_Logic.MessagesModule.EntitiesExtensions;
using Business_Logic.SqlContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {

    /// <summary>
    /// Controls specific time period and engages BatchCreationManager to create batches.
    /// This entity generaly exists for avoiding doubled same batch creation, and avoiding errors 
    /// from interrupting for some reasons creation process.
    /// MUST BE USED in 'disposable' block:
    /// all BatchCreation Components exists and works only in this block.
    /// ONLY after ALL components finished it's jobs, BatchCreationManager can attempt 
    /// save the results to db.
    /// </summary>
    public class BatchCreationManager : IDisposable {

        public readonly DateTime periodStart;
        public readonly DateTime periodEnd;
        public readonly MessagesModuleLogic Logic;

        /// <summary>
        /// Creates new instance on control period. 
        /// Start of control period is getted from DB, next created Instances will start at this instance end.
        /// Period start on start INCLUDE, and ends on end EXCLUDE.
        /// Manages all lifecycle of batch creating, provides instance of MessagesModuleLogic
        /// for all connected to it BatchCreationComponents
        /// </summary>
        public static BatchCreationManager NewInstance(int controlledPeriodMinutes) {
            //TODO ADD BATCH_SEND PRIORITY HANDLING
            //TODO ADD NO_TIMED CONSTRUCTOR
            //TODO AVOID THE NEXT IMPLICITLY: PERIOD STARTS IN ONE DAY AND END IN NEXT!
            //TODO u can manage here uncovered periods...
            //TODO get from db start time
            DateTime controlledPeriodStart = getStartDate();
            DateTime controlledPeriodEnd = controlledPeriodStart.AddMinutes(controlledPeriodMinutes);

            return new BatchCreationManager(controlledPeriodStart, controlledPeriodStart);
        }

        /// <summary>
        /// Return tblMessageSchedules valid for this time period
        /// </summary>
        public IQueryable<tblMessageSchedule> GetActualMessageSchedules() {
            string repeatMode_none = ScheduleRepeatModeHelper.RepeatModeToString(ScheduleRepeatMode.None);
            string repeatMode_day = ScheduleRepeatModeHelper.RepeatModeToString(ScheduleRepeatMode.EveryDay);
            string repeatMode_week = ScheduleRepeatModeHelper.RepeatModeToString(ScheduleRepeatMode.EveryWeek);
            string repeatMode_month = ScheduleRepeatModeHelper.RepeatModeToString(ScheduleRepeatMode.EveryMonth);
            string repeatMode_year = ScheduleRepeatModeHelper.RepeatModeToString(ScheduleRepeatMode.EveryYear);

            DateTime firstSunday = new DateTime(1753, 1, 7);
            int DayOfWeekNow = (int)periodStart.DayOfWeek;

            var scheds =
                Logic.GetFilteredQueryable<tblMessageSchedule>()
                .Where(x => x.IsActive && (x.InArchive == null || !x.InArchive.Value))
                .Where(x =>
                    (x.RepeatMode == repeatMode_none 
                        && x.ScheduleDate >= periodStart && x.ScheduleDate < periodEnd
                    ) 
                    ||
                    (EntityFunctions.CreateTime(x.ScheduleDate.Value.Hour, x.ScheduleDate.Value.Minute, x.ScheduleDate.Value.Second) >= periodStart.TimeOfDay
                        &&  EntityFunctions.CreateTime(x.ScheduleDate.Value.Hour, x.ScheduleDate.Value.Minute, x.ScheduleDate.Value.Second) < periodEnd.TimeOfDay
                        &&  (x.RepeatMode == repeatMode_day
                            || (x.RepeatMode == repeatMode_week
                                && EntityFunctions.DiffDays(firstSunday, x.ScheduleDate) % 7 == DayOfWeekNow
                            )
                            || (x.RepeatMode == repeatMode_month
                                && x.ScheduleDate.Value.Day == periodStart.Day
                            )
                            || (x.RepeatMode == repeatMode_year
                                && x.ScheduleDate.Value.Month == periodStart.Month
                                && x.ScheduleDate.Value.Day == periodStart.Day
                            )
                        )
                    )
                );
            return scheds;
        }

        public void SaveResultsToDB (IEnumerable<BatchCreationResult> results) {
            //TODO HANDLE DOUBLE-SAVE
            Logic.AddRange(results.Select(x => x.Batch));
            Logic.AddRange(results.SelectMany(x => x.Messages));
            WriteEndDate(periodEnd);
        }

        //---------------------------------------------
        //private part

        static DateTime getStartDate () {
            return DateTime.Now;
        }

        void WriteEndDate (DateTime end) {
            //TODO WriteEndDate
        }

        //TODO use this list and static _lock to implement singletone with queque
        static List<BatchCreationManager> stack = new List<BatchCreationManager>();

        private BatchCreationManager(DateTime controlledPeriodStart, DateTime controlledPeriodEnd) {
            Logic = new MessagesModuleLogic();
            periodStart = controlledPeriodStart;
            periodEnd = controlledPeriodEnd;
            stack.Add(this);
        }

        #region IDisposable Support
        bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    stack.Remove(this);
                }
                Logic.Dispose();
                //dispose unmanaged resources here
                disposedValue = true;
            }
        }
        public bool IsDisposed { get { return disposedValue; } }
        public void Dispose() {
            Dispose(true);
        }
        #endregion

    }
}
