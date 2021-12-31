using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Account.Confirmation;

[assembly: RegisterPageTemplate(
    "Generic.Account_Confirmation",
    "Registration Confirmation",
    typeof(ConfirmationPageTemplateProperties),
    "~/Features/Account/Confirmation/ConfirmationPageTemplate.cshtml")]

namespace Generic.Features.Account.Confirmation
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class ConfirmationPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}