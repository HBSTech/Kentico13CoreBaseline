using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Account.Features.Account.ForgottenPasswordReset
{
    public class ForgottenPasswordResetViewModel
    {
        [Required]
        public Guid UserID { get; set; } = Guid.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password")]
        public string PasswordConfirm { get; set; } = string.Empty;

        public IdentityResult? Result { get; set; }

        /// <summary>
        /// Identityresult doesn't serialize/deserialize properly so need to use this as a toggle
        /// </summary>
        public bool? Succeeded { get; set; }
        public string LoginUrl { get; set; } = string.Empty;
    }

    public class ForgottenPasswordResetViewModelValidator : AbstractValidator<ForgottenPasswordResetViewModel>
    {
        public ForgottenPasswordResetViewModelValidator(IAccountSettingsRepository _accountSettingsRepository)
        {
            var passwordSettings = _accountSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);

        }
    }
}
