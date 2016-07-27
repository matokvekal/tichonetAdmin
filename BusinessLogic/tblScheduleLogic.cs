using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Business_Logic.Helpers;
using Business_Logic.Services;

namespace Business_Logic
{
    public class tblScheduleLogic : baseLogic
    {
        public IEnumerable<tblSchedule> Schedule
        {
            get { return DB.tblSchedules; }
        }

        public List<tblSchedule> GetList()
        {
            return DB.tblSchedules.ToList();
        }

        public List<tblSchedule> GetPaged(IEnumerable<int> linesIds, DateTime? dateFrom, DateTime? dateTo, bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters)
        {
            var searchModel = new { groupOp = "", rules = new[] { new { field = "", op = "", data = "" } } };
            var searchFilters = searchModel;

            IEnumerable<tblSchedule> query = DB.tblSchedules;
            
            var idsArr = linesIds != null ? linesIds.ToList() : new List<int>();
            if (!idsArr.Any())
            {
                return new List<tblSchedule>();
            }
            query = query.Where(x => idsArr.Any(l => l == x.LineId));

            if (dateFrom.HasValue)
            {
                query = query.Where(x => x.Date.HasValue && x.Date.Value >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(x => x.Date.HasValue && x.Date.Value <= dateTo.Value);
            }

            if (isSearch && !string.IsNullOrWhiteSpace(filters))
            {
                searchFilters = JsonConvert.DeserializeAnonymousType(filters, searchModel);
                foreach (var rule in searchFilters.rules)
                {
                    var filterByProperty = typeof(tblSchedule).GetProperty(rule.field);
                    if (filterByProperty != null)
                    {
                        query = query.Where(x => filterByProperty.GetValue(x, null) != null);
                        if (filterByProperty.PropertyType == typeof(string))
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString().Contains(rule.data));
                        else if (filterByProperty.PropertyType == typeof(int))
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString().StartsWith(rule.data));
                        else
                            query = query.Where(x => filterByProperty.GetValue(x, null).ToString() == rule.data);
                    }
                }
            }

            var sortByProperty = typeof(tblSchedule).GetProperty(sortBy);
            if (sortByProperty != null)
            {
                query = sortOrder == "desc"
                    ? query.OrderByDescending(x => sortByProperty.GetValue(x, null))
                    : query.OrderBy(x => sortByProperty.GetValue(x, null));
            }

            query = query.Skip(rows * (page - 1))
                .Take(rows);

            return query.ToList();
        }

        public tblSchedule GetItem(int id)
        {
            return DB.tblSchedules.FirstOrDefault(z => z.Id == id);
        }

        public List<tblSchedule> GetItems(List<int> ids)
        {
            return DB.tblSchedules.Where(z => ids.Contains(z.Id)).ToList();
        }

        public tblSchedule SaveItem(tblSchedule scheduleItem)
        {
            try
            {
                UpdateLineBus(scheduleItem.LineId, scheduleItem.BusId);
                //BusProjectEntities db = new BusProjectEntities();
                DB.tblSchedules.Add(scheduleItem);
                DB.SaveChanges();
                return scheduleItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool Update(tblSchedule scheduleItem)
        {
            var res = false;
            try
            {
                UpdateLineBus(scheduleItem.LineId, scheduleItem.BusId);
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(scheduleItem).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public bool DeleteItem(int id)
        {
            var res = false;
            try
            {
                var item = DB.tblSchedules.FirstOrDefault(z => z.Id == id);
                DB.tblSchedules.Remove(item);
                DB.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public bool DeleteItems(IEnumerable<tblSchedule> items)
        {
            var res = false;
            try
            {
                DB.tblSchedules.RemoveRange(items);
                DB.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }
        
        public void SaveChanges()
        {
            DB.SaveChanges();
        }

        private void UpdateLineBus(int? lineId, int? busId)
        {
            //using (var logic = new BusToLineLogic())
            //{
            var logic = new BusToLineLogic(DB);
            logic.UpdateBusToLine(lineId ?? 0, busId ?? 0);
            //}
        }

        public void AutoCorrectLineSchedules(int LineID, DateTime periodStart_incl, DateTime periodEnd_excl) {
            var line = DB.Lines.First(x => x.Id == LineID);
            IQueryable<tblSchedule> schedules = DB
                .tblSchedules
                .Where(x => x.LineId == LineID);

            DateTime dateSt = periodStart_incl, dateEnd = periodStart_incl;
            Expression<Func<tblSchedule, bool>> scheduleDayPredicate = x =>
                x.Date >= dateSt && x.Date < dateEnd;

            int days = DateHelper.GetDatesPeriodInDays(periodStart_incl, periodEnd_excl);
            if (days <= 0)
                throw new ArgumentException("start date and end date periods must be in one day range at least, start should come first");

            ScheduleService schedServ = new ScheduleService();
            DateHelper.IterDays(periodStart_incl, days, (i, date) => {
                dateSt = date;
                dateEnd = dateSt.AddDays(1);
                if (LineHelper.IsLineActiveAtDay(line, (DayOfWeek)i - 1)) {
                    if (!schedules.Any(scheduleDayPredicate)) {
                        var newSch = schedServ.GenerateSingleSchedule(line, date, true, true);
                        SaveItem(newSch);
                    }
                }
                else if (schedules.Any(scheduleDayPredicate))
                    DB.tblSchedules.RemoveRange(schedules.Where(scheduleDayPredicate));
            });
            DB.SaveChanges();
        }
    }
}
