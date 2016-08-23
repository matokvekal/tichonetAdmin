using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Business_Logic.Entities;
using Business_Logic.Dtos;
using Business_Logic.Helpers;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Business_Logic.Services;
using System.Data.SqlClient;

namespace Business_Logic
{
    public class LineLogic : baseLogic
    {
        public IEnumerable<Line> Lines
        {
            get { return DB.Lines; }
        }

        public List<Line> GetList()
        {
            return DB.Lines.ToList();
        }

        public List<Line> GetPaged(bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters)
        {
            IEnumerable<Line> query = GetFilteredAll(isSearch, filters);

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortOrder == "desc"
                    ? query.OrderByDescending(GetSortField(sortBy))
                    : query.OrderBy(GetSortField(sortBy));
            }

            query = query.Skip(rows * (page - 1))
                .Take(rows);

            return query.ToList();
        }

        public TotalDto GetTotal(bool isSearch, int rows, int page, string sortBy, string sortOrder, string filters)
        {
            TotalDto total = new TotalDto();
            IEnumerable<Line> query = GetFilteredAll(isSearch, filters);
            Line[] filteredAll = query.ToArray();
            foreach (var line in filteredAll)
            {
                if (line.IsActive)
                {
                    var busesToLines = line.BusesToLines.FirstOrDefault();
                    if (busesToLines != null)
                    {
                        var bus = busesToLines.Bus;
                        DateHelper.IterDays(7, weekDay =>
                        {
                            if (LineHelper.IsLineActiveAtDay(line, (DayOfWeek)(weekDay - 1)))
                            {
                                total.Seats += bus.seats.HasValue ? busesToLines.Bus.seats.Value : 0;
                                total.Students += line.totalStudents.HasValue ? line.totalStudents.Value : 0;
                                total.WeekDayPrices[weekDay - 1] += bus.price ?? 0;
                                total.Price += bus.price.HasValue ? busesToLines.Bus.price.Value : 0;
                            }
                        });
                    }
                }
            }
            return total;
        }

        public List<StationsToLine> GetStations(int lineId)
        {
            return DB.StationsToLines.Where(z => z.LineId == lineId).ToList();
        }

        public Line GetLine(int id)
        {
            return DB.Lines.FirstOrDefault(z => z.Id == id);
        }

        public List<Line> GetLines(IEnumerable<int> ids)
        {
            return DB.Lines.Where(z => ids.Contains(z.Id)).ToList();
        }

        public void UpdateStudentCount()
        {
            foreach (var line in DB.Lines)
            {
                line.totalStudents = DB.StudentsToLines.Count(z => z.LineId == line.Id);
            }
            DB.SaveChanges();
        }

        public Line SaveLine(int id, string number, string name, string color, int direction)
        {
            color = color.CssToNumeric();
            Line res = null;
            try
            {
                var itm = DB.Lines.FirstOrDefault(z => z.Id == id) ?? new Line
                {
                    IsActive = true
                };
                var c = itm.HexColor;
                var oldColor = itm.HexColor;
                itm.LineNumber = number;
                itm.LineName = name;
                var updateColors = itm.HexColor != color;
                var updateDirections = itm.Direction != direction;
                itm.HexColor = color;
                itm.Direction = direction;
                if (itm.Id == 0)
                {
                    DB.Lines.Add(itm);
                }
                if (updateDirections)
                {
                    foreach (var st in DB.StudentsToLines.Where(z => z.LineId == id))
                    {
                        st.Direction = itm.Direction;
                    }
                }
                if (updateColors)
                {
                    foreach (var st in DB.StudentsToLines.Where(z => z.LineId == id))
                    {
                        var stud = DB.tblStudents.FirstOrDefault(z => z.pk == st.StudentId);
                        if (stud != null && stud.Color == c)
                        {
                            stud.Color = itm.HexColor;
                        }
                    }
                }
                DB.SaveChanges();

                res = itm;
                if (!string.IsNullOrEmpty(oldColor)) UpdateStationsColor(itm, oldColor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public Line SaveLine(Line line)
        {
            line.CssToNumeric();
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Lines.Add(line);
                db.SaveChanges();
                return line;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool Update(Line line)
        {
            line.CssToNumeric();
            var res = false;
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(line).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public void UpdateStationsColor(Line line, string oldColor)
        {
            line.CssToNumeric();
            foreach (var stId in DB.StationsToLines.Where(z => z.LineId == line.Id).Select(z => z.StationId))
            {
                var station = DB.Stations.FirstOrDefault(z => z.Id == stId);
                if (station != null && station.color == oldColor) station.color = line.HexColor;
            }
            DB.SaveChanges();
            foreach (var stId in DB.StudentsToLines.Where(z => z.LineId == line.Id).Select(z => z.StudentId))
            {
                var student = DB.tblStudents.FirstOrDefault(z => z.pk == stId);
                if (student != null && student.Color == oldColor) student.Color = line.HexColor;
            }
            DB.SaveChanges();
        }

        public bool DeleteLine(int id)
        {
            var res = false;
            try
            {
                var students = DB.StudentsToLines.Where(z => z.LineId == id);
                DB.StudentsToLines.RemoveRange(students);
                var stations = DB.StationsToLines.Where(z => z.LineId == id);
                DB.StationsToLines.RemoveRange(stations);
                DB.SaveChanges();
                var line = DB.Lines.FirstOrDefault(z => z.Id == id);
                DB.Lines.Remove(line);
                DB.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public List<Line> GetLinesForStation(int stationId)
        {
            return DB.StationsToLines
                .Where(z => z.StationId == stationId)
                .Select(z => z.Line)
                .ToList();
        }

        public bool SwitchActive(int id, bool active)
        {
            var res = false;
            try
            {
                var itm = DB.Lines.FirstOrDefault(z => z.Id == id);
                if (itm != null)
                {
                    itm.IsActive = active;
                    DB.SaveChanges();
                    res = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public Line ReCalcTimeTable(SaveDurationsModel data)
        {
            Line res = null;
            try
            {
                var line = DB.Lines.FirstOrDefault(z => z.Id == data.LineId);
                if (line == null) return null;
                var stations = DB.StationsToLines.Where(z => z.LineId == data.LineId)
                    .OrderBy(z => z.Position).ToList();
                if (stations.Count == 0) return null;
                var loadTime = BusHelper.TimeForLoad;
                TimeSpan fst;
                if (data.FirstStation.Trim() == ":") //autoupdate
                {
                    if (line.Direction == 0)
                    {
                        fst = stations.Last().ArrivalDate;
                    }
                    else
                    {
                        fst = stations.First().ArrivalDate;
                    }

                }
                else
                {
                    var prts = data.FirstStation.Split(':');
                    if (prts.Length != 2) return null;
                    fst = new TimeSpan(int.Parse(prts[0]), int.Parse(prts[1]), 0); // time for first / last station 
                }


                var total = 0;
                if (line.Direction == 0) //to Station
                {
                    //Important! Last station will not included to data.Durations because sent duration from prev station
                    stations.Last().ArrivalDate = fst;
                    var pt = fst;
                    for (int i = data.Durations.GetLength(0) - 1; i >= 0; i--)
                    {
                        total += data.Durations[i].Duration + loadTime;
                        var d = new TimeSpan(0, 0, data.Durations[i].Duration + loadTime);
                        pt = pt.Subtract(d);
                        stations.Find(z => z.StationId == data.Durations[i].StationId).ArrivalDate = pt;
                    }
                }
                else //from Station
                {
                    var frst = data.Durations.First();
                    stations.First(z => z.StationId == frst.StationId).ArrivalDate = fst;
                    var pt = fst;
                    for (var i = 0; i < data.Durations.Length; i++)
                    {
                        total += data.Durations[i].Duration + loadTime;
                        var d = new TimeSpan(0, 0, data.Durations[i].Duration + loadTime);
                        pt = pt.Add(d);
                        stations[i + 1].ArrivalDate = pt;
                    }
                }
                line.Duration = new TimeSpan(0, 0, total);
                DB.SaveChanges();
                res = DB.Lines.FirstOrDefault(z => z.Id == data.LineId);
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

        public IEnumerable<Bus> GetAvailableBuses(int lineId)
        {
            return DB.Buses
                .Include(x => x.BusesToLines)
                .Where(x => !x.BusesToLines.Any() || x.BusesToLines.Any(b => b.LineId == lineId))
                .ToList();
        }

        public void UpdateBusToLine(int lineId, int busId)
        {
            var existingBusInLine = DB.BusesToLines.FirstOrDefault(x => x.LineId == lineId);
            if (existingBusInLine != null)
            {
                if (busId == 0)
                    DB.BusesToLines.Remove(existingBusInLine);
                else
                    existingBusInLine.BusId = busId;
            }
            else if (busId != 0)
            {
                DB.BusesToLines.Add(new BusesToLine
                {
                    LineId = lineId,
                    BusId = busId
                });
            }
            DB.SaveChanges();
        }

        public IEnumerable<tblBusCompany> GetCompaniesFilter()
        {
            return DB.Buses
                .Include(x => x.BusesToLines)
                .Include(x => x.BusCompany)
                .Where(x => x.BusesToLines.Any() && x.BusCompany != null)
                .Select(x => x.BusCompany)
                .Distinct()
                .ToList();
        }

        private IEnumerable<Line> GetFilteredAll(bool isSearch, string filters)
        {
            var searchModel = new { groupOp = "", rules = new[] { new { field = "", op = "", data = "" } } };
            var searchFilters = searchModel;

            IEnumerable<Line> query = DB.Lines;

            if (isSearch && !string.IsNullOrWhiteSpace(filters))
            {
                searchFilters = JsonConvert.DeserializeAnonymousType(filters, searchModel);
                foreach (var rule in searchFilters.rules)
                {
                    if (rule.field == "BusCompanyName")
                    {
                        int id;
                        int.TryParse(rule.data, out id);
                        query = query.AsQueryable()
                            .Include(x => x.BusesToLines)
                            .Where(x => x.BusesToLines.Select(l => l.Bus).Any(b => b.BusCompany != null && b.BusCompany.pk == id));
                    }
                    else if (rule.field == "DateRange")
                    {
                        DateTime dtmin, dtmax;
                        var dates = rule.data.Replace("\"", "").Split(new string[] { "<->" }, StringSplitOptions.RemoveEmptyEntries);
                        if (dates.Length == 2 && DateTime.TryParse(dates[0], out dtmin) && DateTime.TryParse(dates[1], out dtmax))
                        {
                            query = query.AsQueryable()
                            .Include(x => x.tblSchedules)
                            .Where(x => x.tblSchedules.Any(y => y.Date >= dtmin && y.Date < dtmax));
                        }
                        else
                            throw new ArgumentException();
                    }
                    else
                    {
                        var filterByProperty = typeof(Line).GetProperty(rule.field);
                        var filterByPropertyType = filterByProperty.PropertyType;
                        if (filterByProperty != null)
                        {
                            query = query.Where(x => filterByProperty.GetValue(x, null) != null);

                            if (filterByPropertyType == typeof(string))
                                query = query.Where(x => filterByProperty.GetValue(x, null).ToString().Contains(rule.data));
                            else if (filterByPropertyType == typeof(int))
                                query = query.Where(x => filterByProperty.GetValue(x, null).ToString().StartsWith(rule.data));
                            else if (filterByPropertyType == typeof(DateTime) && !string.IsNullOrWhiteSpace(rule.op))
                            {
                                DateTime compareDate;
                                DateTime.TryParse(rule.data, out compareDate);
                                //ge: greater or equal
                                if (rule.op == "ge")
                                    query = query.Where(x => (DateTime)filterByProperty.GetValue(x, null) >= compareDate);
                                //we dont need other operands now. so it goes like
                                //le: less or equal
                                else
                                    query = query.Where(x => (DateTime)filterByProperty.GetValue(x, null) <= compareDate);
                            }
                            else
                                query = query.Where(x => filterByProperty.GetValue(x, null).ToString() == rule.data);
                        }
                    }
                }
            }

            return query;
        }

        private Func<Line, object> GetSortField(string sortBy)
        {
            var sortByProperty = typeof(Line).GetProperty(sortBy);
            if (sortByProperty != null)
            {
                return line => sortByProperty.GetValue(line, null);
            }
            switch (sortBy)
            {
                case "Bus":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.Id;
                case "BusId":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.BusId;
                case "PlateNumber":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.PlateNumber;
                case "seats":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.seats;
                case "price":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.price;
                case "BusCompanyName":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.BusCompany.IfNotNull(c => c.companyName);
            }
            return line => line.Id;
        }

        public LinesTotalStatisticDto GetLinesTotalStatistic(DateTime periodStart_incl, DateTime periodEnd_excl)
        {
            var query = DB.tblSchedules
                .Where(x => x.Date >= periodStart_incl && x.Date < periodEnd_excl);
            var result = new LinesTotalStatisticDto
            {
                linesCount = query.Select(x => x.Line).Any() ? query.Select(x => x.Line).Count() : 0,
                totalStudents = query.Select(x => x.Line).Any() ? query.Select(x => x.Line).Sum(x => x.totalStudents ?? 0) : 0,
                totalPrice = query.Select(x => x.Bus).Any() ? query.Select(x => x.Bus).Sum(x => x.price ?? 0) : 0
            };
            return result;
        }

        public List<LinePeriodStatisticDto> GetAllLinesPeriodActivities(DateTime periodStart_incl, DateTime periodEnd_excl)
        {
            var output = new List<LinePeriodStatisticDto>();
            var lineIDs = DB.Lines.Select(x => x.Id);
            foreach (var id in lineIDs)
                output.Add(GetLinePeriodActivity(id, periodStart_incl, periodEnd_excl));
            return output;
        }

        /// <summary>
        /// important note: this method only works if dates range covers only one month!
        /// you should rewrite it if you want use it in any case.
        /// </summary>
        public LinePeriodStatisticDto GetLinePeriodActivity(int LineID, DateTime periodStart_incl, DateTime periodEnd_excl)
        {
            var lineQuery = DB.Lines
                .Where(x => x.Id == LineID);

            var line = lineQuery.First();

            var bus = DB.BusesToLines
                .Where(x => x.LineId == LineID)
                .Select(x => x.Bus)
                .FirstOrDefault();
            var LPA = new LinePeriodStatisticDto
            {
                Id = line.Id,
                LineName = line.LineName,
                LineNumber = line.LineNumber,
                Direction = line.Direction,
                totalStudents = line.totalStudents ?? 0,
                BusCompanyName = string.Empty,
                DayScheduleData = new List<string>(),
                DayDate = new List<DateTime>()
            };
            if (bus != null)
            {
                LPA.seats = bus.seats;
                LPA.price = bus.price;
                LPA.BusCompanyName = bus.BusCompany != null ? bus.BusCompany.companyName : string.Empty;
            }

            var schedulesQuery = lineQuery
                .SelectMany(x => x.tblSchedules)
                .Where(x => x.Date != null && x.Date >= periodStart_incl && x.Date < periodEnd_excl);

            //var activityDates = schedulesQuery
            //    .Select(x => x.Date.Value.Day).Distinct().ToDictionary(x => x);

            var dayScheduleRawData = schedulesQuery
                .GroupBy(x => x.Date.Value.Day)
                .Select(x => x.FirstOrDefault())
                .Select(x => new
                {
                    arrive = x.arriveTime,
                    leave = x.leaveTime,
                    dayNumber = x.Date.Value.Day
                }).ToDictionary(x => x.dayNumber);

            //its possible to optimize and do without dict
            int days = DateHelper.GetDatesPeriodInDays(periodStart_incl, periodEnd_excl);
            if (days <= 0)
                throw new ArgumentException("start date and end date periods must be in one day range at least, start should come first");

            DateHelper.IterDays(days, i =>
            {
                if (dayScheduleRawData.ContainsKey(i))
                {
                    var a = dayScheduleRawData[i];
                    LPA.DayScheduleData.Add(encodeDayScheduleDataString(a.leave, a.arrive));
                }
                else
                    LPA.DayScheduleData.Add(LinePeriodStatisticDto.DayScheduleData_INACTIVE);
                LPA.DayDate.Add(periodStart_incl.AddDays(i - 1));
            });

            return LPA;
        }

        private string encodeDayScheduleDataString(DateTime? leave, DateTime? arrive)
        {
            string output = string.Empty;
            if (leave.HasValue)
                output += leave.Value.ToShortTimeString();
            else
                output += " - ";
            output += "/";
            if (arrive.HasValue)
                output += arrive.Value.ToShortTimeString();
            else
                output += " - ";
            return output;
        }

        public List<LinesDatedTotalStatisticDto> GetLineTotalStatisticByDays(DateTime periodStart_incl, DateTime periodEnd_excl)
        {
            var rootQ = DB.Lines
                .SelectMany(x => x.tblSchedules)
                .Where(x => x.Date != null && x.Date >= periodStart_incl && x.Date < periodEnd_excl);
            var activityDates = rootQ
                .GroupBy(x => x.Date.Value)
                .ToDictionary(x => x.Key);
            int days = DateHelper.GetDatesPeriodInDays(periodStart_incl, periodEnd_excl);
            if (days <= 0)
                throw new ArgumentException("start date and end date periods must be in one day range at least, start should come first");
            var result = new List<LinesDatedTotalStatisticDto>();
            DateHelper.IterDays(periodStart_incl, days, (i, d) =>
            {
                IGrouping<DateTime, tblSchedule> group;
                LinesDatedTotalStatisticDto statItem = new LinesDatedTotalStatisticDto { Date = d };
                if (activityDates.TryGetValue(d, out group))
                {
                    var buses = group.Select(x => x.Bus);
                    bool isAnyBuses = buses.Any();
                    statItem.totalPrice = isAnyBuses ? buses.Sum(x => (x != null ? (x.price ?? 0) : 0)) : 0;
                    statItem.totalSeats = isAnyBuses ? buses.Sum(x => (x != null ? (x.seats ?? 0) : 0)) : 0;
                    statItem.linesCount = group.Select(x => x.Line).Distinct().Count();
                };
                result.Add(statItem);
            });
            return result;
        }

        public List<Line> GetLinesByPlan(IEnumerable<int> ids)
        {
            IEnumerable<int> linesByPlanIds = DB.tblLinesPlans.Where(z => ids.Contains(z.LineId)).Select(x => x.LineId).ToArray();
            var lines = DB.Lines.Where(z => linesByPlanIds.Contains(z.Id)).ToList();

            foreach (var line in lines)
            {
                line.IsActive = true;
                var plan = line.tblLinesPlan.FirstOrDefault();
                line.SyncDatesTo(plan);
            }

            return lines;
        }
        public static void updateAriveAndDepartureTime()//from the stations
        {
            try
            {

                System.Data.SqlClient.SqlConnection cn;
                cn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BusProject"].ConnectionString);

                string sql1 = "UPDATE Lines "
                       + "     SET BasicArriveTime = "
                       + "     (select max(StationsToLines.ArrivalDate) "
                       + "     FROM StationsToLines "
                       + "     WHERE StationsToLines.LineId = Lines.Id and Lines.Direction=0 ) ";//lines to school take the max time of station even not a school

                cn.Open();
                SqlCommand com1 = new SqlCommand(sql1, cn);
                com1.ExecuteScalar();
                string sql2 = "UPDATE Lines "
                        + "     SET BasicDepartureTime = "
                        + "     (select min(StationsToLines.ArrivalDate) "
                        + "     FROM StationsToLines "
                        + "     WHERE StationsToLines.LineId = Lines.Id and Lines.Direction=1 ) ";//lines from school take the min time of station even not a school
                
                SqlCommand com2 = new SqlCommand(sql2, cn);
                com2.ExecuteScalar();
                cn.Close();

            }
            catch (Exception ex)
            {
                //todo write to log
                throw ex;
            }

            // copy all admin data to the user type


        }

    }
}
