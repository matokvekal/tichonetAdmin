using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Mvc;
using Business_Logic;
using Business_Logic.Entities;
using MvcJqGrid;

namespace ticonet.Controllers
{
    public class StudentsController : ApiController
    {
        [System.Web.Http.ActionName("StudentsForMap")]
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
                    Color = data.Color,
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

        [System.Web.Http.ActionName("SaveCoords")]
        public void GetSaveCoords(int id, string lat, string lng)
        {
            double pLat = 0;
            double pLng = 0;

            string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string strLat = lat.Replace(".", sep).Replace(",", sep);
            string strLng = lng.Replace(".", sep).Replace(",", sep);


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

        [System.Web.Http.ActionName("Family")]
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

            return new StudentFamilyInfo { Id = id, Family = res };
        }


    }

    public class StudentFamilyInfo
    {
        public int Id { get; set; }
        public tblFamily Family { get; set; }
    }
}
