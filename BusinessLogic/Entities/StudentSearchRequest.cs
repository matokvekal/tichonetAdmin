using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class StudentSearchRequest
    {
        public string StudentId { get; set; }

        public string Name
        {
            get;
            set;

        }

        public string[] Shicva { get; set; }
        public string[] Class { get; set; }
        public string Address { get; set; }


        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Color { get; set; }
        public int Line { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int Active { get; set; }

        public int[] LineIds { get; set; }
        public int[] StationIds { get; set; }
        public int registrationStatus { get; set; }
        public int PayStatus { get; set; }


        public int Subcidy { get; set; }

        public int SibilingAtSchool { get; set; }

        public int SpecialRequest { get; set; }

        public int DistanceFromSchoolFrom { get; set; }
        public int DistanceFromSchoolTo { get; set; }

        public int DistanceFromStationFrom { get; set; }
        public int DistanceFromStationTo { get; set; }

        public int Direction { get; set; }
    }
}
