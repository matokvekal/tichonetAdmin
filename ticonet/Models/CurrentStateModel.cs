using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Models
{
    public class CurrentStateModel
    {
        public double CenterLat { get; set; }
        public double CenterLng { get; set; }
        public int Zoom { get; set; }

        public bool ShowStationsWithoutLine { get; set; }

        public List<int> HiddenLines { get; set; }
        public List<int> HiddenStations { get; set; }
        public List<int> HiddenStudents { get; set; }

    }
}