using CMS.Membership;
using Kentico.Membership;
using MVCCaching;
using System;
using System.Linq;
using Generic.Repositories.Interfaces;
using CMS.Base;
using Generic.Library.Helpers;

namespace Generic.Repositories.Implementations
{
    public class KenticoUserRepository : IUserRepository
    {
        private IUserInfoProvider _UserInfoProvider;
        private ApplicationUserManager<ApplicationUser> _UserManager;

        public KenticoUserRepository(IUserInfoProvider UserInfoProvider,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _UserInfoProvider = UserInfoProvider;
            _UserManager = userManager;
        }

        //[CacheDependency("cms.user|byname|{0}")]
        public IUserInfo GetUserByUsername(string Username, string[] Columns = null)
        {
            return _UserInfoProvider.Get()
                .WhereEquals("Username", Username)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        //[CacheDependency("cms.user|byname|{0}")]
        public IUserInfo GetUserByEmail(string Email, string[] Columns = null)
        {
            return _UserInfoProvider.Get()
                .WhereEquals("Email", Email)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        //[CacheDependency("cms.user|byid|{0}")]
        public IUserInfo GetUserByID(int UserID, string[] Columns = null)
        {
            return _UserInfoProvider.Get()
                .WhereEquals("UserID", UserID)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        //[CacheDependency("cms.user|byguid|{0}")]
        public IUserInfo GetUserByGuid(Guid UserGuid)
        {
            return _UserInfoProvider.Get()
                .WhereEquals("UserGuid", UserGuid)
                .Columns("UserID")
                .FirstOrDefault();
        }

        //[CacheDependency("cms.user|byguid|{0}")]
        public int? UserGuidToID(Guid UserGuid)
        {
            return _UserInfoProvider.Get()
                .WhereEquals("UserGuid", UserGuid)
                .Columns("UserID")
                .FirstOrDefault()
                ?.UserID;
        }

    }
}