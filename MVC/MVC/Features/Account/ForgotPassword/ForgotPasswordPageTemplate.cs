using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Account.ForgotPassword;

[assembly: RegisterPageTemplate(
    "Generic.Account_ForgotPassword",
    "Forgot Password",
    typeof(ForgotPasswordPageTemplateProperties),
    "~/Features/Account/ForgotPassword/ForgotPasswordPageTemplate.cshtml")]

namespace Generic.Features.Account.ForgotPassword
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class ForgotPasswordPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}