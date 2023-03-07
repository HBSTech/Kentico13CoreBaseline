using System;
using System.ComponentModel.DataAnnotations;

namespace Account.Features.Account.ForgotPassword
{
    [Serializable]
    public class ForgotPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        public bool? Succeeded { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
