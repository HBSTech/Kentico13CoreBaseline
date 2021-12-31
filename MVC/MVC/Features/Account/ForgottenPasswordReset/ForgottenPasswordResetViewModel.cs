using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.ForgottenPasswordReset
{
    public class ForgottenPasswordResetViewModel
    {
        [Required]
        public Guid UserID { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [DisplayName("Confirm New Password")]
        public string PasswordConfirm { get; set; }

        public IdentityResult Result { get; set; }
        public string LoginUrl { get; set; }
    }
}
