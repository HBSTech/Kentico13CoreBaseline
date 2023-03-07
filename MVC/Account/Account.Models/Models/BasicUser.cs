using Core.Repositories;
using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Account.Models
{
    public class BasicUser
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Converts a basic user to a user object
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            return new User(
                userName: UserName,
                firstName: FirstName,
                lastName: LastName,
                email: UserEmail,
                enabled: false,
                isExternal: false,
                isPublic: false
            );
        }
    }

    public class BasicUserValidator : AbstractValidator<BasicUser>
    {
        public BasicUserValidator(IUserRepository _userRepository)
        {
            RuleFor(model => model.UserEmail)
                .EmailAddress()
                .WithMessage("Invalid Email Address")
                .MustAsync(async (userEmail, thread) => (await _userRepository.GetUserByEmailAsync(userEmail)).IsFailure)
                .WithMessage("User already exists with this email address.");
            RuleFor(model => model.UserName)
                .MustAsync(async (userName, thread) => (await _userRepository.GetUserAsync(userName)).IsFailure)
                .WithMessage("User already exists with this username.");
        }
    }
}