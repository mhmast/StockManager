using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Jarvis.Infrastructure.Logging.Nlog
{
    internal class NlogLog : ILog, IDisposable
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly object _lockObj = new object();
        private readonly Queue<LogData> _logQueue = new Queue<LogData>();

        public NlogLog(ILogger logger)
        {
            var token = _tokenSource.Token;
            RunBackgroundWorker(logger, token);
        }

        private void RunBackgroundWorker(ILogger logger, CancellationToken token)
        {
            var mapping = new Dictionary<LogLevel, Action<LogData>>
            {
                { LogLevel.Debug, d=>logger.Debug(d.Exception,d.Message,d.Args)},
                { LogLevel.Info, d=>logger.Info(d.Exception,d.Message,d.Args)},
                { LogLevel.Warn, d=>logger.Warn(d.Exception,d.Message,d.Args)},
                { LogLevel.Trace, d=>logger.Trace(d.Exception,d.Message,d.Args)},
                { LogLevel.Error, d=>logger.Error(d.Exception,d.Message,d.Args)},
                { LogLevel.Fatal, d=>logger.Fatal(d.Exception,d.Message,d.Args)}
            };
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    var data = Dequeue();
                    mapping[data.LogLevel](data);
                }
            }, token);
        }

        private LogData Dequeue()
        {
            while (true)
            {
                lock (_lockObj)
                {
                    if (_logQueue.Count > 0)
                    {
                        return _logQueue.Dequeue();
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void Queue(LogData data)
        {
            lock (_lockObj)
            {
                _logQueue.Enqueue(data);
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            Queue(new LogData(LogLevel.Debug, message, null, args));
        }

        public void LogInfo(string message, params object[] args)
        {
            Queue(new LogData(LogLevel.Info, message, null, args));
        }

        public void LogWarn(string message, params object[] args)
        {
            Queue(new LogData(LogLevel.Warn, message, null, args));
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            Queue(new LogData(LogLevel.Error, message, exception, args));
        }

        public void LogFatal(string message, Exception exception, params object[] args)
        {
            Queue(new LogData(LogLevel.Fatal, message, exception, args));
        }

        private class LogData
        {
            public LogData(LogLevel logLevel, string message, Exception exception = null, params object[] args)
            {
                LogLevel = logLevel;
                Message = message;
                Args = args;
                Exception = exception;
            }

            public LogLevel LogLevel { get; }
            public string Message { get; }
            public Exception Exception { get; }
            public object[] Args { get; }

        }

        public void Dispose()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }
    }
}
