using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business_Logic;
using Business_Logic.Entities;

namespace ticonet.Models
{
    public class EditLineResultModel
    {
        public bool Done { get; set; }

        public LineModel Line { get; set; }

        public List<StationModel> Stations { get; set; }

        public List<StudentShortInfo> Students { get; set; } 

    }
}