using System;
using Business_Logic;

namespace ticonet.Models
{
    public class StudentToLineModel
    {
        public StudentToLineModel() { }

        public StudentToLineModel(StudentsToLine data)
        {
            StudentId = data.StudentId;
            if (data.LineId == -1)
            {
                LineId = null;
            }
            else
            {
                LineId = data.LineId;
            }
            StationId = data.StationId;
            Direction = data.Direction;
            Date = data.Date;
            Distance = data.distanceFromStation;
            Geometry = data.PathGeometry;
        }


        public int StudentId { get; set; }

        public int? LineId { get; set; }

        public int StationId { get; set; }

        public int Direction { get; set; }

        public DateTime? Date { get; set; }

        public int? Distance { get; set; }

        public string Geometry { get; set; }

    }
}