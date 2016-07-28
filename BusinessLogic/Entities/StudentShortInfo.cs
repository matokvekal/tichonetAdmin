namespace Business_Logic.Entities
{
    public class StudentShortInfo
    {
        public StudentShortInfo()
        {

        }
        public StudentShortInfo(tblStudent data)
        {
            var c = data.Color ?? "";

            Id = data.pk;
            StudentId = data.studentId;
            Lat = data.Lat;
            Lng = data.Lng;
            Color = string.IsNullOrEmpty(c.Trim()) ? "FFFFFF" : c.Trim().Replace("#", "");
            Name = data.lastName + ", " + data.firstName;
            CellPhone = data.CellPhone;
            Email = data.Email;
            Address = (data.city ?? "") + ", " + (data.street ?? "") + ", " + data.houseNumber;
            Shicva = data.Shicva;
            Class = data.@class;
            Active = data.Active ?? false;
            request = data.request;
        }
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Shicva { get; set; }
        public string Class { get; set; }
        public string Address { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string request { get; set; }


    }
}
