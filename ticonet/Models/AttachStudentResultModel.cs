using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business_Logic;
using Business_Logic.Entities;

namespace ticonet.Models
{
    public class AttachStudentResultModel
    {
        public List<LineModel> Lines { get; set; }

        public List<StationModel> Stations { get; set; }

        public StudentShortInfo Student { get; set; }

        public bool Done { get; set; }

    }
}