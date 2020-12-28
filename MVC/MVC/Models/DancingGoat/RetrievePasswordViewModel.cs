using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models
{
    public class RetrievePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string Email { get; set; }
    }
}