using System.Collections.Generic;
using Business_Logic;
using System;

namespace ticonet.Models {

    public class LinesPlanApiModel : IWeekDatedObject {
        public string ParentLineName { get; set; }
        public string ParentLineNumber { get; set; }

        public int Id { get; set; }
        public int LineId { get; set; }
        public bool? Sun { get; set; }
        public DateTime? SunTime { get; set; }
        public bool? Mon { get; set; }
        public DateTime? MonTime { get; set; }
        public bool? Tue { get; set; }
        public DateTime? TueTime { get; set; }
        public bool? Wed { get; set; }
        public DateTime? WedTime { get; set; }
        public bool? Thu { get; set; }
        public DateTime? ThuTime { get; set; }
        public bool? Fri { get; set; }
        public DateTime? FriTime { get; set; }
        public bool? Sut { get; set; }
        public DateTime? SutTime { get; set; }

        public void UpdateDBModelShallow (tblLinesPlan model) {
            model.SyncDatesTo(this);
        }
    }

}