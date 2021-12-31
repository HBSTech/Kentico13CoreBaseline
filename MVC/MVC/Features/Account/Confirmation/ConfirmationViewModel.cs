using Microsoft.AspNetCore.Identity;

namespace Generic.Features.Account.Confirmation
{
    public class ConfirmationViewModel
    {
        public string LoginUrl { get; set; }
        public IdentityResult Result { get; set; }
        public bool IsEditMode { get; set; }
    }
}
