using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Generic.Features.Account.MyAccount;
using XperienceCommunity.Authorization;

[assembly: RegisterPageTemplate(
    "Generic.Account_MyAccount",
    "My Account",
    typeof(MyAccountPageTemplateProperties),
    "~/Features/Account/MyAccount/MyAccountPageTemplate.cshtml")]
[assembly: RegisterPageBuilderAuthorization(pageTemplateIdentifiers: "Generic.Account_MyAccount", userAuthenticationRequired: true)]

namespace Generic.Features.Account.MyAccount
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class MyAccountPageTemplateProperties : IPageTemplateProperties
    {
        // Custom Properties here
    }
}