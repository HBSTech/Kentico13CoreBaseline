using System.ComponentModel.DataAnnotations;

namespace Generic.Features.Account.LogIn
{
    public record TwoFormAuthenticationViewModel
    {
        public string UserName { get; set; }
        public string RedirectUrl { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Must provide code")]
        public string TwoFormCode { get; set; }
        public bool StayLoggedIn { get; set; }
        [Display(Name = "Remember Device")]
        public bool RememberComputer { get; set; } = false;
        public bool Failure { get; set; } = false;
    }
}
