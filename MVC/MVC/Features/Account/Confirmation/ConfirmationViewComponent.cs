using Generic.Features.Account.LogIn;
using Generic.Repositories.Interfaces;
using Generic.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Features.Account.Confirmation
{
    [ViewComponent]
    public class ConfirmationViewComponent : ViewComponent
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly ISiteSettingsRepository _siteSettingsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPageContextRepository _pageContextRepository;

        public ConfirmationViewComponent(IUserRepository userRepository,
            IUserService userService,
            ISiteSettingsRepository siteSettingsRepository,
            IHttpContextAccessor httpContextAccessor,
            IPageContextRepository pageContextRepository)
        {
            _userRepository = userRepository;
            _userService = userService;
            _siteSettingsRepository = siteSettingsRepository;
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
            Guid? userId = null;
            string token = null;
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("userId", out StringValues queryUserID) && queryUserID.Any())
            {
                if (Guid.TryParse(queryUserID, out Guid userIdTemp))
                {
                    userId = userIdTemp;
                }
            }
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("token", out StringValues queryToken) && queryToken.Any())
            {
                token = queryToken.FirstOrDefault();
            }

            var model = new ConfirmationViewModel()
            {
                IsEditMode = await _pageContextRepository.IsEditModeAsync()
            };

            try
            {
                if (!userId.HasValue)
                {
                    throw new InvalidOperationException("No user Identity Provided");
                }
                // Convert Guid to ID
                var user = await _userRepository.GetUserAsync(userId.Value);

                if (user == null)
                {
                    throw new InvalidOperationException("No user found.");
                }
                // Verifies the confirmation parameters and enables the user account if successful
                model.Result = await _userService.ConfirmRegistrationConfirmationTokenAsync(user, token);

                if (model.Result.Succeeded)
                {
                    model.LoginUrl = await _siteSettingsRepository.GetAccountLoginUrlAsync(LogInController.GetUrl());
                }
            }
            catch (InvalidOperationException ex)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                model.Result = IdentityResult.Failed(new IdentityError() {  Description = ex.Message });
            }

            return View("~/Features/Account/Confirmation/Confirmation.cshtml", model);
        }

    }
}
