using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using Business_Logic;
using Business_Logic.Entities;
using Business_Logic.Helpers;
using log4net;
using ticonet.Models;

namespace ticonet
{
    public class StudentsController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(StudentsController));


        
         
        [ActionName("StudentsForMap")]
        public List<StudentShortInfo> GetStudentsForMap()
        {

            var res = new List<StudentShortInfo>();
            using (var context = new BusProjectEntities())
            {
                res = context.tblStudents.Select(data => new StudentShortInfo
                {
                    Id = data.pk,
                    StudentId = data.studentId,
                    Lat = data.Lat,
                    Lng = data.Lng,
                    Color = string.IsNullOrEmpty(data.Color.Trim()) ? "FF0000" : data.Color.Trim().Replace("#", ""),
                    Name = data.lastName + ", " + data.firstName,
                    CellPhone = data.CellPhone,
                    Email = data.Email,
                    Address = (data.city ?? "") + ", " + (data.street ?? "") + ", " + data.houseNumber,
                    Shicva = data.Shicva,
                    Class = data.@class,
                    Active = data.Active ?? false
                }).ToList();
            }
            return res;
        }

        [ActionName("SaveCoords")]
        public void GetSaveCoords(int id, string lat, string lng)
        {
            double pLat = 0;
            double pLng = 0;

            string strLat = StringHelper.FixDecimalSeparator(lat);
            string strLng = StringHelper.FixDecimalSeparator(lng);


            if (!double.TryParse(strLat, out pLat)) return;
            if (!double.TryParse(strLng, out pLng)) return;
            using (var context = new BusProjectEntities())
            {
                var st = context.tblStudents.FirstOrDefault(z => z.pk == id);
                if (st != null)
                {
                    st.Lat = pLat;
                    st.Lng = pLng;
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        [ActionName("Family")]
        public object GetFamily(int id)
        {
            tblFamily res = null;
            using (var context = new BusProjectEntities())
            {

                var st = context.tblStudents.FirstOrDefault(z => z.pk == id);
                if (st != null)
                {
                    res = context.tblFamilies.FirstOrDefault(z => z.familyId == st.familyId);
                }
            }

            return new StudentFamilyInfo { Id = id, Family = (res != null ? new FamilyModel(res) : new FamilyModel()) };
        }


    }

    public class StudentFamilyInfo
    {
        public int Id { get; set; }
        public FamilyModel Family { get; set; }
    }
}
