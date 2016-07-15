using Business_Logic;
using Business_Logic.Helpers;

namespace ticonet.Models
{
    public class ScheduleItemModel
    {
        public ScheduleItemModel(tblSchedule data)
        {
            Id = data.Id;
            Date = DateHelper.DateToString(data.Date);
            Direction = data.Direction;
            LineId = data.LineId;
            DriverId = data.DriverId;
            BusId = data.BusId;
            leaveTime = DateHelper.DateToString(data.leaveTime);
            arriveTime = DateHelper.DateToString(data.arriveTime);
        }

        public ScheduleItemModel() { }

        public int Id { get; set; }

        public string Date { get; set; }

        public int? Direction { get; set; }

        public int? LineId { get; set; }

        public int? DriverId { get; set; }

        public int? BusId { get; set; }

        public string leaveTime { get; set; }

        public string arriveTime { get; set; }

        public string Oper { get; set; }

        public tblSchedule ToDbModel()
        {
            return new tblSchedule
            {

                Id = Id,
                Direction = Direction,
                LineId = LineId,
                DriverId = DriverId,
                BusId = BusId,
                Date = DateHelper.StringToDate(Date),
                leaveTime = DateHelper.StringToDate(leaveTime),
                arriveTime = DateHelper.StringToDate(arriveTime)
            };
        }
    }
}