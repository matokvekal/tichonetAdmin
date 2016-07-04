using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Business_Logic
{
    public class tblBusLogic : baseLogic
    {
        public List<Bus> GetList()
        {
            return DB.Buses.ToList();
        }
        
        public Bus GetBus(int id)
        {
            return DB.Buses.FirstOrDefault(z => z.Id == id);
        }

        public List<Bus> GetBuses(List<int> ids)
        {
            return DB.Buses.Where(z => ids.Contains(z.Id)).ToList();
        }

        public Bus SaveBus(Bus bus)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Buses.Add(bus);
                db.SaveChanges();
                return bus;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public static bool Update(Bus bus)
        {
            var res = false;
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(bus).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public bool DeleteBus(int id)
        {
            var res = false;
            try
            {
                var busToLine = DB.BusesToLines.Where(z => z.BusId == id);
                DB.BusesToLines.RemoveRange(busToLine);
                DB.SaveChanges();
                var bus = DB.Buses.FirstOrDefault(z => z.Id == id);
                DB.Buses.Remove(bus);
                DB.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }
    }
}
