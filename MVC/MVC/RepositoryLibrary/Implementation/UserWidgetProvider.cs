
using CMS.Base;
using CMS.Membership;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.Widgets;
using Kentico.Membership;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Repositories.Implementations
{
    public class UserWidgetProvider : IUserWidgetProvider

    {

        public UserWidgetProvider(IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            UserRepository = userRepository;
            HttpContextAccessor = httpContextAccessor;
        }

        public const string KenticoFormWidget_IDENTIFIER = "Kentico.FormWidget";
        public const string KenticoRichTextWidget_IDENTIFIER = "Kentico.Widget.RichText";

        public IUserRepository UserRepository { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        /// <summary>
        /// Listing of all widgets considered "Generic" and as a baseline for what should be allowed
        /// </summary>
        /// <returns></returns>
        private string[] GetGenericWidgets()
        {
            return GetGenericWidgetsAsync().Result;
        }

        /// <summary>
        /// Listing of all widgets considered "Generic" and as a baseline for what should be allowed
        /// </summary>
        /// <returns></returns>
        private async Task<string[]> GetGenericWidgetsAsync()
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

        public string[] GetUserAllowedWidgets(string[] ZoneWidgets = null, bool AddZoneWidgets = false)
        {
            return GetUserAllowedWidgetsAsync(ZoneWidgets, AddZoneWidgets).Result;
        }

        /// <summary>
        /// Gets the current user's allowed widgets
        /// </summary>
        /// <param name="ZoneWidgets">If provided, only widgets in this list will be allowed of all allowable.</param>
        /// <param name="AddZoneWidgets">If true, then the zone widgets will be added to the user's default widgets.  False by default means only the ZoneWidgets the user has access to will be allowed</param>
        /// <returns></returns>
        public async Task<string[]> GetUserAllowedWidgetsAsync(string[] ZoneWidgets = null, bool AddZoneWidgets = false)
        {
            var User = UserRepository.GetUserByUsername(HttpContextAccessor.HttpContext.User.Identity.Name);
            List<string> Widgets = new List<string>();
            if (ZoneWidgets == null || ZoneWidgets.Length == 0 || AddZoneWidgets)
            {
                Widgets.AddRange(GetGenericWidgets());
            }

            // Add or remove widgets here based on permission
            if (User != null && User is UserInfo)
            {
                switch (((UserInfo)User).SiteIndependentPrivilegeLevel)
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
