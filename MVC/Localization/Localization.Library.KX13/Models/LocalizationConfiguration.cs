namespace Localization.Models
{
    public class LocalizationConfiguration
    {
        public LocalizationConfiguration(string defaultCulture)
        {
            DefaultCulture = defaultCulture;
        }

        public string DefaultCulture { get; set; }
    }
}
