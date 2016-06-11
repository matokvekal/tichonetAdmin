namespace ticonet.Models
{
    public class AttachStudentModel
    {
        public int StudentId { get; set; }

        public int StationId { get; set; }

        public int Distance { get; set; }

        public int? LineId { get; set; }

        public int UseColor { get; set; }
    }
}