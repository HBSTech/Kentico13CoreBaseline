using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models
{
    public class RegisterViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter your email")]
        [DisplayName("Email (User name)")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string UserName { get; set; }


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


        [DisplayName("First name")]
        [Required(ErrorMessage = "Please enter your first name")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string FirstName { get; set; }


        [DisplayName("Last name")]
        [Required(ErrorMessage = "Please enter your last name")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string LastName { get; set; }
    }
}