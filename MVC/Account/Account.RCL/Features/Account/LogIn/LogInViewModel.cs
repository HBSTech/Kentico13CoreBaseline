using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Account.Features.Account.LogIn
{
    public class LogInViewModel
    {

        [Display(Name = "Username", Prompt = "Enter your username or email")]
        [Required(ErrorMessage = "Username Address Required")]
        public string UserName { get; set; } = string.Empty;


        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "{$ form.password $}")]
        public string Password { get; set; } = string.Empty;


        [DisplayName("Stay loged in on this computer")]
        public bool StayLogedIn { get; set; } = false;
        public string MyAccountUrl { get; set; } = string.Empty;
        public string RegistrationUrl { get; set; } = string.Empty;
        public string ForgotPassword { get; set; } = string.Empty;
        public bool AlreadyLogedIn { get; set; } = false;
        public SignInResult? Result { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public List<AuthenticationScheme> ExternalLoginProviders { get; set; } = new List<AuthenticationScheme>();
    }
}