using CMS.Base;
using CMS.Core;
using MVCCaching;

namespace Core.Services.Implementation
{
    [AutoDependencyInjection]
    public class Logger : ILogger
    {
        private ISiteService _siteRepo;
        private IEventLogService _LogService;

        public Logger(ISiteService SiteRepo, IEventLogService LogService)
        {
            _siteRepo = SiteRepo;
            _LogService = LogService;
        }
        public void LogException(Exception ex, string Source, string EventCode, string Description = "")
        {
            _LogService.LogException(Source, EventCode, ex, additionalMessage: Description);
        }

        public void LogException(string Source, string EventCode, string Description = "")
        {
            _LogService.LogEvent(EventTypeEnum.Error, Source, EventCode, eventDescription: Description);
        }

        public void LogInformation(string Source, string EventCode, string Description = "")
        {
            _LogService.LogInformation(Source, EventCode, Description);
        }

        public void LogWarning(Exception ex, string Source, string EventCode, string Description = "")
        {
            _LogService.LogWarning(Source, EventCode, ex, _siteRepo.CurrentSite.SiteID, Description);
        }
    }
}