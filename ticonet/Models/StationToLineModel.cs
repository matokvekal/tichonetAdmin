using System;
using Business_Logic;

namespace ticonet.Models
{
    public class StationToLineModel
    {

        public StationToLineModel() { }

        public StationToLineModel(StationsToLine data)
        {
            Id = data.Id;
            LineId = data.LineId;
            StationId = data.StationId;
            Position = data.Position;
            ArrivalDate = data.ArrivalDate;
        }


        public int Id { get; set; }
        public int LineId { get; set; }

        public int StationId { get; set; }

        public int Position { get; set; }

        public TimeSpan ArrivalDate { get; set; }

        public string ArrivalDateString
        {
            get { return ArrivalDate.ToString("g"); }
        }
    }
}