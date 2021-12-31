using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.LogIn
{
    public class LogInViewModel
    {
        
        [Display(Name = "Username", Prompt = "Enter your username or email")]
        [Required(ErrorMessage = "Username Address Required")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "{$ form.password $}")]
        public string Password { get; set; }


        [DisplayName("Stay loged in on this computer")]
        public bool StayLogedIn { get; set; }
        public string MyAccountUrl { get; set; }
        public string RegistrationUrl { get; set; }
        public string ForgotPassword { get; set; }
        public bool AlreadyLogedIn { get; set; } = false;
        public SignInResult Result { get; set; }
        public string RedirectUrl { get; set; }
    }
}