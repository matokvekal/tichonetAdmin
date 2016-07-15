using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;

namespace Business_Logic
{
    public class tblBusLogic : baseLogic
    {
        public IEnumerable<Bus> Buses
        {
            get { return DB.Buses; }
        }

        public List<Bus> GetPaged(bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters)
        {
            var searchModel = new { groupOp = "", rules = new[] { new { field = "", op = "", data = "" } } };
            var searchFilters = searchModel;

            IEnumerable<Bus> query = DB.Buses;

            if (isSearch && !string.IsNullOrWhiteSpace(filters))
            {
                searchFilters = JsonConvert.DeserializeAnonymousType(filters, searchModel);
                foreach (var rule in searchFilters.rules)
                {
                    var filterByProperty = typeof(Bus).GetProperty(rule.field);
                    if(filterByProperty.PropertyType == typeof(string))
                        query = query.Where(x => filterByProperty.GetValue(x, null).ToString().Contains(rule.data));
                    else if (filterByProperty.PropertyType == typeof(int))
                        query = query.Where(x => filterByProperty.GetValue(x, null).ToString().StartsWith(rule.data));
                    else 
                        query = query.Where(x => filterByProperty.GetValue(x, null).ToString() == rule.data);
                }
            }

            var sortByProperty = typeof(Bus).GetProperty(sortBy);
            if (sortByProperty != null)
            {
                query = sortOrder == "desc"
                    ? query.OrderByDescending(x => sortByProperty.GetValue(x, null))
                    : query.OrderBy(x => sortByProperty.GetValue(x, null));
            }



            query = query.Skip(rows*(page - 1))
                .Take(rows);

            return query.ToList();
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

        public bool Update(Bus bus)
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
                using (var busToLineLogic = new BusToLineLogic())
                {
                    busToLineLogic.RemoveByBus(id);
                }
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
