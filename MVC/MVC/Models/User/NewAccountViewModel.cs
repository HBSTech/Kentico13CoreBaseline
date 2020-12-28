using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models.User
{
    public class NewAccountViewModel
    {
        public BasicUser User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,20}$", ErrorMessage = "Password must be at least 8 characters long and contain at least 1 uppercase, 1 lowercase, 1 number and 1 symbol.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,20}$", ErrorMessage = "Password must be at least 8 characters long and contain at least 1 uppercase, 1 lowercase, 1 number and 1 symbol.")]
        [DisplayName("Confirm Password")]
        public string PasswordConfirm { get; set; }

        public NewAccountViewModel()
        {
        }
    }
}