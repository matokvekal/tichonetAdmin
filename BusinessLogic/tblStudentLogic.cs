using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Business_Logic.Entities;

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
        public static void create(tblStudent c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                c.dateCreate = DateTime.Today;
                c.lastUpdate = DateTime.Today;
                c.pk = 999999;
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
                BusProjectEntities db = new BusProjectEntities();
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

                BusProjectEntities db = new BusProjectEntities();
                db.Entry<tblStudent>(c).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
            }
        }
        public static bool checkIfIdExist(string id, int year)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblStudents.Any(x => x.studentId == id && x.yearRegistration == year));
        }


        public List<string> clas()
        {
            var clas = new List<string> { "ז", "ח", "ט", "י", "יא", "יב" };
            return clas.ToList();
        }

        public List<StudentShortInfo> GetStudentsForTable(StudentSearchRequest request, out int total)
        {
            var res = new List<StudentShortInfo>();
            using (var context = new BusProjectEntities())
            {
                var lst =
                    context.tblStudents.Where(
                        z =>
                            (string.IsNullOrEmpty(request.Name) ||
                             (z.lastName + ", " + z.firstName).ToLower().Contains(request.Name.ToLower()))
                             && (string.IsNullOrEmpty(request.Address)
                            || (z.city + " " + z.street + " " + z.houseNumber.ToString()).ToLower().Contains(request.Address.ToLower()))
                            && (string.IsNullOrEmpty(request.Shicva)
                            || (z.Shicva.ToLower().Contains(request.Shicva.ToLower())))
                            && (string.IsNullOrEmpty(request.Class))
                            || (z.@class.ToLower().Contains(request.Class.ToLower()))
                            && (string.IsNullOrEmpty(request.Color))
                            || (z.Color.ToLower().Contains(request.Color.ToLower())))
                        .Select(z => new StudentShortInfo
                        {
                            Name = z.lastName + ", " + z.firstName,
                            Address = z.city + " " + z.street + z.houseNumber.ToString(),
                            Active = z.Active ?? false,
                            Class = z.@class,
                            Color = z.Color,
                            Id = z.pk,
                            StudentId = z.studentId,
                            Shicva = z.Shicva
                        })
                        .ToList();
                total = lst.Count();

                if (string.IsNullOrEmpty(request.SortColumn)) request.SortColumn = "name";
                if (string.IsNullOrEmpty(request.SortOrder)) request.SortOrder = "asc";

                var skeep = (request.PageNumber - 1) * request.PageSize;
                var take = request.PageSize;

                switch (request.SortColumn)
                {
                    case "id":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.StudentId).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.StudentId).Skip(skeep).Take(take).ToList();
                        break;
                    case "name":
                        res = request.SortOrder == "asc"
                            ? lst.OrderBy(z => z.Name).Skip(skeep).Take(take).ToList()
                            : lst.OrderByDescending(z => z.Name).Skip(skeep).Take(take).ToList();
                        break;
                    case "addr":
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
                    case "color":
                        res = request.SortOrder == "asc"
                           ? lst.OrderBy(z => z.Color).Skip(skeep).Take(take).ToList()
                           : lst.OrderByDescending(z => z.Color).Skip(skeep).Take(take).ToList();
                        break;
                }
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
