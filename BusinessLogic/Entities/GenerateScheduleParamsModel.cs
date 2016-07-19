namespace Business_Logic.Entities
{
    public class ScheduleParamsModel
    {
        public string LinesIds { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public bool ArriveTime { get; set; }

        public bool LeaveTime { get; set; }

        public bool Sun { get; set; }

        public bool Mon { get; set; }

        public bool Tue { get; set; }

        public bool Wed { get; set; }

        public bool Thu { get; set; }

        public bool Fri { get; set; }

        public bool Sut { get; set; }
    }
}