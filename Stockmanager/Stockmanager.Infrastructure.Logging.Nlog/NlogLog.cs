using System;
using System.Diagnostics;
using NLog;

namespace Stockmanager.Infrastructure.Logging.Nlog
{
    internal class NlogLog : ILog
    {
        private readonly ILogger _logger;

        public NlogLog(ILogger logger)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Log(LogLevel.Debug, message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.Log(LogLevel.Info, message, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            _logger.Log(LogLevel.Warn, message, args);
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            _logger.Log(LogLevel.Error, message, exception, args);
        }

        public void LogFatal(string message, Exception exception, params object[] args)
        {
            _logger.Log(LogLevel.Fatal, message, exception, args);
        }
    }
}
