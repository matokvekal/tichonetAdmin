using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic {

    public class tblLinesPlanLogic : baseLogic {

        public int tblLinesCount { get { return DB.tblLinesPlans.Count(); } }
        
        //TODO avoid moving data to memory
        public List<tblLinesPlan> GetPaged(bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters) {
            IEnumerable<tblLinesPlan> query = GetFilteredAll(isSearch, filters);

            if (!string.IsNullOrEmpty(sortBy)) {
                query = sortOrder == "desc"
                    ? query.OrderByDescending(SortFieldSelector(sortBy))
                    : query.OrderBy(SortFieldSelector(sortBy));
            }
            query = query.Skip(rows * (page - 1))
                .Take(rows);

            return query.ToList();
        }

        //TODO incaps
        private Func<tblLinesPlan, object> SortFieldSelector(string sortBy) {
            var sortByProperty = typeof(tblLinesPlan).GetProperty(sortBy);
            if (sortByProperty != null) {
                return line => sortByProperty.GetValue(line, null);
            }
            switch (sortBy) {
                case "ParentLineName":
                    return l => l.Line.LineName;
                case "ParentLineNumber":
                    return l => l.Line.LineNumber;
            }
            return line => line.Id;
        }

        private IEnumerable<tblLinesPlan> GetFilteredAll(bool isSearch, string filters) {
            if (isSearch)
                throw new NotImplementedException();
            return DB.tblLinesPlans;
        }

        public tblLinesPlan GetFirstByLine (int lineID) {
            return DB.tblLinesPlans.FirstOrDefault(x => x.LineId == lineID);
        }

        public tblLinesPlan Get(int tblLinesPlanID) {
            return DB.tblLinesPlans.FirstOrDefault(x => x.Id == tblLinesPlanID);
        }

        //TODO split to SaveOrCreate | SaveOnly
        public tblLinesPlan Save (tblLinesPlan item) {
            var entry = DB.tblLinesPlans.FirstOrDefault(x => x.Id == item.Id);
            if (entry == null)
                DB.tblLinesPlans.Add(item);
            else
                DB.Entry(item).State = System.Data.Entity.EntityState.Modified;
            DB.SaveChanges();
            return item;
        }

        public void Remove (tblLinesPlan item) {
            DB.tblLinesPlans.Remove(item);
            DB.SaveChanges();
        }

        /// <summary>
        /// Syncs each line activity: activity = true, if there is some plan, and false if not
        /// also sets line's (week) dates to plan's dates
        /// </summary>
        public void SyncLinesToPlans () {
            foreach (var line in DB.Lines) {
                if (!line.tblLinesPlan.Any())
                    line.IsActive = false;
                else {
                    var plan = GetFirstByLine(line.Id);
                    line.SyncDatesTo(plan);
                }
            }
            DB.SaveChanges();
        }


    }
}
