using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Business_Logic.Helpers;
using Newtonsoft.Json;

namespace Business_Logic
{
    public class tblCalendarLogic : baseLogic
    {
        public IEnumerable<tblCalendar> Events
        {
            get { return DB.tblCalendars; }
        }

        public List<tblCalendar> GetPaged(bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters)
        {
            var searchModel = new { groupOp = "", rules = new[] { new { field = "", op = "", data = "" } } };
            var searchFilters = searchModel;

            IEnumerable<tblCalendar> query = DB.tblCalendars;

            if (isSearch && !string.IsNullOrWhiteSpace(filters))
            {
                searchFilters = JsonConvert.DeserializeAnonymousType(filters, searchModel);
                foreach (var rule in searchFilters.rules)
                {
                    var filterByProperty = typeof(tblCalendar).GetProperty(rule.field);
                    if (filterByProperty != null)
                    {
                        query = query.Where(x => filterByProperty.GetValue(x, null) != null);
                        if (filterByProperty.PropertyType == typeof(string))
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString().Contains(rule.data));
                        else if (filterByProperty.PropertyType == typeof(int))
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString().StartsWith(rule.data));
                        else if (filterByProperty.PropertyType == typeof(DateTime?))
                            query = query.Where(x => (DateTime?)filterByProperty.GetValue(x, null) == DateHelper.StringToDate(rule.data));
                        else
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString() == rule.data);
                    }
                }
            }

            var sortByProperty = typeof(tblCalendar).GetProperty(sortBy);
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
        
        public tblCalendar GetEvent(int id)
        {
            return DB.tblCalendars.FirstOrDefault(z => z.pk == id);
        }

        public List<tblCalendar> GetEvents(List<int> ids)
        {
            return DB.tblCalendars.Where(z => ids.Contains(z.pk)).ToList();
        }

        public tblCalendar SaveEvent(tblCalendar calendarEvent)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.tblCalendars.Add(calendarEvent);
                db.SaveChanges();
                return calendarEvent;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool Update(tblCalendar calendarEvent)
        {
            var res = false;
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(calendarEvent).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public bool DeleteEvent(int id)
        {
            var res = false;
            try
            {
                var calendarEvent = DB.tblCalendars.FirstOrDefault(z => z.pk == id);
                DB.tblCalendars.Remove(calendarEvent);
                DB.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public int[] GetDaysWithEvents(int month, int year)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                var days = db.tblCalendars
                    .Where(x => x.date.HasValue)
                    .Where(x => x.date.Value.Month == month && x.date.Value.Year == year)
                    .ToList()
                    .Select(x => x.date.Value.Date.Day)
                    .Distinct()
                    .ToArray();
                
                return days;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
