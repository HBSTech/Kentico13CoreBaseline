using Core.Repositories;
using Core.Repositories.Implementation;
using Core.Services;
using Core.Services.Implementations;
using Generic.Repositories.Implementation;
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
    }
}
