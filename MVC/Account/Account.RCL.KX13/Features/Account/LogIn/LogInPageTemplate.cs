using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Account.Features.Account.LogIn;


[assembly: RegisterPageTemplate(
    "Generic.Account_LogIn",
    "Log In",
    typeof(LogInPageTemplateProperties),
    "/Features/Account/LogIn/LogInPageTemplate.cshtml")]

namespace Account.Features.Account.LogIn
{
    // Template filter at /Features/Account/AccountPageTemplateFilter.cs

    public class LogInPageTemplateProperties : IPageTemplateProperties
    {

    }
}