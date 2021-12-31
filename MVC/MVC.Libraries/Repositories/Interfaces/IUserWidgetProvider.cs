using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IUserWidgetProvider : IRepository
    {
        Task<string[]> GetUserAllowedWidgetsAsync(string[] ZoneWidgets = null, bool AddZoneWidgets = false);
    }
}
