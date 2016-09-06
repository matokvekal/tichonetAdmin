using Business_Logic.Entities;

namespace ticonet.Models
{
    public class SaveGeometryModel
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public SegmentDuration[] Durations { get; set; }
    }

    //public class SegmentDuration
    //{
    //    public int Duration { get; set; }
    //    public int StartStationId { get; set; }
    //    public int EndStationId { get; set; }
    //    public int Position { get; set; }
    //}
}