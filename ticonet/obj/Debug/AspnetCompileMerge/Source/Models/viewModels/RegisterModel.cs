using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Business_Logic;

namespace ticonet
{
    public class RegisterModel
    {

        [Required]
        [localizedSystemDisplayName("Login.userName")]
        [DataType(DataType.Text)]
        public string userName { get; set; }

         [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [localizedSystemDisplayName("Login.password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

         [Required]
        [localizedSystemDisplayName("Login.ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("password",ErrorMessage="Password and Confirm Password do NotFiniteNumberException mutch")]
        public string confirmPassword { get; set; }

         [Required]
        [localizedSystemDisplayName("Login.entranceCode")]
        [DataType(DataType.Text)]
        public string entranceCode { get; set; }
    }
}