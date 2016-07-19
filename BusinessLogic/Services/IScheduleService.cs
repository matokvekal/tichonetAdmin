using System.Collections.Generic;
using Business_Logic.Entities;

namespace Business_Logic.Services
{
    public interface IScheduleService
    {
        IEnumerable<tblSchedule> GenerateSchedule(ScheduleParamsModel parameters);
    }
}