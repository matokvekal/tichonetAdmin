using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic.Services
{
    public class ScheduleService : IScheduleService
    {
        public IEnumerable<tblSchedule> GenerateSchedule(ScheduleParamsModel parameters)
        {
            var dateFrom = DateHelper.StringToDate(parameters.DateFrom);
            var dateTo = DateHelper.StringToDate(parameters.DateTo);
            var schedule = new List<tblSchedule>();
            var fakeId = 0;

            if (!dateFrom.HasValue || !dateTo.HasValue || string.IsNullOrEmpty(parameters.LinesIds))
            {
                return null;
            }

            using (var logic = new LineLogic())
            {
                var lines = logic.GetLines(parameters.LinesIds.Split(',').Select(int.Parse));

                foreach (var line in lines)
                {
                    var dates = GetScheduleLineDates(line, dateFrom.Value, dateTo.Value, parameters);
                    foreach (var date in dates)
                    {
                        schedule.Add(new tblSchedule
                        {
                            Id = fakeId++,
                            Date = date,
                            Direction = line.Direction,
                            LineId = line.Id,
                            BusId = line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().BusId,
                            Line = line,
                            Bus = line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().Bus
                        });
                    }
                }
            }
            return schedule;
        }

        public bool SaveGeneratedShcedule(IEnumerable<tblSchedule> schedule, DateTime dateFrom, DateTime dateTo)
        {
            using (var logic = new tblScheduleLogic())
            {
                var scheduleArr = schedule != null ? schedule.ToArray() : new tblSchedule[0];
                var linesIds = scheduleArr
                    .Where(x => x.LineId.HasValue)
                    .Select(x => x.LineId.Value)
                    .Distinct();

                var itemsToDelete = logic.Schedule
                    .Where(x => x.LineId.HasValue)
                    .Where(x => linesIds.Any(id => id == x.LineId.Value) && x.Date >= dateFrom && x.Date <= dateTo);
                logic.DeleteItems(itemsToDelete);

                foreach (var item in scheduleArr)
                {
                    item.Line = null;
                    item.Bus = null;
                    logic.SaveItem(item);
                }
            }
            return true;
        }
        
        private List<DateTime> GetScheduleLineDates(Line line, DateTime dateFrom, DateTime dateTo, ScheduleParamsModel parameters)
        {
            var dates = new List<DateTime>();
            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (line.Sun.HasValue && line.Sun.Value && parameters.Sun) { dates.Add(date); }
                        break;
                    case DayOfWeek.Monday:
                        if (line.Mon.HasValue && line.Mon.Value && parameters.Mon) { dates.Add(date); }
                        break;
                    case DayOfWeek.Tuesday:
                        if (line.Tue.HasValue && line.Tue.Value && parameters.Tue) { dates.Add(date); }
                        break;
                    case DayOfWeek.Wednesday:
                        if (line.Wed.HasValue && line.Wed.Value && parameters.Wed) { dates.Add(date); }
                        break;
                    case DayOfWeek.Thursday:
                        if (line.Thu.HasValue && line.Thu.Value && parameters.Thu) { dates.Add(date); }
                        break;
                    case DayOfWeek.Friday:
                        if (line.Fri.HasValue && line.Fri.Value && parameters.Fri) { dates.Add(date); }
                        break;
                    case DayOfWeek.Saturday:
                        if (line.Sut.HasValue && line.Sut.Value && parameters.Sut) { dates.Add(date); }
                        break;
                }
            }
            return dates;
        }
    }
}
