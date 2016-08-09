using System;
using System.Collections.Generic;

namespace Business_Logic.Dtos
{
    public class LinePeriodStatisticDto
    {
        //REMEMBER, THIS IS USED IN JAVASCRIPT
        public const string DayScheduleData_INACTIVE = "inactive";

        public int Id { get; set; }
        public string LineName { get; set; }
        public string LineNumber { get; set; }
        public int Direction { get; set; }
        public int totalStudents { get; set; }
        public int? seats { get; set; }
        public double? price { get; set; }
        public string BusCompanyName { get; set; }

        public List<string> DayScheduleData { get; set; }
        public List<DateTime> DayDate { get; set; }

    }
}
