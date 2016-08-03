using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic
{
    public class tblSchoolLogic : baseLogic
    {
        public List<tblSchool> GetList()
        {
            return DB.tblSchools.ToList();
        }

        public tblSchool GetById(int id)
        {
            return DB.tblSchools.FirstOrDefault(z => z.id == id);
        }
    }
}
