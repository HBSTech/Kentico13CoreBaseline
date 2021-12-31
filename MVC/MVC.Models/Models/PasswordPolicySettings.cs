namespace Generic.Models
{
    public class PasswordPolicySettings
    {
        public bool UsePasswordPolicy { get; set; }
        public int MinLength { get; set; }
        public int NumNonAlphanumericChars { get; set; }
        public string Regex { get; set; }
        public string ViolationMessage { get; set; }
    }
}