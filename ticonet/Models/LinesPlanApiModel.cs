using System.Collections.Generic;
using Business_Logic;
using System;
using Business_Logic.Helpers;

namespace ticonet.Models {

    public class LinesPlanApiModel
    {
        public LinesPlanApiModel()
        {
        }

        public LinesPlanApiModel(tblLinesPlan data)
        {
            Id = data.Id;
            LineId = data.LineId;
            Sun = data.Sun.HasValue ? data.Sun.Value : false;
            SunTime = DateHelper.TimeToString(data.SunTime);
            Mon = data.Mon.HasValue ? data.Mon.Value : false;
            MonTime = DateHelper.TimeToString(data.MonTime);
            Tue = data.Tue.HasValue ? data.Tue.Value : false;
            TueTime = DateHelper.TimeToString(data.TueTime);
            Wed = data.Wed.HasValue ? data.Wed.Value : false;
            WedTime = DateHelper.TimeToString(data.WedTime);
            Thu = data.Thu.HasValue ? data.Thu.Value : false;
            ThuTime = DateHelper.TimeToString(data.ThuTime);
            Fri = data.Fri.HasValue ? data.Fri.Value : false;
            FriTime = DateHelper.TimeToString(data.FriTime);
            Sut = data.Sut.HasValue ? data.Sut.Value : false;
            SutTime = DateHelper.TimeToString(data.SutTime);
        }

        public string ParentLineName { get; set; }
        public string ParentLineNumber { get; set; }

        public int Id { get; set; }
        public int LineId { get; set; }

        public bool Sun { get; set; }

        public string SunTime { get; set; }

        public bool Mon { get; set; }

        public string MonTime { get; set; }

        public bool Tue { get; set; }

        public string TueTime { get; set; }

        public bool Wed { get; set; }

        public string WedTime { get; set; }

        public bool Thu { get; set; }

        public string ThuTime { get; set; }

        public bool Fri { get; set; }

        public string FriTime { get; set; }

        public bool Sut { get; set; }

        public string SutTime { get; set; }

        public void UpdateDBModelShallow (tblLinesPlan model) {
            model.Sun = Sun;
            model.SunTime = DateHelper.StringToTime(SunTime);
            model.Mon = Mon;
            model.MonTime = DateHelper.StringToTime(MonTime);
            model.Tue = Tue;
            model.TueTime = DateHelper.StringToTime(TueTime);
            model.Wed = Wed;
            model.WedTime = DateHelper.StringToTime(WedTime);
            model.Thu = Thu;
            model.ThuTime = DateHelper.StringToTime(ThuTime);
            model.Fri = Fri;
            model.FriTime = DateHelper.StringToTime(FriTime);
            model.Sut = Sut;
            model.SutTime = DateHelper.StringToTime(SutTime);
        }
    }

}