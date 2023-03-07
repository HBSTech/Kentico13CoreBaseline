using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Account.Features.Account.ForgotPassword;

[assembly: RegisterPageTemplate(
    "Generic.Account_ForgotPassword",
    "Forgot Password",
    typeof(ForgotPasswordPageTemplateProperties),
    "/Features/Account/ForgotPassword/ForgotPasswordPageTemplate.cshtml")]

namespace Account.Features.Account.ForgotPassword
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class ForgotPasswordPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}