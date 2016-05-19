using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic
{
    public class LoginLogic : baseLogic
    {
        public bool IsExist(string userName)
        {
            try
            {
                return DB.Logins.Any(l => l.userName == userName);
            }
            catch
            {
                throw;
            }

        }


        public bool IsExist(string userName, string password)
        {
            try
            {
                return DB.Logins.Any(l => l.userName == userName && l.Password == password);
            }
            catch
            {
                throw;
            }
        }

        public bool checkIfregisterd(string userName)
        {
            try
            {
                return DB.Logins.Any(l => l.userName == userName && l.emailConfirm==true);
            }
            catch
            {
                throw;
            }
        }
        public static bool confirmEmail(string userName)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                var c = (from s in db.Logins
                         where (s.userName == userName)
                         select s).FirstOrDefault();
                c.emailConfirm = true;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Register(string userName, string password)
        {
            try
            {
                Login login = new Login { userName = userName, Password = password };
                DB.Logins.Add(login);
                DB.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        public void Register(string userName, string password, string passwordUnSecure)
        {
            try
            {
                Login login = new Login { userName = userName, Password = password, passwordUnSecure = passwordUnSecure };
                DB.Logins.Add(login);
                DB.SaveChanges();
            }
            catch
            {
                throw;
            }
        }


        public  Login getLogin(string userName, string password)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.Logins.FirstOrDefault(c => c.userName == userName && c.Password == password);
            }
            catch
            {
                throw;
            }
        }

        public static Login getFamilyId(string userName, string password)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.Logins.FirstOrDefault(c => c.userName == userName && c.Password == password);
            }
            catch
            {
                throw;
            }
        }
        public static void updateFamilyId(string userName, int familyId)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                var c = (from s in db.Logins
                         where (s.userName == userName)
                         select s).FirstOrDefault();
                c.familyId = familyId;
                db.SaveChanges();
            }
            catch (Exception ex)
            {//todo write to log
                throw ex;
            }
        }

        public static void deleteByEmail(string Email)
        {
            try
            {

                      BusProjectEntities db = new BusProjectEntities();
                var v = (from s in db.Logins
                         where s.userName == Email
                         select s);

                if (v.Any() || v.Count() > 0)
                {
                    System.Data.SqlClient.SqlConnection cn;
                    cn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BusProject"].ConnectionString);
                    string sql = "delete Login WHERE[userName]='" + Email + "'  ";
                    cn.Open();
                    SqlCommand com = new SqlCommand(sql, cn);
                    com.ExecuteScalar();
                    cn.Close();
                }

                //BusProjectEntities db = new BusProjectEntities();
                //db.Entry<Login>(new Login { userName = Email }).State = EntityState.Deleted;
                //db.SaveChanges();
            }
            catch (Exception ex)
            {//todo write to log
                throw ex;
            }

        }
    }


}
