using FluentValidation;
using Generic.Library.Validation;
using Generic.Models.Account;
using Generic.Repositories.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.Registration
{
    public class RegistrationViewModel
    {
        public BasicUser User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string PasswordConfirm { get; set; }
        public bool? RegisterationSuccessful { get; set; }
        public string RegistrationFailureMessage { get; set; }

        public RegistrationViewModel()
        {
        }
    }
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator(ISiteSettingsRepository _siteSettingsRepository)
        {
            var passwordSettings = _siteSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);
        }
    }
}