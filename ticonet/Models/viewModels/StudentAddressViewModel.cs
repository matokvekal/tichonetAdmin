namespace ticonet.Models.viewModels
{
    public class StudentAddressViewModel
    {
        public int StudentId { get; set; }

        public string City { get; set; }

        public int CityId { get; set; }

        public string Street { get; set; }

        public int StreetId { get; set; }

        public int HouseNumber { get; set; }

        public bool Confirm { get; set; }
    }
}