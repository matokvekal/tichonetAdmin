using Business_Logic.MessagesModule.DataObjects;
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

    public class BatchCreationResult {
        public tblMessageBatch Batch;
        public IEnumerable<tblMessage> Messages;
        public IEnumerable<tblPendingMessagesQueue> SendQueue;
    }

    /// <summary>
    /// Controls specific time period and engages BatchCreator to create batches.
    /// This entity generaly exists for avoiding doubled same batch creation, and avoiding errors 
    /// from interrupting for some reasons creation process;
    /// Manages all lifecycle of batch creating, provides instance of MessagesModuleLogic
    /// for all connected to it BatchCreationComponents.
    /// MUST BE USED in 'disposable' block:
    /// all BatchCreation Components exists and works only in this block.
    /// ONLY after ALL components finished it's jobs, BatchCreationManager can attempt 
    /// save the results to db.
    /// </summary>
    public class BatchCreationManager : IDisposable {

        public readonly DateTime periodStart;
        public readonly DateTime periodEnd;

        public readonly MessagesModuleLogic Logic;
        public readonly ISqlLogic SqlLogic;

        /// <summary>
        /// Creates new instance on control period. 
        /// Start of control period is getted from DB, end is: start+checkMinutesFromLastInvoke.
        /// Period start on start INCLUDE, and ends on end INCLUDE.
        /// If no MessagesModuleLogic passed, new instance of it will be created and disposed after usage.
        /// </summary>
        public static BatchCreationManager NewInstance(DateTime startDate, int checkMinutesFromLastInvoke, ISqlLogic SqlLogic, MessagesModuleLogic Logic = null) {
            throw new NotImplementedException();
            //TODO add logic to compare given period with period from db...
            DateTime controlledPeriodStart = getStartDate();
            DateTime controlledPeriodEnd = controlledPeriodStart.AddMinutes(checkMinutesFromLastInvoke);

            return new BatchCreationManager(controlledPeriodStart, controlledPeriodEnd, Logic, SqlLogic);
        }

        /// <summary>
        /// Creates new instance on control period. 
        /// Period start on start INCLUDE, and ends on end INCLUDE.
        /// If no MessagesModuleLogic passed, new instance of it will be created and disposed after usage.
        /// </summary>
        public static BatchCreationManager NewInstance(DateTime controlledPeriodStart, DateTime controlledPeriodEnd, ISqlLogic SqlLogic, MessagesModuleLogic Logic = null) {
            return new BatchCreationManager(controlledPeriodStart, controlledPeriodEnd, Logic, SqlLogic);
        }

        /// <summary>
        /// Creates new instance without control period. No data about job period will written to db.
        /// GetActualMessageSchedules() will throw error if called. 
        /// If no MessagesModuleLogic passed, new instance of it will be created and disposed after usage.
        /// </summary>
        public static BatchCreationManager NewInstance(ISqlLogic SqlLogic, MessagesModuleLogic Logic = null) {
            return new BatchCreationManager(default(DateTime), default(DateTime), Logic, SqlLogic)
                .ClearPeriod()
                .DontMakeTimeStamp();
        }

        /// <summary>
        /// Return tblMessageSchedules valid for this BatchCreationManager time period
        /// </summary>
        public IQueryable<tblMessageSchedule> GetActualMessageSchedules() {
            if (_NoPeriod)
                throw new InvalidOperationException("BatchCreationManager has no valid period setted");
            var RepeatModes = new RepeatModeStrings();

            IQueryable<tblMessageSchedule> result = null;

            foreach (var period in new DateTimePeriod(periodStart,periodEnd).SplitToPeriodsByDays())
                result = GetSchedsForDayPeriod(period.Start, period.End, result);

            return result;
        }

        /// <summary>
        /// (!) Here we check period including start and including end!
        /// </summary>
        IQueryable<tblMessageSchedule> GetSchedsForDayPeriod(DateTime start, DateTime end, IQueryable<tblMessageSchedule> income) {
            int DayOfWeekNow = (int)start.DayOfWeek;

            var scheds =
                Logic.GetFilteredQueryable<tblMessageSchedule>()
                .Where(x => x.IsActive && (x.InArchive == null || !x.InArchive.Value))
                .Where(x =>
                    //CHECK ACTUALITY... x.ScheduleDate >= DateTime.Now - x.Actuality
                    //U WRONG, NOT HERE
                    //&&
                    (
                        (x.RepeatMode == RepeatModes.repeatMode_none
                            && x.ScheduleDate >= start && x.ScheduleDate <= end
                        )
                        ||
                        (DbFunctions.CreateTime(x.ScheduleDate.Value.Hour, x.ScheduleDate.Value.Minute, x.ScheduleDate.Value.Second) >= start.TimeOfDay
                            && DbFunctions.CreateTime(x.ScheduleDate.Value.Hour, x.ScheduleDate.Value.Minute, x.ScheduleDate.Value.Second) <= end.TimeOfDay
                            && (x.RepeatMode == RepeatModes.repeatMode_day
                                || (x.RepeatMode == RepeatModes.repeatMode_week
                                    && DbFunctions.DiffDays(firstSunday, x.ScheduleDate) % 7 == DayOfWeekNow
                                )
                                || (x.RepeatMode == RepeatModes.repeatMode_month
                                    && x.ScheduleDate.Value.Day == start.Day
                                )
                                || (x.RepeatMode == RepeatModes.repeatMode_year
                                    && x.ScheduleDate.Value.Month == start.Month
                                    && x.ScheduleDate.Value.Day == start.Day
                                )
                            )
                        )
                    )
                );

            income = income == null ? scheds : income.Concat(scheds);
            return income;
        }

        public void SaveResultsToDB (IEnumerable<BatchCreationResult> results) {
            //TODO HANDLE DOUBLE-SAVE
            Logic.AddRange(results.Select(x => x.Batch));
            Logic.AddRange(results.SelectMany(x => x.Messages));
            Logic.AddRange(results.SelectMany(x => x.SendQueue));
            if (_MakeTimeStamp)
                WriteEndDate(periodEnd);
        }

        bool _MakeTimeStamp = true;
        /// <summary>
        /// The BatchCreationManager will not write to db information about his job start and end time.
        /// </summary>
        public BatchCreationManager DontMakeTimeStamp () {
            _MakeTimeStamp = false;
            return this;
        }

        bool _NoPeriod = false;

        public BatchCreationManager ClearPeriod() {
            _NoPeriod = true;
            return this;
        }

        //---------------------------------------------
        //private part

        RepeatModeStrings RepeatModes = new RepeatModeStrings();

        DateTime firstSunday = new DateTime(1753, 1, 7);

        static DateTime getStartDate () {
            //returns last invokation end Data + 1 milisecond
            using (var l = new MessagesModuleLogic()) {
                var last = l.GetAll<tblBatchCreationManagerData>().FirstOrDefault();
                if (last == null)
                    return DateTime.Now.AddYears(-1);
                return last.LastEndTime.AddMilliseconds(1);
            }
        }



        void WriteEndDate (DateTime end) {
            if (_NoPeriod)
                throw new InvalidOperationException("Can't save job period, cos no period was setted");
            var single = Logic.GetAll<tblBatchCreationManagerData>().FirstOrDefault();
            if (single == null)
                single = Logic.Create<tblBatchCreationManagerData>();
            single.LastStartTime = periodStart;
            single.LastEndTime = periodStart;
            Logic.Save(single);
        }

        //TODO use this list and static _lock to implement singletone with queque
        static List<BatchCreationManager> stack = new List<BatchCreationManager>();

        private BatchCreationManager(DateTime controlledPeriodStart, DateTime controlledPeriodEnd, MessagesModuleLogic Logic, ISqlLogic SqlLogic) {
            this.SqlLogic = SqlLogic;
            this.Logic = Logic ?? new MessagesModuleLogic();
            AutoDisposeLogic = Logic == null;
            periodStart = controlledPeriodStart;
            periodEnd = controlledPeriodEnd;
            stack.Add(this);
        }

        #region IDisposable Implementation
        bool disposedValue = false; // To detect redundant calls
        readonly bool AutoDisposeLogic;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    stack.Remove(this);
                }
                if (AutoDisposeLogic)
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
