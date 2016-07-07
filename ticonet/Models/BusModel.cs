using Business_Logic;
using Business_Logic.Helpers;

namespace ticonet.Models
{
    public class BusModel
    {
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
            munifacturedate = DateHelper.DateToString(data.munifacturedate);
            LicensingDueDate = DateHelper.DateToString(data.LicensingDueDate);
            insuranceDueDate = DateHelper.DateToString(data.insuranceDueDate);
            winterLicenseDueDate = DateHelper.DateToString(data.winterLicenseDueDate);
            brakeTesDueDate = DateHelper.DateToString(data.brakeTesDueDate);
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
                munifacturedate = DateHelper.StringToDate(munifacturedate),
                LicensingDueDate = DateHelper.StringToDate(LicensingDueDate),
                insuranceDueDate = DateHelper.StringToDate(insuranceDueDate),
                winterLicenseDueDate = DateHelper.StringToDate(winterLicenseDueDate),
                brakeTesDueDate = DateHelper.StringToDate(brakeTesDueDate)
            };
        }
    }
}