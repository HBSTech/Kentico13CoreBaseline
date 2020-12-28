//using Generic.Attributes;
//using HBS.LocalizedValidationAttributes.Kentico.MVC;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models
{
    public class ResetForgotPassword
    {
        [Required]
        //[LocalizedUserExists(ErrorMessage = "User not found")]
        public Guid UserID { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[LocalizedPasswordPolicy]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[LocalizedPasswordPolicy]
        //[LocalizedCompare("Password")]
        [DisplayName("Confirm New Password")]
        public string PasswordConfirm { get; set; }
    }
}