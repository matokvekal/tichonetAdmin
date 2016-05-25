using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class StudentShortInfo
    {
        public StudentShortInfo()
        {

        }
        public StudentShortInfo(tblStudent data)
        {
            Id = data.pk;
            StudentId = data.studentId;
            Lat = data.Lat;
            Lng = data.Lng;
            Color = data.Color;
            Name = data.lastName + ", " + data.firstName;
            CellPhone = data.CellPhone;
            Email = data.Email;
            Address = (data.city ?? "") + ", " + (data.street ?? "") + ", " + data.houseNumber;
            Shicva = data.Shicva;
            Class = data.@class;
            Active = data.Active ?? false;

        }
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Shicva { get; set; }
        public string Class { get; set; }
        public string Address { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; }
        public int LineId { get; set; }
        public string LineColor { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public string CellPhone { get; set; }
        public string Email { get; set; }


    }
}
