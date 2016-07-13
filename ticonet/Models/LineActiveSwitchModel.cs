using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Models
{
    public class LineActiveSwitchModel
    {
        public int LineId { get; set; }
        public bool Active { get; set; }

        public bool Done { get; set; }
    }

    
}