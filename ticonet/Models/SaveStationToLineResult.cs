using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business_Logic.Entities;

namespace ticonet.Models
{
    public class SaveStationToLineResult
    {
        public bool Done { get; set; }

        public LineModel Line { get; set; }

        public StationModel Station { get; set; }

        public List<StudentShortInfo> Students { get; set; } 
    }
}