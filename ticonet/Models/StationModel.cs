using System;
using System.Collections.Generic;
using Business_Logic;

namespace ticonet.Models
{
    public class StationModel
    {
        public StationModel() { }

        public StationModel(Station data)
        {
            Color = data.color ?? "FF0000";
            Id = data.Id;
            Name = data.StationName;
            StrLat = data.Lattitude;
            StrLng = data.Longitude;
            Address = data.Address;
            Type = data.StationType;
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public string Color { get; set; }
        public string StrLat { get; set; }

        public string StrLng { get; set; }

        public string Address { get; set; }

        public int Type { get; set; }

        public List<StudentToLineModel> Students { get; set; }
    }
}