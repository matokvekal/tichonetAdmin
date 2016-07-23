using System.Collections.Generic;
using System.Linq;

namespace Business_Logic
{
    public class tblStreetsLogic:baseLogic
    {
        public string GetCityById(int id)
        {
            var city = DB.tblStreets.FirstOrDefault(z => z.cityId == id);
            return city != null ? city.cityName : "";
        }
        public bool IsExist(string street)
        {
            try
            {
                return DB.tblStreets.Any(l => l.streetName == street);
            }
            catch
            {
                throw;
            }
        }
     
        
        public List<tblStreet> GetStreetsByprefix(string street)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblStreet> c = db.tblStreets.Where(x => x.streetName.Contains(street)).ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
        public List<tblStreet> GetStreets()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblStreet> c = db.tblStreets.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
    }
}
