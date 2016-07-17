using System.Linq;
using Business_Logic;
using Business_Logic.Enums;
using Business_Logic.Helpers;

namespace ticonet.Models
{
    public class ScheduleItemModel
    {
        public ScheduleItemModel(tblSchedule data)
        {
            Id = data.Id;
            Date = DateTimeHelper.DateToString(data.Date);
            Direction = (LineDirection?)data.Direction;
            DriverId = data.DriverId;
            BusId = data.BusId;
            leaveTime = DateTimeHelper.TimeToString(data.leaveTime);
            arriveTime = DateTimeHelper.TimeToString(data.arriveTime);
            LineId = data.LineId;
            LineIdDescription = data.Line != null ? data.Line.LineName : string.Empty;
            DriverIdDescription = data.Driver != null ? data.Driver.FirstName + " " + data.Driver.LastName : string.Empty;
            BusIdDescription = GetBusIdDescription(data);
        }

        public ScheduleItemModel() { }

        public int Id { get; set; }

        public string Date { get; set; }

        public LineDirection? Direction { get; set; }

        public int? LineId { get; set; }

        public int? DriverId { get; set; }

        public int? BusId { get; set; }

        public string leaveTime { get; set; }

        public string arriveTime { get; set; }

        public string LineIdDescription { get; set; }

        public string DriverIdDescription { get; set; }

        public string BusIdDescription { get; set; }

        public string Oper { get; set; }

        public tblSchedule ToDbModel()
        {
            return new tblSchedule
            {

                Id = Id,
                Direction = (int?)Direction,
                LineId = LineId,
                DriverId = DriverId,
                BusId = BusId,
                Date = DateTimeHelper.StringToDate(Date),
                leaveTime = DateTimeHelper.StringToTime(leaveTime),
                arriveTime = DateTimeHelper.StringToTime(arriveTime)
            };
        }

        private string GetBusIdDescription(tblSchedule data)
        {
            var bus = data.Line != null && data.Line.BusesToLines.Any()
                ? data.Line.BusesToLines.First().Bus
                : null;

            if (bus == null)
            {
                return string.Empty;
            }

            return string.Format("{0} ({1} - {2})", bus.Id, bus.BusId, bus.PlateNumber);
        }
    }
}