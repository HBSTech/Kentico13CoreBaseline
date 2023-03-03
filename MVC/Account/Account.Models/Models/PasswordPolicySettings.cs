namespace Account.Models
{
    public class PasswordPolicySettings
    {
        public PasswordPolicySettings(bool usePasswordPolicy, int minLength, int numNonAlphanumericChars, string regex, string violationMessage)
        {
            UsePasswordPolicy = usePasswordPolicy;
            MinLength = minLength;
            NumNonAlphanumericChars = numNonAlphanumericChars;
            Regex = regex;
            ViolationMessage = violationMessage;
        }

        public bool UsePasswordPolicy { get; set; }
        public int MinLength { get; set; }
        public int NumNonAlphanumericChars { get; set; }
        public string Regex { get; set; }
        public string ViolationMessage { get; set; }
    }
}
