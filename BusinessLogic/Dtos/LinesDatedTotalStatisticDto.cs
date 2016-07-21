using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Dtos {
    public class LinesDatedTotalStatisticDto {
        public DateTime Date { get; set; }
        public int linesCount { get; set; }
        public int totalSeats { get; set; }
        public double totalPrice { get; set; }
    }
}
