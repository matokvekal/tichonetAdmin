using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects {

    public struct DateTimePeriod {
        readonly public DateTime Start;
        readonly public DateTime End;

        [JsonConstructor]
        public DateTimePeriod(DateTime Start, DateTime End) {
            if (End <= Start)
                throw new ArgumentException("End of period must be later then start");
            this.Start = Start;
            this.End = End;
        }

        public IEnumerable<DateTimePeriod> SplitToPeriodsByDays() {
            if (Start.Date == End.Date)
                return new[] { this };

            var st = Start;
            var end = End;
            var list = new List<DateTimePeriod>();
            while (st.Date != end.Date) {
                list.Add(new DateTimePeriod(st, st.AddDays(1).Date.AddMilliseconds(-1)));
                st = st.AddDays(1).Date;
            }
            if (st != end)
                list.Add(new DateTimePeriod(st, end));
            return list;
        }

    }
}
