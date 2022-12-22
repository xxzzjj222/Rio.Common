
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rio.Extensions;
public static class ExceptionExtension
{
    public static Exception Unwrap(this Exception ex, int depth = 16)
    {
        var exception = ex;
        while (exception is AggregateException
            && exception.InnerException != null
            && depth-- > 0)
        {
            exception = exception.InnerException;
        }
        return exception;
    }

    public static bool IsFatal(this Exception exception)
    {
        while (exception != null)
        {
            if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException))
                || (exception is ThreadAbortException)
                || (exception is AccessViolationException)
                || (exception is SEHException)
                || (exception is StackOverflowException)
                || (exception is TypeInitializationException))
            {
                return true;
            }

            if (exception is TargetInvocationException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            else if (exception is AggregateException aex)
            {
                exception = aex.Unwrap(64);
            }
            else
            {
                break;
            }
        }

        return false;
    }
}
