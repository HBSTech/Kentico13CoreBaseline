using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Account.Features.Account.ResetPassword;
using XperienceCommunity.Authorization;

[assembly: RegisterPageTemplate(
    "Generic.Account_ResetPassword",
    "Reset Password",
    typeof(ResetPasswordPageTemplateProperties),
    "/Features/Account/ResetPassword/ResetPasswordPageTemplate.cshtml")]
[assembly: RegisterPageBuilderAuthorization(pageTemplateIdentifiers: "Generic.Account_ResetPassword", userAuthenticationRequired: true)]

namespace Account.Features.Account.ResetPassword
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class ResetPasswordPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}