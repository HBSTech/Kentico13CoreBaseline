using FluentValidation;
using Generic.Repositories.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models.Account
{
    public class BasicUser
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Converts a basic user to a user object
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            return new User()
            {
                UserName = UserName,
                Email = UserEmail,
                FirstName = FirstName,
                LastName = LastName
            };
        }
    }

    public class BasicUserValidator : AbstractValidator<BasicUser>
    {
        public BasicUserValidator(IUserRepository _userRepository)
        {
            RuleFor(model => model.UserEmail)
                .EmailAddress()
                .WithMessage("Invalid Email Address")
                .MustAsync(async (userEmail, thread) => (await _userRepository.GetUserByEmailAsync(userEmail)) == null)
                .WithMessage("User already exists with this email address.");
            RuleFor(model => model.UserName)
                .MustAsync(async (userName, thread) => (await _userRepository.GetUserAsync(userName)) == null)
                .WithMessage("User already exists with this username.");
        }
    }
}