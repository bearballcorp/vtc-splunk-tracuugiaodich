using Microsoft.Extensions.Logging;
using System;

namespace Vtc.Splunk.Log.Analyzer.WinForms.Core
{
    public class NullLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => default!;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
    }

    public class NullLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider) { }
        public ILogger CreateLogger(string categoryName) => new NullLogger<object>();
        public void Dispose() { }
    }
}