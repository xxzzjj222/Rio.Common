
using System.Reflection;

namespace Rio.Common.Helpers;

public static class ReflectHelper
{
    public static Assembly[] GetAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }
}
