using System;
using System.Collections.Generic;
using Business_Logic;

namespace ticonet.Models
{
    public class BusModel
    {
        public BusModel(Bus data)
        {
            Id = data.Id;
            BusId = data.BusId;
            PlateNumber = data.PlateNumber;
            BusType = data.BusType;
            Occupation = data.Occupation;
            Owner = data.Owner;
            GpsSource = data.GpsSource;
            GpsCode = data.GpsCode;
            seats = data.seats;
            price = data.price;
            munifacturedate = data.munifacturedate;
            LicensingDueDate = data.LicensingDueDate;
            insuranceDueDate = data.insuranceDueDate;
            winterLicenseDueDate = data.winterLicenseDueDate;
            brakeTesDueDate = data.brakeTesDueDate;
        }

        public int Id { get; set; }

        public string BusId { get; set; }

        public string PlateNumber { get; set; }

        public int? BusType { get; set; }

        public int? Occupation { get; set; }

        public int? Owner { get; set; }

        public int? GpsSource { get; set; }

        public string GpsCode { get; set; }

        public int? seats { get; set; }

        public double? price { get; set; }

        public DateTime? munifacturedate { get; set; }

        public DateTime? LicensingDueDate { get; set; }

        public DateTime? insuranceDueDate { get; set; }

        public DateTime? winterLicenseDueDate { get; set; }

        public DateTime? brakeTesDueDate { get; set; }

        public string Oper { get; set; }
    }
}