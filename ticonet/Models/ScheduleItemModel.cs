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
            Date = DateHelper.DateToString(data.Date);
            Direction = (LineDirection?)data.Direction;
            DriverId = data.DriverId;
            BusId = data.BusId;
            leaveTime = DateHelper.TimeToString(data.leaveTime);
            arriveTime = DateHelper.TimeToString(data.arriveTime);
            LineId = data.LineId;
            LineIdDescription = GetLineIdDescription(data.Line);
            DriverIdDescription = data.Driver != null ? data.Driver.FirstName + " " + data.Driver.LastName : string.Empty;
            BusIdDescription = GetBusIdDescription(data.Bus);
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

        public int? LineIdKey { get; set; }

        public int? DriverIdKey { get; set; }

        public int? BusIdKey { get; set; }

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
                Date = DateHelper.StringToDate(Date),
                leaveTime = DateHelper.StringToTime(leaveTime),
                arriveTime = DateHelper.StringToTime(arriveTime)
            };
        }

        public static string GetLineIdDescription(Line line)
        {
            return (line != null ? string.Format("{0} - {1}", line.LineName, line.LineNumber) : string.Empty);
        }

        public static string GetBusIdDescription(Bus bus)
        {
            if (bus == null)
            {
                return string.Empty;
            }

            return string.Format("{0} ({1} - {2})", bus.Id, bus.BusId, bus.PlateNumber);
        }
    }
}