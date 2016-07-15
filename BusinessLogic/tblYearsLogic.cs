using System.Collections.Generic;
using System.Linq;

namespace Business_Logic
{
    public class tblYearsLogic: baseLogic
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
