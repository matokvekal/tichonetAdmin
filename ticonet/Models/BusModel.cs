using System;
using System.Collections.Generic;
using System.Globalization;
using Business_Logic;

namespace ticonet.Models
{
    public class BusModel
    {
        string DateToString(DateTime? dt)
        {
            return (dt.HasValue? dt.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) :"") ;
        }
        DateTime? StringToDate(string s)
        {
            DateTime? dtNull = null;
            if (!string.IsNullOrWhiteSpace(s))
            {
                DateTime dt =  new DateTime();
                if(DateTime.TryParseExact(s, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dtNull = (DateTime?)dt;
            }
            return dtNull;
        }

        public BusModel(Bus data)
        {
            Id = data.Id;
            BusId = data.BusId;
            PlateNumber = data.PlateNumber;
            //BusType = data.BusType;
            //Occupation = data.Occupation;
            Owner = data.Owner;
            OwnerDescription = data.BusCompany != null ? data.BusCompany.companyName : string.Empty;
            //GpsSource = data.GpsSource;
            //GpsCode = data.GpsCode;
            seats = data.seats;
            price = data.price;
            munifacturedate = DateToString(data.munifacturedate);
            LicensingDueDate = DateToString(data.LicensingDueDate);
            insuranceDueDate = DateToString(data.insuranceDueDate);
            winterLicenseDueDate = DateToString(data.winterLicenseDueDate);
            brakeTesDueDate = DateToString(data.brakeTesDueDate);
        }

        public BusModel() { }

        public int Id { get; set; }

        public string BusId { get; set; }

        public string PlateNumber { get; set; }

        //public int? BusType { get; set; }

        //public int? Occupation { get; set; }

        public string OwnerDescription { get; set; }
        public int? Owner { get; set; }

        //public int? GpsSource { get; set; }

        //public string GpsCode { get; set; }

        public int? seats { get; set; }

        public double? price { get; set; }

        public string munifacturedate { get; set; }

        public string LicensingDueDate { get; set; }

        public string insuranceDueDate { get; set; }

        public string winterLicenseDueDate { get; set; }

        public string brakeTesDueDate { get; set; }

        public string Oper { get; set; }

        public Bus ToDbModel()
        {
            return new Bus
            {

                Id = Id,
                BusId = BusId,
                PlateNumber = PlateNumber,
                //BusType = BusType,
                //Occupation = Occupation,
                Owner = Owner,
                //GpsSource = GpsSource,
                //GpsCode = GpsCode,
                seats = seats,
                price = price,
                munifacturedate = StringToDate(munifacturedate),
                LicensingDueDate = StringToDate(LicensingDueDate),
                insuranceDueDate = StringToDate(insuranceDueDate),
                winterLicenseDueDate = StringToDate(winterLicenseDueDate),
                brakeTesDueDate = StringToDate(brakeTesDueDate)
            };
        }
    }
}