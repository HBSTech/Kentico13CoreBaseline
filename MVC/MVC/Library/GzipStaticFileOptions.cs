using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Generic.Library
{
    public class GzipStaticFileOptions : IPostConfigureOptions<StaticFileOptions>
    {
        public void PostConfigure(string name, StaticFileOptions options)
        {
            options.OnPrepareResponse = context =>
            {
                IHeaderDictionary headers = context.Context.Response.Headers;
                string contentType = headers["Content-Type"];
                if (contentType == "application/x-gzip")
                {
                    if (context.File.Name.EndsWith("js.gz"))
                    {
                        contentType = "application/javascript";
                        headers.Add("Content-Encoding", "gzip");
                        headers["Content-Type"] = contentType;
                    }
                    else if (context.File.Name.EndsWith("css.gz"))
                    {
                        contentType = "text/css";
                        headers.Add("Content-Encoding", "gzip");
                        headers["Content-Type"] = contentType;
                    }
                }
            };
        }
    }
}
