using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Account.Features.Account.ForgottenPasswordReset;

[assembly: RegisterPageTemplate(
    "Generic.Account_ForgottenPasswordReset",
    "Forgotten Password Reset",
    typeof(ForgottenPasswordResetPageTemplateProperties),
    "/Features/Account/ForgottenPasswordReset/ForgottenPasswordResetPageTemplate.cshtml")]

namespace Account.Features.Account.ForgottenPasswordReset
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class ForgottenPasswordResetPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}