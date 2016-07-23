using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class StudentFullInfo
    {
        public int Id { get; set; }

        public string StudentId { get; set; }

        public int IntStudentId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Class { get; set; }

        public string Shicva { get; set; }

        public string Address { get; set; }

        public bool? PayStatus { get; set; }

        public bool? Active { get; set; }
        public bool? SibilingAtSchool { get; set; }

        public bool SpecialRequest { get; set; }

        public int DistanceToSchool { get; set; }

        public string LineName { get; set; }

        public int LineId { get; set; }
        public int StationId { get; set; }
        public int FamilyId { get; set; }
    }
}
