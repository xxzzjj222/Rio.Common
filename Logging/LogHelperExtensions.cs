namespace Rio.Common.Logging;

public static class LogHelperExtensions
{
    public static void Log(this ILogHelperLogger logger, LogHelperLogLevel loggerLevel, string? msg) => logger.Log(loggerLevel, null, msg);

    #region Info

    public static void Info(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Info, null, msg, parameters);
    }

    public static void Info(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Info, ex, msg);

    public static void Info(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Info, ex, ex?.Message);

    #endregion Info

    #region Trace

    public static void Trace(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Trace, null, msg, parameters);
    }

    public static void Trace(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Trace, ex, msg);

    public static void Trace(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Trace, ex, ex?.Message);

    #endregion Trace

    #region Debug

    public static void Debug(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Debug, null, msg, parameters);
    }

    public static void Debug(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Debug, ex, msg);

    public static void Debug(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Debug, ex, ex?.Message);

    #endregion Debug

    #region Warn

    public static void Warn(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Warn, null, msg, parameters);
    }

    public static void Warn(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Warn, ex, msg);

    public static void Warn(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Warn, ex, ex?.Message);

    #endregion Warn

    #region Error

    public static void Error(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Error, null, msg, parameters);
    }

    public static void Error(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Error, ex, msg);

    public static void Error(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Error, ex, ex?.Message);

    #endregion Error

    #region Fatal

    public static void Fatal(this ILogHelperLogger logger, string? msg, params object[] parameters)
    {
        logger.Log(LogHelperLogLevel.Fatal, null, msg, parameters);
    }

    public static void Fatal(this ILogHelperLogger logger, Exception? ex, string? msg) => logger.Log(LogHelperLogLevel.Fatal, ex, msg);

    public static void Fatal(this ILogHelperLogger logger, Exception? ex) => logger.Log(LogHelperLogLevel.Fatal, ex, ex?.Message);

    #endregion Fatal

    #region ILogHelperFactory

    public static ILogHelperLogger GetLogger<T>(this ILogHelperFactory logHelperFactory) =>
        GetLogger(logHelperFactory, typeof(T));

    public static ILogHelperLogger GetLogger(this ILogHelperFactory logHelperFactory, Type type)
    {
        Guard.NotNull(logHelperFactory, nameof(logHelperFactory));

        return logHelperFactory.CreateLogger(type.FullName ?? type.Name);
    }

    #endregion ILogHelperFactory

    #region LoggingEnricher

    public static void AddProperty(this LogHelperLoggingEvent loggingEvent,string propertyName,object propertyValue, bool overwrite)
    {
        Guard.NotNull(loggingEvent, nameof(loggingEvent));

        loggingEvent.Properties ??= new Dictionary<string, object?>();

        if(loggingEvent.Properties.ContainsKey(propertyName) && !overwrite)
        {
            return;
        }

        loggingEvent.Properties[propertyName] = propertyValue;
    }

    public static void AddProperty(this LogHelperLoggingEvent loggingEvent, string propertyName, Func<LogHelperLoggingEvent, object> propertyValueFactory, bool overwrite = false)
    {
        Guard.NotNull(loggingEvent, nameof(loggingEvent));
        Guard.NotNull(propertyValueFactory, nameof(propertyValueFactory));

        loggingEvent.Properties ??= new Dictionary<string, object?>();

        if (loggingEvent.Properties.ContainsKey(propertyName) && !overwrite)
        {
            return;
        }

        loggingEvent.Properties[propertyName]=propertyValueFactory.Invoke(loggingEvent);
    }

    #endregion LoggingEnricher
}

