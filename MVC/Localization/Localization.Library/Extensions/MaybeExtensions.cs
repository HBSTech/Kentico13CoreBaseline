using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace Localization.Extensions
{
    public static class MaybeExtensions
    {
        public static Maybe<string> MaybeIfResourceFound(this LocalizedString value)
        {
            return value.ResourceNotFound ? Maybe.None : value.Value;
        }

        public static Maybe<string> MaybeIfResourceFound(this LocalizedHtmlString value)
        {
            return value.IsResourceNotFound ? Maybe.None : value.Value;
        }
    }
}
