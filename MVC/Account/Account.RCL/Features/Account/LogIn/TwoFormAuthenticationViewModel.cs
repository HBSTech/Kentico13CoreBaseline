using System.ComponentModel.DataAnnotations;

namespace Account.Features.Account.LogIn
{
    public record TwoFormAuthenticationViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Must provide code")]
        public string TwoFormCode { get; set; } = string.Empty;
        public bool StayLoggedIn { get; set; }
        [Display(Name = "Remember Device")]
        public bool RememberComputer { get; set; } = false;
        public bool Failure { get; set; } = false;
    }
}
