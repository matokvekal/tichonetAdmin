using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects {

    public class SendProviderRestrictionDataLog {
        public class JobPeriod {
            public DateTimePeriod period;
            public int MessagesCount;
        }

        public List<JobPeriod> Jobs { get; set; }

        public SendProviderRestrictionDataLog() {
            Jobs = new List<JobPeriod>();
        }

        public SendProviderRestrictionDataLog RegisterJob (DateTimePeriod period, int MessagesWasSended) {
            Jobs.Add(new JobPeriod { period = period, MessagesCount = MessagesWasSended });
            return this;
        }

        public void ForgotJobsEndedEarlierThen(DateTime from) {
            Jobs = Jobs.Where(x => x.period.End > from).ToList();
        }

        public int GetMessagesSendedForPeriodCount (DateTimePeriod period) {
            return Jobs
                .Where(x => x.period.Start <= period.End && x.period.End >= period.Start)
                .Sum(x => x.MessagesCount);
        }
    }
}
