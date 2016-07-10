using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Business_Logic.Entities;
using Business_Logic.Helpers;
using Newtonsoft.Json;

namespace Business_Logic
{
    public class BusToLineLogic : baseLogic
    {
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

        public void RemoveByBus(int busId)
        {
            var busToLine = DB.BusesToLines.Where(z => z.BusId == busId);
            DB.BusesToLines.RemoveRange(busToLine);
            DB.SaveChanges();
        }
    }
}
