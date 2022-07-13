using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Rio.Extensions;

public static class ConfigurationExtension
{
    #region Placeholder

    private static readonly Regex ConfigPlaceholderRegex = new(@"\$\(([A-Za-z0-9:_]+?)\)");

    public static IConfiguration ReplacePlaceholders(this IConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    #endregion
}

