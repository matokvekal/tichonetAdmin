using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.EntitiesExtensions {
    //do not change order, do not use custom value, all names must be uniq
    public enum ScheduleRepeatMode {
        None,
        EveryDay,
        EveryWeek,
        EveryMonth,
        EveryYear
    }

    public static class ScheduleRepeatModeHelper {

        public static ScheduleRepeatMode RepeatModeFromString(string str) {
            str = str.ToLower();
            switch (str) {
                case null:
                case "":
                case "none":
                case "never":
                    return ScheduleRepeatMode.None;
                case "day":
                    return ScheduleRepeatMode.EveryDay;
                case "week":
                    return ScheduleRepeatMode.EveryWeek;
                case "month":
                    return ScheduleRepeatMode.EveryMonth;
                case "year":
                    return ScheduleRepeatMode.EveryYear;
            }
            throw new ArgumentException();
        }

        public static string RepeatModeToString(ScheduleRepeatMode mode) {
            switch (mode) {
                case ScheduleRepeatMode.None:
                    return "none";
                case ScheduleRepeatMode.EveryDay:
                    return "day";
                case ScheduleRepeatMode.EveryWeek:
                    return "week";
                case ScheduleRepeatMode.EveryMonth:
                    return "month";
                case ScheduleRepeatMode.EveryYear:
                    return "year";
            }
            throw new ArgumentException();
        }
        static List<string> RepeatModesStrings;
        static ScheduleRepeatModeHelper() {
            RepeatModesStrings = new List<string>();
            foreach (ScheduleRepeatMode mode in Enum.GetValues(typeof(ScheduleRepeatMode))) {
                RepeatModesStrings.Add(RepeatModeToString(mode));
            }
        }
        public static string[] GetAllowedRepeatModeNames() {
            return RepeatModesStrings.ToArray();
        }
    }
}
