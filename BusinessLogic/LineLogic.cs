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
            TotalDto total = new TotalDto()
            {
                Students = 0,
                Seats = 0,
                Price = 0
            };

            IEnumerable<Line> query = GetFilteredAll(isSearch, filters);

            Line[] filteredAll = query.ToArray();
            foreach (var line in filteredAll)
            {
                total.Students += line.totalStudents.HasValue ? line.totalStudents.Value : 0;
                var busesToLines = line.BusesToLines.FirstOrDefault();
                if (busesToLines != null)
                {
                    total.Seats += busesToLines.Bus.seats.HasValue ? busesToLines.Bus.seats.Value : 0;
                    total.Price += busesToLines.Bus.price.HasValue ? busesToLines.Bus.price.Value : 0;
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

        public List<Line> GetLines(List<int> ids)
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
                    else
                    {
                        var filterByProperty = typeof(Line).GetProperty(rule.field);
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
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine {Bus = new Bus()}).First().Bus.Id;
                case "BusId":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine {Bus = new Bus()}).First().Bus.BusId;
                case "PlateNumber":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine {Bus = new Bus()}).First().Bus.PlateNumber;
                case "seats":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine {Bus = new Bus()}).First().Bus.seats;
                case "price":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine { Bus = new Bus() }).First().Bus.price;
                case "BusCompanyName":
                    return line => line.BusesToLines.DefaultIfEmpty(new BusesToLine {Bus = new Bus() }).First().Bus.BusCompany.IfNotNull(c => c.companyName);
            }
            return line => line.Id;
        }
    }
}
