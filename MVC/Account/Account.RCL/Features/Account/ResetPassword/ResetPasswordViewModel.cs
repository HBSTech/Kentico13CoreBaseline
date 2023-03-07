using Account.Extensions;
using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Account.Features.Account.ResetPassword
{
    [Serializable]
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password")]
        [Compare(nameof(Password))]
        public string PasswordConfirm { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public bool? Succeeded { get; set; }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordViewModel>
    {

        public ResetPasswordValidator(IAccountSettingsRepository _accountSettingsRepository,
            IUserRepository _userRepository,
            IUserService _userService
            )
        {
            var passwordSettings = _accountSettingsRepository.GetPasswordPolicy();

            RuleFor(model => model.Password).ValidPassword(passwordSettings);
            RuleFor(model => model.PasswordConfirm).Equal(model => model.Password);
            RuleFor(model => model.CurrentPassword).MustAsync(async (password, thread) =>
            {
                var user = await _userRepository.GetCurrentUserAsync();
                return await _userService.ValidateUserPasswordAsync(user, password);
            }).WithMessage("Given password does not match your current password");

        }
    }
}
