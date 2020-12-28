using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models
{
    public class LoginViewModel
    {
        
        [DisplayName("Email Address")]
        [EmailAddress]
        [Required(ErrorMessage = "Email Address Required")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }


        [DisplayName("Stay signed in on this computer")]
        public bool StaySignedIn { get; set; }
    }
}