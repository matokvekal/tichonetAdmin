namespace Business_Logic.Entities
{
    public class SaveDurationsModel
    {
        public int LineId { get; set; }
        public string FirstStation { get; set; }
        public SegmentDuration[] Durations { get; set; }
    }
}