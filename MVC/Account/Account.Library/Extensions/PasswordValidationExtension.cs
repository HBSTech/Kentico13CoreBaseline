using FluentValidation;

namespace Account.Extensions
{
    public static class PasswordValidationExtension
    {

        public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder, PasswordPolicySettings settings)
        {
            string message = !string.IsNullOrWhiteSpace(settings.ViolationMessage) ? settings.ViolationMessage : "Invalid Password";
            var options = ruleBuilder.NotNull();
            if (settings.UsePasswordPolicy)
            {
                if (settings.MinLength > 0)
                {
                    options.MinimumLength(settings.MinLength).WithMessage(message);
                }
                if (!string.IsNullOrWhiteSpace(settings.Regex))
                {
                    options.Matches(settings.Regex).WithMessage(message);
                }
                if (settings.NumNonAlphanumericChars > 0)
                {
                    options.Matches($"^(?=.{{{settings.NumNonAlphanumericChars},999}}\\W).*$").WithMessage(message);
                }
            }
            return options;
        }

    }
}
