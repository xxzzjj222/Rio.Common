using Rio.Extensions;

namespace Rio.Common.Logging;

public interface ILogHelperLoggingBuilder
{
    bool AddProvider(ILogHelperProvider provider);

    bool AddEnricher(ILogHelperLoggingEnricher enricher);

    bool AddFilter(Func<Type, LogHelperLoggingEvent, bool> filterFunc);

    ILogHelperFactory Build();
}

internal class LogHelperLoggingBuilder : ILogHelperLoggingBuilder
{
    internal readonly Dictionary<Type, ILogHelperProvider> _logHelperProviders = new();
    internal readonly List<ILogHelperLoggingEnricher> _logHelperEnrichers = new();
    internal readonly List<Func<Type, LogHelperLoggingEvent, bool>> _logFilters = new();
    
    public bool AddEnricher(ILogHelperLoggingEnricher enricher)
    {
        Guard.NotNull(enricher, nameof(enricher));
        _logHelperEnrichers.Add(enricher);
        return true;
    }

    public bool AddFilter(Func<Type, LogHelperLoggingEvent, bool> filterFunc)
    {
        Guard.NotNull(filterFunc, nameof(filterFunc));

        _logFilters.Add(filterFunc);
        return true;
    }

    public bool AddProvider(ILogHelperProvider provider)
    {
        Guard.NotNull(provider, nameof(provider));

        return _logHelperProviders.AddIfNotContainsKey(provider.GetType(), provider);
    }

    public ILogHelperFactory Build() => new LogHelperFactory(_logHelperProviders, _logHelperEnrichers, _logFilters);
}

