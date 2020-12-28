using Kentico.Membership;

namespace DancingGoat.Models
{
    public class YourAccountViewModel
    {
        public ApplicationUser User { get; set; }

        public bool AvatarUpdateFailed { get; set; }
    }
}