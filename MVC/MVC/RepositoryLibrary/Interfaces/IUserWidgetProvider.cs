using MVCCaching;
using System.Threading.Tasks;

namespace Generic.Repositories.Interfaces
{
    public interface IUserWidgetProvider : IRepository
    {
        string[] GetUserAllowedWidgets(string[] ZoneWidgets = null, bool AddZoneWidgets = false);

        Task<string[]> GetUserAllowedWidgetsAsync(string[] ZoneWidgets = null, bool AddZoneWidgets = false);
    }
}
