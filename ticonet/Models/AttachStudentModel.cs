using Business_Logic.Enums;

namespace ticonet.Models
{
    public class AttachStudentModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int StationId { get; set; }

        public int Distance { get; set; }

        public int? LineId { get; set; }

        public int UseColor { get; set; }

        public ConflictActions ConflictAction { get; set; }

        public string StrDate { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }
    }
}