using Rio.Common.Logging;

namespace Rio.Common.Helpers;

public static class InvokeHelper
{
    static InvokeHelper()
    {
        OnInvokeException = ex => LogHelper.GetLogger(typeof(InvokeHelper)).Error(ex);
    }

    public static Action<Exception>? OnInvokeException { get; set; }
}
