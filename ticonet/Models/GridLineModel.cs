﻿using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic;
using Business_Logic.Enums;
using Business_Logic.Helpers;

namespace ticonet.Models
{
    public class GridLineModel
    {

        public GridLineModel()
        {
        }

        public GridLineModel(Line data)
        {
            var bus = data.BusesToLines.Select(x => x.Bus).FirstOrDefault();
            Id = data.Id;
            LineName = data.LineName;
            LineNumber = data.LineNumber;
            Direction = (LineDirection)data.Direction;
            IsActive = data.IsActive;
            totalStudents = data.totalStudents ?? 0;
            Duration = data.Duration;
            Sun = data.Sun.HasValue ? data.Sun.Value : false;
            Mon = data.Mon.HasValue ? data.Mon.Value : false;
            Tue = data.Tue.HasValue ? data.Tue.Value : false;
            Wed = data.Wed.HasValue ? data.Wed.Value : false;
            Thu = data.Thu.HasValue ? data.Thu.Value : false;
            Fri = data.Fri.HasValue ? data.Fri.Value : false;
            Sut = data.Sut.HasValue ? data.Sut.Value : false;
            Bus = bus != null ? bus.Id : (int)0;
            BusDescription = bus != null ? DescriptionHelper.GetBusDescription(bus) : string.Empty;
            BusId = bus != null ? bus.BusId : string.Empty;
            PlateNumber = bus != null ? bus.PlateNumber : string.Empty;
            BusCompanyName = bus != null ? (bus.BusCompany!=null? bus.BusCompany.companyName: string.Empty) : string.Empty;
            seats = bus != null ? bus.seats : null;
            price = bus != null ? bus.price : null;
        }

        public int Id { get; set; }

        public string LineName { get; set; }

        public string LineNumber { get; set; }

        public LineDirection Direction { get; set; }

        public bool IsActive { get; set; }

        public int totalStudents { get; set; }

        public TimeSpan? Duration { get; set; }

        public bool Sun { get; set; }

        public bool Mon { get; set; }

        public bool Tue { get; set; }

        public bool Wed { get; set; }

        public bool Thu { get; set; }

        public bool Fri { get; set; }

        public bool Sut { get; set; }

        public int Bus { get; set; }
        public string BusDescription { get; set; }

        public string BusId { get; set; }

        public string PlateNumber { get; set; }

        public string BusCompanyName { get; set; }

        public int? seats { get; set; }
        public double? price { get; set; }


        public string Oper { get; set; }

        public void UpdateDbModel(Line existingLine)
        {
            existingLine.LineName = LineName;
            existingLine.LineNumber = LineNumber;
            existingLine.Direction = (int)Direction;
            existingLine.IsActive = IsActive;
            // existingLine.totalStudents = totalStudents; // Can not be modefied in lines grid
            // existingLine.Duration = Duration; // Can not be modefied in lines grid
            existingLine.Sun = Sun;
            existingLine.Mon = Mon;
            existingLine.Tue = Tue;
            existingLine.Wed = Wed;
            existingLine.Thu = Thu;
            existingLine.Fri = Fri;
            existingLine.Sut = Sut;
        }
    }
}