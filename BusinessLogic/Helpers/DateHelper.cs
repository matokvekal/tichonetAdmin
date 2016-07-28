using System;
using System.Globalization;

namespace Business_Logic.Helpers
{
    public static class DateHelper
    {
        public static string DateToString(DateTime? dt)
        {
            return (dt.HasValue ? dt.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "");
        }

        public static DateTime? StringToDate(string s)
        {
            DateTime? dtNull = null;
            if (!string.IsNullOrWhiteSpace(s))
            {
                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dtNull = (DateTime?)dt;
            }
            return dtNull;
        }

        public static string TimeToString(DateTime? dt)
        {
            return (dt.HasValue ? dt.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : "");
        }

        public static DateTime? StringToTime(string s)
        {
            DateTime? dtNull = null;
            if (!string.IsNullOrWhiteSpace(s))
            {
                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dtNull = (DateTime?)dt;
            }
            return dtNull;
        }

        public static void IterDays(int dayCount, Action<int> iterator) {
            for (int i = 1; i <= dayCount; i++) {
                iterator(i);
            }
        }

        public static void IterDays(DateTime startFrom, int dayCount, Action<int, DateTime> iterator) {
            for (int i = 1; i <= dayCount; i++) {
                iterator(i, startFrom);
                startFrom = startFrom.AddDays(1);
            }
        }

        public static int GetDatesPeriodInDays(DateTime periodStart_incl, DateTime periodEnd_excl) {
            return periodEnd_excl.Subtract(periodStart_incl).Days;
        }

        public static DateTime GetSunday(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }
            return date;
        }
    }
}
