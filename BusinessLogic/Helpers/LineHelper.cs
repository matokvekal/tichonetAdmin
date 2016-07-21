using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Helpers
{
    public static class LineHelper
    {
        public static void RefreshActive(Line line)
        {
            bool isActive = line.IsActive;
            if ((!(line.Sun.HasValue && line.Sun.Value)) &&
                (!(line.Mon.HasValue && line.Mon.Value)) &&
                (!(line.Tue.HasValue && line.Tue.Value)) &&
                (!(line.Wed.HasValue && line.Wed.Value)) &&
                (!(line.Thu.HasValue && line.Thu.Value)) &&
                (!(line.Fri.HasValue && line.Fri.Value)) &&
                (!(line.Sut.HasValue && line.Sut.Value)))
                isActive = false;
            line.IsActive = isActive;
        }

    }
}
