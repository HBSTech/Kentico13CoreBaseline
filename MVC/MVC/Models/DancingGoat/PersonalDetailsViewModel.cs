using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Kentico.Membership;

namespace DancingGoat.Models
{
    public class PersonalDetailsViewModel
    {
        [DisplayName("Email / User name")]
        public string UserName { get; set; }


        [DisplayName("First name")]
        [Required(ErrorMessage = "Please enter your first name")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string FirstName { get; set; }


        [DisplayName("Last name")]
        [Required(ErrorMessage = "Please enter your last name")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string LastName { get; set; }


        public PersonalDetailsViewModel()
        {
        }


        public PersonalDetailsViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}