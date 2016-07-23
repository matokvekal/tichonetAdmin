using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.CompilerServices;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic
{
    public class tblStudentLogic : baseLogic
    {



        public tblStudent getStudentByFamilyId(int familyId)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblStudents.FirstOrDefault(c => c.familyId == familyId);
            }
            catch
            {
                return null;
            }
        }
        public List<tblStudent> GetStudentByFamilyIdAndYear(int familyId)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblStudent> c = db.tblStudents.Where(x => x.familyId == familyId).ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
        public List<tblStudent> GetStudentByFamilyIdAndYear(int familyId, int Year)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblStudent> c = db.tblStudents.Where(x => x.familyId == familyId).Where(x => x.yearRegistration == Year).ToList();
                return c;
            }
            catch
            {
                return null;
            }
        }

        public List<SimpleItem> CitiesAutocomplete(string term)
        {
            return DB.tblStreets.Where(z => z.cityName.ToLower().Contains(term.ToLower()))
                .Select(z => new SimpleItem { id = z.cityId, value = z.cityName })
                .Distinct()
                .ToList();
        }

        public List<SimpleItem> StreetsAutocomplete(string term, int cityId)
        {
            return DB.tblStreets.Where(z => z.streetName.ToLower().Contains(term.ToLower()) && z.cityId == cityId)
                .Select(z => new SimpleItem { id = z.streetId, value = z.streetName })
                .Distinct()
                .ToList();
        }


        public int DefaultCityId
        {
            get
            {
                var strValue = SettingsHelper.GetSettingValue("Students", "defaultCityId");
                int id;
                return int.TryParse(strValue, out id) ? id : 0;
            }
            set
            {
                SettingsHelper.SetSettingValue("Students", "defaultCityId", value.ToString());
            }
        }

        public int DefaultSchoolId
        {
            get
            {
                var strValue = SettingsHelper.GetSettingValue("Students", "defaultSchoolId");
                int id;
                return int.TryParse(strValue, out id) ? id : 0;
            }
            set
            {
                SettingsHelper.SetSettingValue("Students", "defaultSchoolId", value.ToString());
            }
        }

        public void Delete(int pk)
        {
            var itm = DB.tblStudents.FirstOrDefault(z => z.pk == pk);
            if (itm != null)
            {
                DB.tblStudents.Remove(itm);
                DB.SaveChanges();
            }
        }

        public static void create(tblStudent c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();

                var ids = db.tblStudents.Select(z => z.studentId).ToList();
                var pid = ids.Max(z => Int32.Parse(z)) + 1;
                c.studentId = pid.ToString();
                c.dateCreate = DateTime.Today;
                c.lastUpdate = DateTime.Today;
                var crYear = tblSystemLogic.getSystemValueByKey("currentRegistrationYear");
                c.yearRegistration = crYear == null ? DateTime.Now.Year : int.Parse(crYear.strValue);
                c.registrationStatus = false;

                db.tblStudents.Add(c);

                db.SaveChanges();

            }
            //sqlException
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        //Console.WriteLine(message);
                    }
                }
            }
            //catch (DbUpdateException ex)
            //{
            //    var sqlex = ex.InnerException.InnerException as SqlException;

            //    if (sqlex != null)
            //    {
            //        switch (sqlex.Number)
            //        {
            //            case 547: throw new ExNoExisteUsuario("No existe usuario destino."); //FK exception
            //            case 2627:
            //            case 2601:
            //                throw new ExYaExisteConexion("Ya existe la conexion."); //primary key exception

            //            default: throw sqlex; //otra excepcion que no controlo.


            //        }
            //    }

            //    throw ex;
            //}

        }

        public List<tblStudent> GetActiveStudents()
        {
            return DB.tblStudents.Where(z => z.Active ?? false).ToList();
        }

        public tblStudent getStudentByPk(int pk)
        {
            try
            {
                var db = new BusProjectEntities();
                return db.tblStudents.FirstOrDefault(c => c.pk == pk);
            }
            catch
            {
                return null;
            }
        }

        public static void update(tblStudent c)
        {

            try
            {

                var db = new BusProjectEntities();
                db.Entry<tblStudent>(c).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
            }
        }
        public static bool checkIfIdExist(string id, int year)
        {
            var db = new BusProjectEntities();

            return (db.tblStudents.Any(x => x.studentId == id && x.yearRegistration == year));
        }


        public List<string> clas()
        {
            var clas = new List<string> { "ז", "ח", "ט", "י", "יא", "יב" };
            return clas.ToList();
        }

        public List<string> Classes()
        {
            return DB.tblStudents.Select(z => z.@class).Distinct().ToList();
        }

        public List<string> Shicvas()
        {
            return DB.tblStudents.Select(z => z.Shicva).Distinct().ToList();
        }


        public List<StudentFullInfo> GetStudentsForTable(StudentSearchRequest request, out int total)
        {
            var res = new List<StudentFullInfo>();
            using (var context = new BusProjectEntities())
            {
                context.Database.Log = Console.Write;
                var cs = request.Class.Length;
                var ss = request.Shicva.Length;
                var ls = request.LineIds.Length;
                var sts = request.StationIds.Length;
                var lst = (from stud in context.tblStudents
                           join
                               stLine in context.StudentsToLines
                               on stud.pk equals stLine.StudentId into ps
                           from stLine in ps.DefaultIfEmpty()
                           join stat in context.Stations on stLine.StationId equals stat.Id into ps1
                           from stat in ps1.DefaultIfEmpty()
                           join line in context.Lines on stLine.LineId equals line.Id into ps2
                           from line in ps2.DefaultIfEmpty()
                           where stLine.Date == null &&
                           ((string.IsNullOrEmpty(request.StudentId)) || (stud.studentId.ToLower().Contains(request.StudentId.ToLower()))) &&
                            ((ss == 0) || (request.Shicva.Contains(stud.Shicva))) &&
                            ((cs == 0) || (request.Class.Contains(stud.@class))) &&
                            (ls == 0 || request.LineIds.Contains(stLine.LineId)) &&
                            (sts == 0 || request.StationIds.Contains(stLine.StationId)) &&
                            (string.IsNullOrEmpty(request.Name) || (stud.lastName + ", " + stud.firstName).ToLower().Contains(request.Name.ToLower())) &&
                            (string.IsNullOrEmpty(request.Address) || (stud.city + " " + stud.street + " " + stud.houseNumber).ToLower().Contains(request.Address.ToLower())) &&
                            (string.IsNullOrEmpty(request.City) || stud.city.ToLower().Contains(request.City)) &&
                            (string.IsNullOrEmpty(request.Street) || stud.street.ToLower().Contains(request.Street)) &&
                            (string.IsNullOrEmpty(request.House) || stud.houseNumber.ToString().Contains(request.House)) &&
                            (request.Active == 0 || (request.Active == 1 && stud.Active == true) || (request.Active == 2 && stud.Active == false)) &&
                            (request.PayStatus == 0 || (request.PayStatus == 1 && stud.paymentStatus == true) || (request.PayStatus == 2 && stud.paymentStatus == false)) &&
                            (request.Subcidy == 0 || (request.Subcidy == 1 && stud.subsidy == true) || (request.Subcidy == 2 && stud.subsidy == false)) &&
                            (request.SibilingAtSchool == 0 || (request.SibilingAtSchool == 1 && stud.siblingAtSchool == true) || (request.SibilingAtSchool == 2 && stud.siblingAtSchool == false)) &&
                            (request.SpecialRequest == 0 || (request.SpecialRequest == 1 && stud.specialRequest == true) || (request.SpecialRequest == 2 && stud.specialRequest == false)) &&
                            (request.DistanceFromSchoolFrom == 0 || (stud.distanceFromSchool != null && stud.distanceFromSchool.Value >= request.DistanceFromSchoolFrom)) &&
                            (request.DistanceFromSchoolTo == 0 || (stud.distanceFromSchool != null && stud.distanceFromSchool.Value <= request.DistanceFromSchoolTo)) &&
                            (request.DistanceFromStationFrom == 0 || (stLine.distanceFromStation != null && stLine.distanceFromStation.Value >= request.DistanceFromStationFrom)) &&
                            (request.DistanceFromStationTo == 0 || (stLine.distanceFromStation != null && stLine.distanceFromStation.Value <= request.DistanceFromStationTo)) &&
                            (request.Direction == 0 || (request.Direction == 1 && stLine.Direction == 1) || (request.Direction == 2 && stLine.Direction == 0))
                           select new StudentFullInfo()
                           {
                               FirstName = stud.firstName,
                               LastName = stud.lastName,
                               Address = stud.city + " " + stud.street + " " + stud.houseNumber,
                               Active = stud.Active,
                               Class = stud.@class,
                               Id = stud.pk,
                               StudentId = stud.studentId,
                               PayStatus = stud.paymentStatus,
                               Shicva = stud.Shicva,
                               SibilingAtSchool = stud.siblingAtSchool,
                               SpecialRequest = stud.specialRequest ?? false,
                               DistanceToSchool = stud.distanceFromSchool.HasValue ? (int)stud.distanceFromSchool : 0,
                               LineName = stat != null ? stat.StationName + (line != null ? " (line " + line.LineNumber + ")" : "") : "--"
                           }).Distinct();


                total = lst.Count();

                if (string.IsNullOrEmpty(request.SortColumn)) request.SortColumn = "studentid";
                if (string.IsNullOrEmpty(request.SortOrder)) request.SortOrder = "asc";

                var skeep = (request.PageNumber - 1) * request.PageSize;
                var take = request.PageSize;

                switch (request.SortColumn.ToLower())
                {
                    case "studentid":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.StudentId).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.StudentId).Skip(skeep).Take(take).ToList();
                        break;
                    case "name":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.FirstName).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.FirstName).Skip(skeep).Take(take).ToList();
                        break;
                    case "firstname":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.FirstName).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.FirstName).Skip(skeep).Take(take).ToList();
                        break;
                    case "lastname":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.LastName).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.LastName).Skip(skeep).Take(take).ToList();
                        break;
                    case "addr":
                    case "address":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.Address).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.Address).Skip(skeep).Take(take).ToList();
                        break;
                    case "shicva":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.Shicva).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.Shicva).Skip(skeep).Take(take).ToList();
                        break;
                    case "class":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.Class).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.Class).Skip(skeep).Take(take).ToList();
                        break;
                    case "payment":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.PayStatus).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.PayStatus).Skip(skeep).Take(take).ToList();
                        break;
                    case "active":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.Active).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.Active).Skip(skeep).Take(take).ToList();
                        break;
                    case "sibilingatschool":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.SibilingAtSchool).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.SibilingAtSchool).Skip(skeep).Take(take).ToList();
                        break;
                    case "specialrequest":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.SpecialRequest).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.SpecialRequest).Skip(skeep).Take(take).ToList();
                        break;
                    case "distance":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.DistanceToSchool).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.DistanceToSchool).Skip(skeep).Take(take).ToList();
                        break;
                    case "line":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.LineName).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.LineName).Skip(skeep).Take(take).ToList();
                        break;
                }
            }
            return res;
        }

        public List<StudentShortInfo> GetStudentsForFamily(int familyId, int? studentId, int page, int pageSize, string sortColumn,
            string sortOrder, out int total)
        {
            var res = new List<StudentShortInfo>();
            var lst = (from stud in DB.tblStudents.ToList()
                       where stud.familyId == familyId && stud.pk != studentId
                       select new StudentShortInfo(stud)).ToList();
            total = lst.Count;
            var skeep = (page - 1) * pageSize;
            var take = pageSize;
            switch (sortColumn.ToLower())
            {
                case "studentid":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.StudentId).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.StudentId).Skip(skeep).Take(take).ToList();
                    break;
                case "name":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Name).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Name).Skip(skeep).Take(take).ToList();
                    break;
                case "shicva":
                    res = sortOrder == "asc"
                       ? lst.OrderBy(z => z.Shicva).Skip(skeep).Take(take).ToList()
                       : lst.OrderByDescending(z => z.Shicva).Skip(skeep).Take(take).ToList();
                    break;
                case "class":
                    res = sortOrder == "asc"
                       ? lst.OrderBy(z => z.Class).Skip(skeep).Take(take).ToList()
                       : lst.OrderByDescending(z => z.Class).Skip(skeep).Take(take).ToList();
                    break;
                case "address":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Address).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Address).Skip(skeep).Take(take).ToList();
                    break;
            }
            return res;
        }

        public List<StudentLineInfo> GetLinesForStudent(int studentId, int page, int pageSize, string sortColumn,
            string sortOrder, out int total)
        {
            List<StudentLineInfo> res = new List<StudentLineInfo>();
            var lst = (from stud in DB.StudentsToLines
                       join line in DB.Lines on stud.LineId equals line.Id
                       where stud.StudentId == studentId
                       select new StudentLineInfo
                       {
                           Id = stud.Id,
                           Color = line.HexColor,
                           Name = line.LineName,
                           Number = line.LineNumber,
                           Date = stud.Date,
                           Direction = line.Direction
                       }).ToList();
            total = lst.Count;
            var skeep = (page - 1) * pageSize;
            var take = pageSize;
            switch (sortColumn.ToLower())
            {
                case "color":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Color).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Color).Skip(skeep).Take(take).ToList();
                    break;
                case "number":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Number).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Number).Skip(skeep).Take(take).ToList();
                    break;
                case "name":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Name).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Name).Skip(skeep).Take(take).ToList();
                    break;
                case "direction":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Direction).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Direction).Skip(skeep).Take(take).ToList();
                    break;
                case "date":
                    res = sortOrder == "asc"
                        ? lst.OrderBy(z => z.Date).Skip(skeep).Take(take).ToList()
                        : lst.OrderByDescending(z => z.Date).Skip(skeep).Take(take).ToList();
                    break;
            }
            return res;
        }

        public List<string> GetStudentsColorsList()
        {
            List<string> res;
            using (var context = new BusProjectEntities())
            {
                res = context.tblStudents.Select(z => z.Color).Distinct().ToList();
            }
            return res;
        }

        public List<tblStudent> GetStudentsForLine(int lineId)
        {
            var ids = DB.StudentsToLines
                .Where(z => z.LineId == lineId)
                .Select(z => z.StudentId).ToArray();
            return DB.tblStudents.Where(z => ids.Contains(z.pk)).ToList();
        }
        public List<tblStudent> GetStudentsForStation(int stationId)
        {
            var ids = DB.StudentsToLines
                .Where(z => z.StationId == stationId)
                .Select(z => z.StudentId).ToArray();
            return DB.tblStudents.Where(z => ids.Contains(z.pk)).ToList();
        }

        public List<StudentsToLine> GetAttachInfo(int studentId)
        {
            return DB.StudentsToLines.Where(z => z.StudentId == studentId).ToList();
        }
    }
}
