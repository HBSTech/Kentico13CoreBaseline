using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Account.Features.Account.Registration
{
    [Serializable]
    public class RegistrationViewModel
    {
        public BasicUser User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string PasswordConfirm { get; set; } = string.Empty;

        public bool? RegistrationSuccessful { get; set; }

        public string RegistrationFailureMessage { get; set; } = string.Empty;

        public RegistrationViewModel()
        {
            User = new BasicUser();
        }
    }
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator(IAccountSettingsRepository _accountSettingsRepository, IUserRepository userRepository)
        {
            var passwordSettings = _accountSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);
            RuleFor(model => model.User.UserName).MustAsync(async (model, cancellationToken) =>
            {
                return (await userRepository.GetUserAsync(model)).IsFailure;
            }).WithMessage("User already exists with that username");
            RuleFor(model => model.User.UserEmail).MustAsync(async (model, cancellationToken) =>
            {
                return (await userRepository.GetUserByEmailAsync(model)).IsFailure;
            }).WithMessage("User already exists with that email address");
        }
    }
}