using System.Collections.Generic;

namespace ticonet.Models
{
    public class SaveStationResultModel
    {
        public StationModel Station { get; set; }

        public List<LineModel> Lines { get; set; }   
    }
}