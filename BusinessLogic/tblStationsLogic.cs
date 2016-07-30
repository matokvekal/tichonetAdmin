using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.Enums;
using Business_Logic.Dtos;

namespace Business_Logic
{
    public class StationsLogic : baseLogic
    {
        public Station Save(Station station)
        {
            if (DB.Stations.Any(z => z.Id == station.Id))
            {
                // update station
                DB.Stations.Attach(station);
                DB.Entry(station).State = EntityState.Modified;

                ////Update Attached student colors
                //foreach (var rec in DB.StudentsToStations.Where(z => z.StationId == station.Id))
                //{
                //    var st = DB.tblStudents.FirstOrDefault(z => z.pk == rec.StudentId);
                //    if (st != null) st.Color = station.Color;
                //}
            }
            else
            {
                //add station
                DB.Stations.Add(station);
            }
            DB.SaveChanges();
            return station;
        }


        public Station GetStation(int id)
        {
            return DB.Stations.FirstOrDefault(z => z.Id == id);
        }

        public List<Station> GetStations(List<int> ids)
        {
            return DB.Stations.Where(z => ids.Contains(z.Id)).ToList();
        }


        public List<StudentsToLine> GetStudents(int stationsId)
        {
            return DB.StudentsToLines.Where(z => z.StationId == stationsId).ToList();
        }


        public List<Station> GetList()
        {
            return DB.Stations.ToList();
        }


        public List<Station> GetStationForLine(int lineId)
        {
            return DB.StationsToLines
                .Where(z => z.LineId == lineId)
                .Select(z => z.Station)
                .ToList();
        }

        public bool Delete(int Id)
        {
            var res = false;
            try
            {
                var itm = DB.Stations.FirstOrDefault(z => z.Id == Id);
                if (itm != null)
                {
                    DB.Stations.Remove(itm);
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

        public bool AddToLine(int stationId, int lineId, TimeSpan arrivalTime, int position, bool changeColor)
        {
            var res = false;
            try
            {
                //Remove old value if it exists
                var itm = DB.StationsToLines.FirstOrDefault(z => z.LineId == lineId && z.StationId == stationId);
                if (itm != null)
                {
                    DB.StationsToLines.Remove(itm);
                    DB.SaveChanges();
                }
                var stationsOnLine = DB.StationsToLines.Where(z => z.LineId == lineId).OrderBy(z => z.Position); //All station of line exclude  new
                foreach (var station in stationsOnLine)
                {
                    //If position of station equals ore more that new station position then move to one position
                    if (station.Position >= position) station.Position++;
                }
                itm = new StationsToLine
                {
                    LineId = lineId,
                    StationId = stationId,
                    ArrivalDate = arrivalTime,
                    Position = position
                };
                DB.StationsToLines.Add(itm);
                var line = DB.Lines.FirstOrDefault(z => z.Id == lineId);
                var c = "";
                var oldColor = "";
                if (changeColor)
                {
                    var station = DB.Stations.FirstOrDefault(z => z.Id == stationId);

                    if (station != null && line != null)
                    {
                        oldColor = station.color;
                        station.color = line.HexColor;
                        c = line.HexColor;
                    }
                }
                //Students attached to stations without line
                var students = DB.StudentsToLines.Where(z => z.StationId == stationId && z.LineId == -1);
                foreach (var student in students)
                {
                    if (student.LineId == -1)
                    {
                        student.LineId = lineId;
                        if (line != null) student.Direction = line.Direction;
                    }
                    if (!string.IsNullOrEmpty(c))
                    {
                        var st = DB.tblStudents.FirstOrDefault(z => z.pk == student.StudentId);
                        if (st != null)
                        {
                            //if (student.LineId==-1 || st.Color==oldColor) st.Color = c; //Change student color if student didn't have lines before or same color like station
                            st.Color = c;
                        }
                    }

                }


                DB.SaveChanges();
                using (var logic = new LineLogic())
                {
                    logic.UpdateStudentCount();
                }
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public ChangeStationsOrderResult ChangeStationPosition(int stationId, int lineId, int newPosition) {
            var itm = DB.StationsToLines.FirstOrDefault(z => z.LineId == lineId && z.StationId == stationId);
            if (itm == null)
                return null;
            var result = new ChangeStationsOrderResult();
            if (itm.Position != newPosition) { 
                var p = 1;
                foreach (var station in DB.StationsToLines.Where(z => z.LineId == lineId).OrderBy(z => z.Position)) {
                    if (station.StationId != stationId) {
                        if (p == newPosition) p++;
                        station.Position = p;
                        result.StationsToPositions.Add(station.StationId, p);
                        p++;
                    }
                }
                itm.Position = newPosition;
                result.StationsToPositions.Add(itm.StationId, newPosition);
                DB.SaveChanges();
            }
            
            return result;
        }

        public bool SaveOnLine(int stationId, int lineId, TimeSpan arrivalTime, int position, bool changeColor)
        {
            var res = false;
            try
            {
                var itm = DB.StationsToLines.FirstOrDefault(z => z.LineId == lineId && z.StationId == stationId);
                if (itm != null)
                {
                    if (itm.Position != position) //reorder
                    {
                        var p = 1;
                        foreach (var station in DB.StationsToLines.Where(z => z.LineId == lineId).OrderBy(z => z.Position))
                        {
                            if (station.StationId != stationId)
                            {
                                if (p == position) p++;
                                station.Position = p;
                                p++;
                            }
                        }
                        itm.Position = position;
                    }
                    itm.ArrivalDate = arrivalTime;
                    //var c = "";
                    if (changeColor)
                    {
                        var station = DB.Stations.FirstOrDefault(z => z.Id == stationId);
                        var line = DB.Lines.FirstOrDefault(z => z.Id == lineId);
                        if (station != null && line != null)
                        {
                            station.color = line.HexColor;

                        }

                    }


                    DB.SaveChanges();
                    using (var logic = new LineLogic())
                    {
                        logic.UpdateStudentCount();
                    }
                    res = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public bool DeleteFromLine(int stationId, int lineId)
        {
            var res = false;
            try
            {
                var itm = DB.StationsToLines.FirstOrDefault(z => z.LineId == lineId && z.StationId == stationId);
                if (itm != null)
                {
                    DB.StationsToLines.Remove(itm);
                    DB.SaveChanges();
                    var p = 1;
                    foreach (var station in DB.StationsToLines.Where(z => z.LineId == lineId).OrderBy(z => z.Position))
                    {
                        station.Position = p;
                        p++;
                    }
                    var students = DB.StudentsToLines.Where(z => z.StationId == stationId && z.LineId == lineId);
                    foreach (var student in students)
                    {
                        student.LineId = -1;
                    }
                    DB.SaveChanges();
                    using (var logic = new LineLogic())
                    {
                        logic.UpdateStudentCount();
                    }
                    res = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

        public bool AttachStudent(int studentId, int stationId, int? lineId, int distance, ColorMode colorMode, DateTime? date, ConflictActions action)
        {
            var res = false;
            try
            {
                using (var logic = new LineLogic())
                {
                    //Remove duplicate
                    var duplicates = DB.StudentsToLines
                        .Where(z => z.StudentId == studentId && z.Date == null)
                        .ToList();

                    DateTime? dt = null;
                    if (duplicates.Any())
                    {
                        if (action == ConflictActions.Replace || date == null)
                        {
                            DB.StudentsToLines.RemoveRange(duplicates);
                            DB.SaveChanges();
                        }
                        else
                        {
                            dt = date;
                        }
                    }




                    var student = DB.tblStudents.FirstOrDefault(z => z.pk == studentId);
                    if (student == null) return false;
                    var station = DB.Stations.FirstOrDefault(z => z.Id == stationId);
                    if (station == null) return false;
                    var line = DB.Lines.FirstOrDefault(z => z.Id == lineId);

                    var direction = 0;
                    if (line != null) direction = line.Direction;

                    if (colorMode == ColorMode.Station)
                    {
                        student.Color = station.color;
                    }

                    if (line != null && colorMode == ColorMode.Line)
                    {
                        student.Color = line.HexColor;
                    }

                    var item = new StudentsToLine
                    {
                        StudentId = studentId,
                        StationId = stationId,
                        LineId = lineId ?? -1,
                        color = student.Color,
                        Date = dt,
                        Direction = direction,
                        distanceFromStation = distance
                    };
                    DB.StudentsToLines.Add(item);
                    DB.SaveChanges();

                    logic.UpdateStudentCount();
                }
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }
        public bool UpdateDistance(int studentId, int stationId, int distance)
        {
            var res = false;
            try
            {
                foreach (var itm in DB.StudentsToLines.Where(z => z.StudentId == studentId && z.StationId == stationId))
                {
                    itm.distanceFromStation = distance;
                }
                DB.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;

        }

        public StudentsToLine GetAttachInfo(int id)
        {
            return DB.StudentsToLines.FirstOrDefault(z => z.Id == id);
        }

        public List<StudentsToLine> GetAttachInfo(int studentId, int stationId)
        {
            return DB.StudentsToLines.Where(z => z.StudentId == studentId && z.StationId == stationId).ToList();
        }


        public bool DeleteAttach(int id)
        {
            var res = false;
            try
            {
                var att = GetAttachInfo(id);
                if (att != null)
                {
                    DB.StudentsToLines.Remove(att);
                    DB.SaveChanges();
                    using (var logic = new LineLogic())
                    {
                        logic.UpdateStudentCount();
                    }
                    res = true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }

    }
}

