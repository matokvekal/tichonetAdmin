using Business_Logic.Enums;
using Newtonsoft.Json;

namespace Business_Logic.Entities
{
    public class LoginInfo
    {
        public LoginInfo() { }

        public LoginInfo(Login userFromDb)
        {
            UserId = userFromDb.userId;
            UserNameEmail = userFromDb.userName;
            Password = userFromDb.Password;
            UserRole = UserRole.Admin;
        }

        public LoginInfo(tblStudent userFromDb)
        {
            UserId = userFromDb.pk;
            UserNameEmail = userFromDb.Email;
            Password = string.Empty;
            UserRole = UserRole.Student;
        }

        [JsonProperty]
        public int UserId { get; set; }

        [JsonProperty]
        public UserRole UserRole { get; set; }

        //[JsonProperty]
        //public int CityId { get; set; }

        //[JsonProperty]
        //public int DepartmentId { get; set; }

        [JsonProperty]
        public string UserNameEmail { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        //[JsonProperty]
        //public string FirstName { get; set; }

        //[JsonProperty]
        //public string LastName { get; set; }

        [JsonProperty]
        public string InterfaceLanguage { get; set; }

        //[JsonProperty]
        //public string RegisteredDate { get; set; }

        //[JsonProperty]
        //public string ProfileAvatar { get; set; }
    }
}
