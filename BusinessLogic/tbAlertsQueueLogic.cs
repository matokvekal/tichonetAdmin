using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic;

namespace Business_Logic
{
    public class tblAlertsQueueLogic : baseLogic
    {

        public static void create(tblAlertsQueue c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                c.id = 9999;

                db.SaveChanges();

            }
            catch
            {

            }

        }
    }
}
