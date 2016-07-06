using System.Collections.Generic;
using Business_Logic;

namespace ticonet.Models
{
    public class LineBusModel
    {

        public LineBusModel()
        {
        }

        public LineBusModel(Bus data)
        {
            Id = data.Id;
            BusId = data.BusId;
        }

        public int Id { get; set; }

        public string BusId { get; set; }
    }
}