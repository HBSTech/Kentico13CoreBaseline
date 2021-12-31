﻿using System;
using System.Net;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Generic.Library
{
    /// <summary>
    /// Adds a rewriter with <see cref="AdminRedirectRule"/> into the pipeline.
    /// </summary>
    public class AdminRedirectStartupFilter : IStartupFilter
    {
        private readonly IConfiguration configuration;


        public AdminRedirectStartupFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                // Ensures redirect to the administration instance based on URL defined in settings
                builder.UseRewriter(new RewriteOptions()
                    .Add(new AdminRedirectRule(configuration)));

                next(builder);
            };
        }


        /// <summary>
        /// Redirects a request to "/admin" to the administration site specified in <c>DancingGoatAdminUrl</c> app setting.
        /// </summary>
        private class AdminRedirectRule : IRule
        {
            private readonly string adminUrl;

            public AdminRedirectRule(IConfiguration configuration)
            {
                adminUrl = configuration["CustomAdminUrl"] ?? String.Empty;
            }

            public void ApplyRule(RewriteContext context)
            {
                if (string.IsNullOrEmpty(adminUrl))
                {
                    return;
                }

                var request = context.HttpContext.Request;

                if (request.Path.Value.TrimEnd('/').Equals("/admin", StringComparison.OrdinalIgnoreCase))
                {
                    var response = context.HttpContext.Response;

                    response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                    response.Headers[HeaderNames.Location] = adminUrl;
                    context.Result = RuleResult.EndResponse;
                }
            }
        }
    }
}
