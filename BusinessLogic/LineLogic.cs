using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic
{
    public class LineLogic : baseLogic
    {
        public List<Line> GetList()
        {
            return DB.Lines.Where(z => z.IsActive).ToList();
        }

        public List<StationsToLine> GetStations(int lineId)
        {
            return DB.StationsToLines.Where(z => z.LineId == lineId).ToList();
        }
    }
}
