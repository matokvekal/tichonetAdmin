using System;
using Business_Logic;
using Business_Logic.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ticonet.Models
{
    public class EventModel
    {
        public EventModel(tblCalendar data)
        {
            pk = data.pk;
            date = DateHelper.DateToString(data.date);
            month = data.month;
            HebMonth = data.HebMonth;
            day = data.day;
            active = data.active;
            calendarEvent = data.@event;
        }

        public EventModel() { }

        public int pk { get; set; }

        public string date { get; set; }

        public string month { get; set; }

        public string HebMonth { get; set; }

        public string day { get; set; }

        public bool? active { get; set; }

        public string calendarEvent { get; set; }

        public string Oper { get; set; }
    }
}