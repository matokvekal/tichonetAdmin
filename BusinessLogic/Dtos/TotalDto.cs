
using System.Collections.Generic;

namespace Business_Logic.Dtos
{
    public class TotalDto {

        public TotalDto() {
            WeekDayPrices = new double[7];
        }

        public int Students { get; set; }

        public int Seats { get; set; }

        public double Price { get; set; }

        public double[] WeekDayPrices {get;set;}
    }
}
