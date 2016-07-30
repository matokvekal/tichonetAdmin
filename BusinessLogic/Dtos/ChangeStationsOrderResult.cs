using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Dtos {
    public class ChangeStationsOrderResult {
        public Dictionary<int, int> StationsToPositions { get; set; }
        public ChangeStationsOrderResult() {
            StationsToPositions = new Dictionary<int, int>();
        }
    }
}
