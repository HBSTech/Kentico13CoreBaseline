using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using Generic.Components.Widgets.ImageWidget;
using Generic.Components.Widgets.RichTextWidget;
using Generic.Components.Widgets.ShareableContentWidget;
using Generic.Components.Widgets.StaticTextContainerizedWidget;
using Generic.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class UserWidgetProvider : IUserWidgetProvider
    {

        public UserWidgetProvider(IUserInfoProvider userInfoProvider,
            IHttpContextAccessor httpContextAccessor,
            IProgressiveCache progressiveCache)
        {
            _userInfoProvider = userInfoProvider;
            _httpContextAccessor = httpContextAccessor;
            _progressiveCache = progressiveCache;
        }

        public const string KenticoFormWidget_IDENTIFIER = "Kentico.FormWidget";
        public const string KenticoRichTextWidget_IDENTIFIER = "Kentico.Widget.RichText";
        private readonly IUserInfoProvider _userInfoProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProgressiveCache _progressiveCache;

        /// <summary>
        /// Listing of all widgets considered "Generic" and as a baseline for what should be allowed
        /// </summary>
        /// <returns></returns>
        private static string[] GetDefaultWidgets()
        {
            return new string[]
             {
                KenticoFormWidget_IDENTIFIER,
                KenticoRichTextWidget_IDENTIFIER,
                RichTextWidgetProperties.IDENTIFIER,
                StaticTextContainerizedWidgetProperties.IDENTIFIER,
                ImageWidgetViewComponent.IDENTIFIER,
                ShareableContentWidgetViewComponent.IDENTITY
             };
        }

        /// <summary>
        /// Gets the current user's allowed widgets
        /// </summary>
        /// <param name="ZoneWidgets">If provided, only widgets in this list will be allowed of all allowable.</param>
        /// <param name="AddZoneWidgets">If true, then the zone widgets will be added to the user's default widgets.  False by default means only the ZoneWidgets the user has access to will be allowed</param>
        /// <returns></returns>
        public async Task<string[]> GetUserAllowedWidgetsAsync(string[] ZoneWidgets = null, bool AddZoneWidgets = false)
        {
            string userName = _httpContextAccessor.HttpContext.User?.Identity?.Name ?? "public";
            var user = await _progressiveCache.LoadAsync(async cs =>
            {
                var user = await _userInfoProvider.GetAsync(_httpContextAccessor.HttpContext.User?.Identity?.Name ?? "public");
                user ??= await _userInfoProvider.GetAsync("Public");
                return user;
            }, new CacheSettings(30, "UserWidgetProviderGetUser", userName));
            List<string> Widgets = new List<string>();
            if (ZoneWidgets == null || ZoneWidgets.Length == 0 || AddZoneWidgets)
            {
                Widgets.AddRange(GetDefaultWidgets());
            }
            if (user != null)
            {
                // Add or remove widgets here based on permission
                switch (user.SiteIndependentPrivilegeLevel)
                {
                    case UserPrivilegeLevelEnum.GlobalAdmin:
                    case UserPrivilegeLevelEnum.Admin:
                        Widgets.Add(PartialWidgetPage.PartialWidgetPageWidgetModel.IDENTITY);
                        break;
                    case UserPrivilegeLevelEnum.Editor:
                        break;
                }
            }


            // If zone widgets provided
            if (ZoneWidgets != null && ZoneWidgets.Length > 0)
            {
                if (AddZoneWidgets)
                {
                    return Widgets.Union(ZoneWidgets).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
                }
                else
                {
                    return ZoneWidgets.Intersect(Widgets, StringComparer.InvariantCultureIgnoreCase).ToArray();
                }
            }
            else
            {
                return Widgets.ToArray();
            }
        }

    }
}
