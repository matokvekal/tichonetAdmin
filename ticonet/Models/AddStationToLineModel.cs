using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Models
{
    public class AddStationToLineModel
    {
        public int StationId { get; set; }

        public int LineId { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Position { get; set; }

        public bool ChangeColor { get; set; }
        public string  StrChangeColor { get; set; }

        public bool AlwaysFirst { get; set; }
        public bool AlwaysLast { get; set; }

        public string  StrAlwaysFirst { get; set; }
        public string  StrAlwaysLast { get; set; }

    }
}