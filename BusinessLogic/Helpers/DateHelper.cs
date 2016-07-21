using System;
using System.Globalization;

namespace Business_Logic.Helpers
{
    public class DateHelper
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

    }
}
