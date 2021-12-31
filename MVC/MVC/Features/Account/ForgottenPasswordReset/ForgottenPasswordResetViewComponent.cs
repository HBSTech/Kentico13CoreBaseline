using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace Generic.Features.Account.ForgottenPasswordReset
{
    [ViewComponent]
    public class ForgottenPasswordResetViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgottenPasswordResetViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Get values from Query String
            Guid? userId = null;
            string token = null;
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("userId", out StringValues queryUserID) && queryUserID.Any())
            {
                if(Guid.TryParse(queryUserID, out Guid userIdTemp))
                {
                    userId = userIdTemp;
                }
            }
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("token", out StringValues queryToken) && queryToken.Any())
            {
                token = queryToken.FirstOrDefault();
            }

            var model = new ForgottenPasswordResetViewModel()
            {
                UserID = userId ?? Guid.Empty,
                Token = token
            };

            return View("~/Features/Account/ForgottenPasswordReset/ForgottenPasswordReset.cshtml", model);
        }
    }
}
