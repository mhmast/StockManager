using System;

namespace Stockmanager.Infrastructure.Logging
{
    public interface ILog
    {
        void LogDebug(string message, params object[] args);
        void LogInfo(string message, params object[] args);
        void LogWarn(string message, params object[] args);
        void LogError(string message, Exception exception,params object[] args);
        void LogFatal(string message, Exception exception,params object[] args);
    }
}
