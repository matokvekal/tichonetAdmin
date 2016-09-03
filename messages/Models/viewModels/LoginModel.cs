using System.ComponentModel.DataAnnotations;
using Business_Logic;

namespace ticonet
{
    public class LoginModel
    {
       // [Required]
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("Login.userName")]
        [DataType(DataType.Text)]
        public string userName { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("Login.password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [localizedSystemDisplayName("Login.RemmemberMe")]
        public bool RememberMe { get; set; }
    }
}
  