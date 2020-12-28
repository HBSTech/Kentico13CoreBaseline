using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Generic.Models;
namespace Generic
{
    [ViewComponent(Name = "ShareableContent")]
    public class ShareableContentComponent : ViewComponent
    {
        public ShareableContentComponent(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new ShareableContentComponentViewModel() { EditMode = HttpContextAccessor.HttpContext.Kentico().PageBuilder().EditMode });
        }
    }
}
