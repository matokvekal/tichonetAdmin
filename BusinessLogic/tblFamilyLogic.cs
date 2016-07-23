

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Business_Logic
{
    public class tblFamilyLogic : baseLogic
    {
        public List<tblFamily> GetAll()
        {
            List<tblFamily> res;
            try
            {
                res = DB.tblFamilies.ToList();
            }
            catch (Exception)
            {
                res = new List<tblFamily>();
                //throw;
            }
            return res;
        } 

        public tblFamily GetFamilyById(int familyId)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblFamilies.FirstOrDefault(c => c.familyId == familyId);
            }
            catch
            {
                return null;
            }

        }

        public tblFamily GetFamilyByStudentId(string pk)
        {
            try
            {
                var id = Int32.Parse(pk);
                var familyId = DB.tblStudents.First(z => z.pk == id).familyId;
                return DB.tblFamilies.FirstOrDefault(c => c.familyId == familyId);
            }
            catch
            {
                return null;
            }

        }

        public static bool checkEmailExist(string email)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblFamilies.Any(x => x.parent1Email == email));
        }




        public static bool checkIfFamilyExist(int familyId)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblFamilies.Any(x => x.familyId == familyId));
        }



        public static bool checkIfIdExist(string id)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblFamilies.Any(x => x.ParentId == id));
        }

        public static bool checkIfEmailExist(string email)
        {
            BusProjectEntities db = new BusProjectEntities();

            return (db.tblFamilies.Any(x => x.parent1Email == email));
        }
        public static int createFamily(tblFamily c)
        {
            var res = -1;
            try
            {
                using (var db = new BusProjectEntities())
                {
                    var pid = 0; 
                    int.TryParse(db.tblFamilies.Max(z => z.ParentId), out pid);
                    c.ParentId = (pid +1).ToString();
                    c.date = DateTime.Today;
                    c.LastUpdate = DateTime.Today;
                    c.registrationStatus = false;
                    db.tblFamilies.Add(c);
                    db.SaveChanges();
                    res = c.familyId;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var e = ex.EntityValidationErrors.FirstOrDefault();
                throw;
            }
            return res;
        }
        public static void update(tblFamily c)
        {

            try
            {

                BusProjectEntities db = new BusProjectEntities();
                db.Entry<tblFamily>(c).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
            }
        }
    }
}

