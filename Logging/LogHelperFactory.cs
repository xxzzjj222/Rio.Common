﻿using System.Collections.Concurrent;

namespace Rio.Common.Logging;

public interface ILogHelperFactory:IDisposable
{
    ILogHelperLogger CreateLogger(string categoryName);
}

internal sealed class NullLogHelperFactory: ILogHelperFactory
{
    public static readonly ILogHelperFactory Instance = new NullLogHelperFactory();

    private NullLogHelperFactory()
    {
    }

    public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;

    public void Dispose()
    {
        //nothing to dispose
    }
}

internal sealed class LogHelperFactory:ILogHelperFactory
{
    internal readonly IReadOnlyDictionary<Type, ILogHelperProvider> _logHelperProviders;
    internal readonly IReadOnlyCollection<ILogHelperLoggingEnricher> _logHelperEnrichers;
    internal readonly IReadOnlyCollection<Func<Type, LogHelperLoggingEvent, bool>> _logFilters;

    private readonly ConcurrentDictionary<string, ILogHelperLogger> _loggers = new();

    public LogHelperFactory(IReadOnlyDictionary<Type, ILogHelperProvider> logHelperProviders,
       IReadOnlyCollection<ILogHelperLoggingEnricher> logHelperEnrichers,
       IReadOnlyCollection<Func<Type, LogHelperLoggingEvent, bool>> logFilters
       )
    {
        _logHelperProviders = logHelperProviders;
        _logHelperEnrichers = logHelperEnrichers;
        _logFilters = logFilters;
    }

    public ILogHelperLogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, _ => new LogHelper(this, _));

    public void Dispose()
    {
        if (_logHelperProviders.Count == 0)
            return;

        foreach(var provider in _logHelperProviders.Values)
        {
            if(provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

