using Account.Features.Account.LogIn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Account.Features.Account.Confirmation
{
    [ViewComponent]
    public class ConfirmationViewComponent : ViewComponent
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IAccountSettingsRepository _accountSettingsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPageContextRepository _pageContextRepository;

        public ConfirmationViewComponent(IUserRepository userRepository,
            IUserService userService,
            IAccountSettingsRepository accountSettingsRepository,
            IHttpContextAccessor httpContextAccessor,
            IPageContextRepository pageContextRepository)
        {
            _userRepository = userRepository;
            _userService = userService;
            _accountSettingsRepository = accountSettingsRepository;
            _httpContextAccessor = httpContextAccessor;
            _pageContextRepository = pageContextRepository;
        }

        /// <summary>
        /// Uses the current page context to render meta data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get values from Query String
            Maybe<Guid> userId = Maybe.None;
            Maybe<string> token = Maybe.None;
            if (_httpContextAccessor.HttpContext.AsMaybe().TryGetValue(out var httpContext))
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

            bool isEditMode = await _pageContextRepository.IsEditModeAsync();
            ConfirmationViewModel model;

            try
            {
                if (!userId.HasValue)
                {
                    throw new InvalidOperationException("No user Identity Provided");
                }
                // Convert Guid to ID
                var userResult = await _userRepository.GetUserAsync(userId.Value);

                if (userResult.IsFailure)
                {
                    throw new InvalidOperationException(userResult.Error);
                }
                // Verifies the confirmation parameters and enables the user account if successful
                model = new ConfirmationViewModel(
                    result: await _userService.ConfirmRegistrationConfirmationTokenAsync(userResult.Value, token.GetValueOrDefault(string.Empty)),
                    isEditMode: isEditMode
                );

                if (model.Result.Succeeded)
                {
                    model.LoginUrl = await _accountSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
                }
            }
            catch (InvalidOperationException ex)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                model = new ConfirmationViewModel(
                    result: IdentityResult.Failed(new IdentityError() { Description = ex.Message }),
                    isEditMode: isEditMode
                    );
            }

            return View("/Features/Account/Confirmation/Confirmation.cshtml", model);
        }

    }
}
