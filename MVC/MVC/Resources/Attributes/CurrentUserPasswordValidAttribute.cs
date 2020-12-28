using CMS.Base;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Generic.Attributes
{
    /// <summary>
    /// Checks if the password matches the current user's password.  Used in Password reset.
    /// </summary>
    public class CurrentUserPasswordValidAttribute : ValidationAttribute
    {
        public CurrentUserPasswordValidAttribute()
        {
        }

        public IUserService UserService { get; internal set; }
        public IUserRepository UserRepository { get; internal set; }
        public IAuthenticationService AuthenticationService { get; internal set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UserService = (IUserService)validationContext.GetService(typeof(IUserService));
            UserRepository = (IUserRepository)validationContext.GetService(typeof(IUserRepository));
            AuthenticationService = (IAuthenticationService)validationContext.GetService(typeof(IAuthenticationService));
            if (value is string)
            {
                string Password = value.ToString();
                if (AuthenticationService.CurrentUser.IsPublic())
                {
                    return new ValidationResult("Cannot change password of Public User");
                }
                if(UserService.ValidateUserPassword(AuthenticationService.CurrentUser, Password))
                {
                    return ValidationResult.Success;
                } else
                {
                    return new ValidationResult("Invalid Password");
                }
            }
            else
            {
                return new ValidationResult("Password must be a string");
            }
        }

    }
}
