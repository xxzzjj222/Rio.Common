using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rio.Common
{
    public static class Guard
    {
        public static T NotNull<T>([NotNull] T? t,[CallerArgumentExpression("t")]string? paramName=default)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(t, paramName);
#else
            if(t is null)
            {
                throw new ArgumentNullException(paramName);
            }
#endif
            return t;
        }

        public static string NotNullOrEmpty([NotNull] string? str,
        [CallerArgumentExpression("str")]
            string? paramName = null)
        {
            NotNull(str, paramName);
            if (str.Length == 0)
            {
                throw new ArgumentException("The argument can not be Empty", paramName);
            }
            return str;
        }
    }
}
