using System.ComponentModel.DataAnnotations;

namespace Generic.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}