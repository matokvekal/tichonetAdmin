using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Helpers
{
    public static class LineHelper
    {
        public static void RefreshActive(Line line)
        {
            bool isActive = line.IsActive;
            if ((!(line.Sun.HasValue && line.Sun.Value)) &&
                (!(line.Mon.HasValue && line.Mon.Value)) &&
                (!(line.Tue.HasValue && line.Tue.Value)) &&
                (!(line.Wed.HasValue && line.Wed.Value)) &&
                (!(line.Thu.HasValue && line.Thu.Value)) &&
                (!(line.Fri.HasValue && line.Fri.Value)) &&
                (!(line.Sut.HasValue && line.Sut.Value)))
                isActive = false;
            line.IsActive = isActive;
        }

        static bool checkNullBool (bool? b, bool defval) {
            return b == null ? defval : b.Value;
        }

        public static bool IsLineActiveAtDay(Line line,DayOfWeek dow) {
            switch (dow) {
                case DayOfWeek.Sunday:
                    return checkNullBool(line.Sun, false);
                case DayOfWeek.Monday:
                    return checkNullBool(line.Mon, false);
                case DayOfWeek.Tuesday:
                    return checkNullBool(line.Tue, false);
                case DayOfWeek.Wednesday:
                    return checkNullBool(line.Wed, false);
                case DayOfWeek.Thursday:
                    return checkNullBool(line.Thu, false);
                case DayOfWeek.Friday:
                    return checkNullBool(line.Fri, false);
                case DayOfWeek.Saturday:
                    return checkNullBool(line.Sut, false);
                default:
                    throw new ArgumentException();
            }
        }

    }
}
