using System;

namespace Business_Logic {

    public interface IWeekDatedObject {
        bool? Sun { get; set; }
        DateTime? SunTime { get; set; }
        bool? Mon { get; set; }
        DateTime? MonTime { get; set; }
        bool? Tue { get; set; }
        DateTime? TueTime { get; set; }
        bool? Wed { get; set; }
        DateTime? WedTime { get; set; }
        bool? Thu { get; set; }
        DateTime? ThuTime { get; set; }
        bool? Fri { get; set; }
        DateTime? FriTime { get; set; }
        bool? Sut { get; set; }
        DateTime? SutTime { get; set; }
    }

    public static class IWeekDatedObjectExtensions {

        public static void SyncDatesTo (this IWeekDatedObject that, IWeekDatedObject source) {
            that.Sun = source.Sun;
            that.SunTime = source.SunTime;
            that.Mon = source.Mon;
            that.MonTime = source.MonTime;
            that.Tue = source.Tue;
            that.TueTime = source.TueTime;
            that.Wed = source.Wed;
            that.WedTime = source.WedTime;
            that.Thu = source.Thu;
            that.ThuTime = source.ThuTime;
            that.Fri = source.Fri;
            that.FriTime = source.FriTime;
            that.Sut = source.Sut;
            that.SutTime = source.SutTime;
        }

    }


}
