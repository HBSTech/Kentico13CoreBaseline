using FluentValidation;
using Generic.Library.Validation;
using Generic.Models.Account;
using Generic.Repositories.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.Registration
{
    [Serializable]
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

        public bool? RegistrationSuccessful { get; set; }

        public string RegistrationFailureMessage { get; set; }

        public RegistrationViewModel()
        {
        }
    }
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator(ISiteSettingsRepository _siteSettingsRepository, IUserRepository userRepository)
        {
            var passwordSettings = _siteSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);
            RuleFor(model => model.User.UserName).MustAsync(async (model, cancellationToken) =>
            {
                return (await userRepository.GetUserAsync(model)) == null;
            }).WithMessage("User already exists with that username");
            RuleFor(model => model.User.UserEmail).MustAsync(async (model, cancellationToken) =>
            {
                return (await userRepository.GetUserByEmailAsync(model)) == null;
            }).WithMessage("User already exists with that email address");
        }
    }
}