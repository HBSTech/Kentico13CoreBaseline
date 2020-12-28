using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Services.Interfaces
{
    public interface ILogger : IService
    {
        void LogException(Exception ex, string Source, string EventCode, string Description = "");

        void LogWarning(Exception ex, string Source, string EventCode, string Description = "");

        void LogInformation(string Source, string EventCode, string Description = "");

    }
}