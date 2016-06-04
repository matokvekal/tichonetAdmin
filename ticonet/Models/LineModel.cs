using System.Collections.Generic;
using Business_Logic;

namespace ticonet.Models
{
    public class LineModel
    {

        public LineModel()
        {
            Stations = new List<StationToLineModel>();
        }

        public LineModel(Line data)
        {
            Id = data.Id;
            Name = data.LineName;
            LineNumber = data.LineNumber;
            Color = data.HexColor;
            Direction = data.Direction;
            Active = data.IsActive;
            StudentsCount = data.totalStudents ?? 0;
            Geometry = data.PathGeometry;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LineNumber { get; set; }

        public string Color { get; set; }

        public int Direction { get; set; }

        public bool Active { get; set; }

        public int StudentsCount { get; set; }

        public string Geometry { get; set; }

        public List<StationToLineModel> Stations { get; set; }
    }
}