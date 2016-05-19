using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic;

namespace Business_Logic
{
    public class tblYearsLogic:baseLogic
    {
        public static List<tblYear> GetYears()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblYear> c = db.tblYears.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
  
    }
}
