using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }


        public string Token { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Confirm your password")]
        [Required(ErrorMessage = "Please confirm your password")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        [Compare("Password", ErrorMessage = "Password does not match the confirmation password")]
        public string PasswordConfirmation { get; set; }
    }
}