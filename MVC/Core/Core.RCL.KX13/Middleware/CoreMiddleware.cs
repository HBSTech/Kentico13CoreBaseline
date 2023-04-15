using Core.Repositories;
using Core.Repositories.Implementation;
using Core.Services;
using Core.Services.Implementations;
using Generic.Repositories.Implementation;
using Kentico.Membership;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Logger = Core.Services.Implementation.Logger;

namespace Core
{
    public static class CoreMiddleware
    {
        public static IServiceCollection UseCoreBaseline(this IServiceCollection services)
        {
            services.AddScoped<IBaselinePageBuilderContext, BaselinePageBuilderContext>()
                .AddScoped<ICategoryCachedRepository, CategoryCachedRepository>()
                .AddScoped<IMediaRepository, MediaRepository>()
                .AddScoped<IPageCategoryRepository, PageCategoryRepository>()
                .AddScoped<IPageContextRepository, PageContextRepository>()
                .AddScoped<ISiteRepository, SiteRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IIdentityService, IdentityService>()
                .AddScoped<ILogger, Logger>()
                .AddScoped<IPageIdentityFactory, PageIdentityFactory>()
                .AddScoped<IUrlResolver, UrlResolver>();
            return services;

        }

        public static IServiceCollection AddCoreBaselineKenticoAuthentication(this IServiceCollection services, IConfiguration configuration, string AUTHENTICATION_COOKIE_NAME = "identity.authentication")
        {
            // Required for authentication
            services.AddScoped<IPasswordHasher<ApplicationUser>, Kentico.Membership.PasswordHasher<ApplicationUser>>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddApplicationIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Note: These settings are effective only when password policies are turned off in the administration settings.
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddApplicationDefaultTokenProviders()
            .AddUserStore<ApplicationUserStore<ApplicationUser>>()
            .AddRoleStore<ApplicationRoleStore<ApplicationRole>>()
            .AddUserManager<ApplicationUserManager<ApplicationUser>>()
            .AddSignInManager<SignInManager<ApplicationUser>>();

            return services;
        }
    }
}
