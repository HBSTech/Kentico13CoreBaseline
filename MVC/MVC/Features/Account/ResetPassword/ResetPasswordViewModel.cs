using FluentValidation;
using Generic.Library.Validation;
using Generic.Repositories.Interfaces;
using Generic.Resources.Attributes;
using Generic.Services.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.ResetPassword
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password")]
        [Compare(nameof(Password))]
        public string PasswordConfirm { get; set; }
        public string Error { get; internal set; }

        public bool? Succeeded { get; set; }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordViewModel>
    {

        public ResetPasswordValidator(ISiteSettingsRepository _siteSettingsRepository,
            IUserRepository _userRepository,
            IUserService _userService
            )
        {
            var passwordSettings = _siteSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);
            RuleFor(model => model.CurrentPassword).ValidPassword(passwordSettings);
            RuleFor(model => model.CurrentPassword).MustAsync(async (password, thread) =>
            {
                var user = await _userRepository.GetCurrentUserAsync();
                return await _userService.ValidateUserPasswordAsync(user, password);
            }).WithMessage("Given password does not match your current password");

        }
    }
}
