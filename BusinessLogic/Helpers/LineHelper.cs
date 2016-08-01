using System;

namespace Business_Logic.Helpers
{
    public static class LineHelper
    {
        public static void RefreshActive(Line line)
        {
            if (line.IsActive)
            {
                line.Sun = line.Sun ?? false;
                line.Mon = line.Mon ?? false;
                line.Tue = line.Tue ?? false;
                line.Wed = line.Wed ?? false;
                line.Thu = line.Thu ?? false;
                line.Fri = line.Fri ?? false;
                line.Sut = line.Sut ?? false;
            }
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
