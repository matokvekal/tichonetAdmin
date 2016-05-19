

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic;
using System.Data.Entity;

namespace Business_Logic
{
    public class tblFamilyLogic : baseLogic
    {

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
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                tblFamily v = new tblFamily();
                c.familyId = 9999999;
                c.date = DateTime.Today;
                c.LastUpdate = DateTime.Today; 
                db.tblFamilies.Add(c);
                db.SaveChanges();
                return c.familyId;
            }
            catch
            {
                throw;
            }

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

