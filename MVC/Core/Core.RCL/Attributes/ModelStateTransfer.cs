using Microsoft.AspNetCore.Mvc.Filters;

namespace Core.Attributes
{
    /// <summary>
    /// https://www.exceptionnotfound.net/the-post-redirect-get-pattern-in-asp-net-mvc/
    /// </summary>
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTransfer)?.FullName ?? string.Empty;
    }
}
