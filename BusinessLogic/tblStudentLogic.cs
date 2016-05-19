using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic;

namespace Business_Logic
{
    public class tblStudentLogic:baseLogic
    {

     
        
            public   tblStudent  getStudentByFamilyId(int familyId)
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
        public List<tblStudent> GetStudentByFamilyIdAndYear(int familyId,int Year)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblStudent> c = db.tblStudents.Where(x => x.familyId == familyId).Where(x=>x.yearRegistration==Year).ToList();
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
        public static bool checkIfIdExist(string id,int year)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblStudents.Any(x => x.studentId == id && x.yearRegistration==year));
        }
        

         public List<string> clas()
        {
            var clas = new List<string> { "ז", "ח", "ט", "י", "יא", "יב" };
            return clas.ToList();
        }
    }
}
