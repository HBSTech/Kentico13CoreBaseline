using Localization.Repositories;
using Localization.Repositories.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XperienceCommunity.Localizer;

namespace Localization
{
    public static class LocalizationStartupMiddleware
    {
        public static IServiceCollection UseLocalization(this IServiceCollection services, LocalizationConfiguration? localizationConfiguration)
        {
            var configuration = localizationConfiguration ?? new LocalizationConfiguration("en-US");
            services
                .AddScoped((serviceProvider) => configuration)
                .AddScoped<ILocalizedCategoryCachedRepository, LocalizedCategoryCachedRepository>()
                .AddLocalization()
                .AddXperienceLocalizer()
                .AddControllersWithViews()
                .AddViewLocalization() // honestly couldn't get View Localization to ever work...
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        // This will use your ~/Resources/SharedResources.resx, with kentico fall back
                        return factory.Create(typeof(SharedResources));
                    };
                });

            return services;
        }
    }
}
