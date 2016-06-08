using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic
{
    public class LineLogic : baseLogic
    {
        public System.Collections.Generic.List<Line> GetList()
        {
            return DB.Lines.Where(z => z.IsActive).ToList();
        }

        public System.Collections.Generic.List<StationsToLine> GetStations(int lineId)
        {
            return DB.StationsToLines.Where(z => z.LineId == lineId).ToList();
        }

        public Line GetLine(int id)
        {
            return DB.Lines.FirstOrDefault(z => z.Id == id);
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
                var oldColor = itm.HexColor;
                itm.LineNumber = number;
                itm.LineName = name;
                itm.HexColor = color;
                itm.Direction = direction;
                if (itm.Id == 0)
                {
                    DB.Lines.Add(itm);
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
    }
}
