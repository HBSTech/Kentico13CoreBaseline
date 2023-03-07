using Microsoft.AspNetCore.Identity;

namespace Account.Features.Account.Confirmation
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(IdentityResult result, bool isEditMode)
        {
            Result = result;
            IsEditMode = isEditMode;
        }

        public Maybe<string> LoginUrl { get; set; }
        public IdentityResult Result { get; set; }
        public bool IsEditMode { get; set; }
    }
}
