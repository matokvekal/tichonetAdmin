using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic
{
    public class LineLogic : baseLogic
    {
        public List<Line> GetList()
        {
            return DB.Lines.ToList();
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
                var updateColors = itm.HexColor != MapHelper.FixColor(color);
                var updateDirections = itm.Direction != direction;
                itm.HexColor = MapHelper.FixColor(color);
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

        public void UpdateStationsColor(Line line, string oldColor)
        {
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
    }
}
