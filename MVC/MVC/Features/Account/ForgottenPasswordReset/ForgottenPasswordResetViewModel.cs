using FluentValidation;
using Generic.Library.Validation;
using Generic.Repositories.Interfaces;
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
        [DisplayName("Confirm New Password")]
        public string PasswordConfirm { get; set; }

        public IdentityResult Result { get; set; }

        /// <summary>
        /// Identityresult doesn't serialize/deserialize properly so need to use this as a toggle
        /// </summary>
        public bool? Succeeded { get; set; }
        public string LoginUrl { get; set; }
    }

    public class ForgottenPasswordResetViewModelValidator : AbstractValidator<ForgottenPasswordResetViewModel>
    {
        public ForgottenPasswordResetViewModelValidator(ISiteSettingsRepository _siteSettingsRepository)
        {
            var passwordSettings = _siteSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);

        }
    }
}
