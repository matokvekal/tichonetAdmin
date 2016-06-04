using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<StudentsToLine> GetStudents(int stationsId)
        {
            return DB.StudentsToLines.Where(z => z.StationId == stationsId).ToList();
        } 

        public List<int> GetAttachedStudentsIds(int stationId)
        {
            return new List<int>();
            //return DB.tblStudentsToStations.Where(z => z.StationId == stationId).Select(z => z.StudentId).ToList();
        } 

        public List<Station> GetList()
        {
            return DB.Stations.ToList();
        }

        public List<Station> GetStationWithoutLine()
        {
            return DB.StudentsToLines
                .Where(z => z.LineId == null)
                .Select(z=>z.Station)
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

        public bool AttachStudent(int studentId, int stationId, int distance)
        {
            var res = false;
            try
            {
                ////Remove all previous attachments of this student
                //var lstOld = DB.tblStudentsToStations.Where(z => z.StudentId == studentId);
                //DB.tblStudentsToStations.RemoveRange(lstOld);

                //var itm = new tblStudentsToStation
                //{
                //    StationId = stationId,
                //    StudentId = studentId
                //};
                //DB.tblStudentsToStations.Add(itm);

                //var station = DB.tblStations.FirstOrDefault(z => z.Id == stationId);
                //var student = DB.tblStudents.FirstOrDefault(z => z.pk == studentId);
                //if (student != null && station != null)
                //{
                //    student.Color = station.Color;
                //}

                //DB.SaveChanges();
                //res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;
        }
    }
}

