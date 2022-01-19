using Generic.Services.Interfaces;
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
        private readonly IModelStateService _modelStateService;

        public ForgottenPasswordResetViewComponent(IHttpContextAccessor httpContextAccessor,
            IModelStateService modelStateService)
        {
            _httpContextAccessor = httpContextAccessor;
            _modelStateService = modelStateService;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            // Merge Model State
            _modelStateService.MergeModelState(ModelState, TempData);

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

            var model = _modelStateService.GetViewModel<ForgottenPasswordResetViewModel>(TempData) ?? new ForgottenPasswordResetViewModel()
            {
                UserID = userId ?? Guid.Empty,
                Token = token
            };

            return View("~/Features/Account/ForgottenPasswordReset/ForgottenPasswordReset.cshtml", model);
        }
    }
}
