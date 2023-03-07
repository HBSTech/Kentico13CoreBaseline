using Microsoft.Extensions.Primitives;

namespace Account.Features.Account.ForgottenPasswordReset
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
            Maybe<Guid> userId = Maybe.None;
            Maybe<string> token = Maybe.None;
            if(_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
            {
                if (httpContext.Request.Query.TryGetValue("userId", out StringValues queryUserID) && queryUserID.Any())
                {
                    if (Guid.TryParse(queryUserID, out Guid userIdTemp))
                    {
                        userId = userIdTemp;
                    }
                }
                if (httpContext.Request.Query.TryGetValue("token", out StringValues queryToken) && queryToken.Any())
                {
                    token = queryToken.First();
                }
            }

            var model = _modelStateService.GetViewModel<ForgottenPasswordResetViewModel>(TempData).GetValueOrDefault(new ForgottenPasswordResetViewModel()
            {
                UserID = userId.GetValueOrDefault(Guid.Empty),
                Token = token.GetValueOrDefault(string.Empty)
            });

            return View("/Features/Account/ForgottenPasswordReset/ForgottenPasswordReset.cshtml", model);
        }
    }
}
