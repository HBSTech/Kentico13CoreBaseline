using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Account.Features.Account.LogOut;
using XperienceCommunity.Authorization;

[assembly: RegisterPageTemplate(
    "Generic.Account_LogOut",
    "Log Out",
    typeof(LogOutPageTemplateProperties),
    "/Features/Account/LogOut/LogOutPageTemplate.cshtml")]
[assembly: RegisterPageBuilderAuthorization(pageTemplateIdentifiers: "Generic.Account_LogOut", userAuthenticationRequired: true)]
namespace Account.Features.Account.LogOut
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class LogOutPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}