//using Generic.Attributes;
//using HBS.LocalizedValidationAttributes.Kentico.MVC;
using Generic.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        [CurrentUserPasswordValid(ErrorMessage = "{$ validation.invalidpassword $}")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[LocalizedPasswordPolicy]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[LocalizedPasswordPolicy]
        [DisplayName("Confirm New Password")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}