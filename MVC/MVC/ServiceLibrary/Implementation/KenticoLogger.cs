using CMS.Base;
using CMS.Core;
using Generic.Services.Interfaces;
using System;

namespace Generic.Services.Implementation
{
    public class KenticoLogger : ILogger
    {
        private ISiteService _SiteRepo;
        private IEventLogService _LogService;

        public KenticoLogger(ISiteService SiteRepo, IEventLogService LogService)
        {
            _SiteRepo = SiteRepo;
            _LogService = LogService;
        }
        public void LogException(Exception ex, string Source, string EventCode, string Description = "")
        {
            _LogService.LogException(Source, EventCode, ex, additionalMessage: Description);
        }

        public void LogInformation(string Source, string EventCode, string Description = "")
        {
            _LogService.LogInformation(Source, EventCode, Description);
        }

        public void LogWarning(Exception ex, string Source, string EventCode, string Description = "")
        {
            _LogService.LogWarning(Source, EventCode, ex, _SiteRepo.CurrentSite.SiteID, Description);
        }
    }
}