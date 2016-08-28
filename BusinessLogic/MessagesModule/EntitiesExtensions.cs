using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.MessagesModule {

    public interface IMessagesModuleEntity {
        int Id { get; }
    }

    public interface IWildcard {
        /// <summary>
        /// returns enumerable where: key - is wildcard from template, 
        /// value - name of column from database table
        /// </summary>
        IEnumerable< KeyValuePair<string, string> > ToKeyValues();
    }


    public partial class tblMessageSchedule: IMessagesModuleEntity { }

    //do not change order, do not use custom value, all names must be uniq
    public enum tblMessageScheduleRepeatMode {
        None,
        EveryDay,
        EveryWeek,
        EveryMonth,
        EveryYear
    }

    public static class tblMessageScheduleHelper {

        public static tblMessageScheduleRepeatMode RepeatModeFromString (string str) {
            str = str.ToLower();
            switch (str) {
                case null:
                case "":
                case "none":
                case "never":
                    return tblMessageScheduleRepeatMode.None;
                case "day":
                    return tblMessageScheduleRepeatMode.EveryDay;
                case "week":
                    return tblMessageScheduleRepeatMode.EveryWeek;
                case "month":
                    return tblMessageScheduleRepeatMode.EveryMonth;
                case "year":
                    return tblMessageScheduleRepeatMode.EveryYear;
            }
            throw new ArgumentException();
        }

        public static string RepeatModeToString(tblMessageScheduleRepeatMode mode) {
            switch (mode) {
                case tblMessageScheduleRepeatMode.None:
                    return "none";
                case tblMessageScheduleRepeatMode.EveryDay:
                    return "day";
                case tblMessageScheduleRepeatMode.EveryWeek:
                    return "week";
                case tblMessageScheduleRepeatMode.EveryMonth:
                    return "month";
                case tblMessageScheduleRepeatMode.EveryYear:
                    return "year";
            }
            throw new ArgumentException();
        }
        static List<string> RepeatModesStrings;
        static tblMessageScheduleHelper() {
            RepeatModesStrings = new List<string>();
            foreach (tblMessageScheduleRepeatMode mode in Enum.GetValues(typeof(tblMessageScheduleRepeatMode))) {
                RepeatModesStrings.Add(RepeatModeToString(mode));
            }
        }
        public static string[] GetAllowedRepeatModeNames() {
            return RepeatModesStrings.ToArray();
        }
    }

    public partial class tblMessageBatch : IMessagesModuleEntity { }
    public partial class tblMessage : IMessagesModuleEntity { }


    public partial class tblFilter: IMessagesModuleEntity {

    }
    public partial class tblRecepientFilter: IMessagesModuleEntity {
    }
    public partial class tblRecepientFilterTableName: IMessagesModuleEntity {
        //TODO should have schema field!
    }
    public partial class tblTemplate: IMessagesModuleEntity {
    }

    public partial class tblWildcard: IMessagesModuleEntity, IWildcard {

        public IEnumerable<KeyValuePair<string, string>> ToKeyValues() {
            return new[] { new KeyValuePair<string,string>(Code,Key) };
        }
    }

    public partial class tblRecepientCard : IMessagesModuleEntity, IWildcard {
        public const string NameCode = "{REC_NAME}";
        public const string PhoneCode = "{REC_PHONE}";
        public const string EmailCode = "{REC_EMAIL}";

        public IEnumerable<KeyValuePair<string, string>> ToKeyValues() {
            return new[] {
                new KeyValuePair<string, string>(NameCode, NameKey),
                new KeyValuePair<string, string>(PhoneCode, PhoneKey),
                new KeyValuePair<string, string>(EmailCode, EmailKey),
            };
        }
    }
}