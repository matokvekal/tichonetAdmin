using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic.Services
{
    public class ScheduleService : IScheduleService
    {

        public bool PopulateLinesPlan()
        {
            var linesIds = new List<int>();
            var dateFrom = DateHelper.GetSunday(DateTime.Now); // DateTime.Now.AddDays(1).Date;
            var dateTo = dateFrom.AddDays(7).Date;

            using (var logic = new LineLogic())
            {
                linesIds = logic.GetList().Select(x => x.Id).ToList();
            }
            var parameters = new ScheduleParamsModel
            {
                LinesIds = string.Join(",", linesIds),
                DateFrom = DateHelper.DateToString(dateFrom),
                DateTo = DateHelper.DateToString(dateTo),
                ArriveTime = true,
                LeaveTime = true,
                Sun = true,
                Mon = true,
                Tue = true,
                Wed = true,
                Thu = true,
                Fri = true,
                Sut = true,
            };

            using (var logic = new tblLinesPlanLogic())
                logic.SyncLinesToPlans();
            var schedule = GenerateSchedule(parameters).ToList();
            return SaveGeneratedShcedule(schedule, dateFrom, dateTo);
        }


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
                        var item = GenerateSingleSchedule(line, date, parameters.LeaveTime, parameters.ArriveTime);
                        item.Id = fakeId++;
                        schedule.Add(item);
                    }
                }
            }
            return schedule;
        }

        //todo use it in GenerateSchedule
        public tblSchedule GenerateSingleSchedule (Line line, DateTime date, bool AddLeaveTime, bool AddArriveTime) {
            var item = new tblSchedule {
                Date = date,
                Direction = line.Direction,
                LineId = line.Id,
                BusId = line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().BusId,
                Line = line,
                Bus = line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().Bus
            };

            var leaveTime = GetLeaveTime(line, date);
            if (leaveTime.HasValue) {
                if (AddLeaveTime) 
                    item.leaveTime = leaveTime;
                if (AddArriveTime && line.Duration.HasValue) 
                    item.arriveTime = leaveTime.Value.Add(line.Duration.Value);
            }
            return item;
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
        
        private List<DateTime> GetScheduleLineDates(IWeekDatedObject line, DateTime dateFrom, DateTime dateTo, ScheduleParamsModel parameters)
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
        
        private DateTime? GetLeaveTime(Line line, DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    if (line.Sun.HasValue && line.Sun.Value) { return line.SunTime; }
                    break;
                case DayOfWeek.Monday:
                    if (line.Mon.HasValue && line.Mon.Value) { return line.MonTime; }
                    break;
                case DayOfWeek.Tuesday:
                    if (line.Tue.HasValue && line.Tue.Value) { return line.TueTime; }
                    break;
                case DayOfWeek.Wednesday:
                    if (line.Wed.HasValue && line.Wed.Value) { return line.WedTime; }
                    break;
                case DayOfWeek.Thursday:
                    if (line.Thu.HasValue && line.Thu.Value) { return line.ThuTime; }
                    break;
                case DayOfWeek.Friday:
                    if (line.Fri.HasValue && line.Fri.Value) { return line.FriTime; }
                    break;
                case DayOfWeek.Saturday:
                    if (line.Sut.HasValue && line.Sut.Value) { return line.SutTime; }
                    break;
            }
            return null;
        }
    }
}
