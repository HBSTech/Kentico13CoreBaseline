using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Account.Registration;

[assembly: RegisterPageTemplate(
    "Generic.Account_Registration",
    "Registration",
    typeof(RegistrationPageTemplateProperties),
    "~/Features/Account/Registration/RegistrationPageTemplate.cshtml")]

namespace Generic.Features.Account.Registration
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class RegistrationPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}