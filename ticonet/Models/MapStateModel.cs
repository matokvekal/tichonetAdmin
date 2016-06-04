using System.Collections.Generic;
using Business_Logic.Entities;

namespace ticonet.Models
{
    public class MapStateModel
    {
        public List<LineModel> Lines { get; set; }
        public List<StationModel> Stations { get; set; }
        public List<StudentShortInfo> Students { get; set; }
    }
}