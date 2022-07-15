using Microsoft.Extensions.Configuration;
using Rio.Common;
using System.Text.RegularExpressions;

namespace Rio.Extensions;

public static class ConfigurationExtension
{
    #region Placeholder

    /// <summary>
    /// A regex which matches tokens in the following format: $(Item:Sub1:Sub2).
    /// inspired by https://github.com/henkmollema/ConfigurationPlaceholders
    /// </summary>
    private static readonly Regex ConfigPlaceholderRegex = new(@"\$\(([A-Za-z0-9:_]+?)\)");

    /// <summary>
    /// Replaces the placeholders in the specified <see cref="IConfiguration" />instance.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/>instance.</param>
    /// <returns>The given <see cref="IConfiguration"/>instance.</returns>
    /// <exception cref="InvalidConfigurationPlaceholderException"></exception>
    public static IConfiguration ReplacePlaceholders(this IConfiguration configuration)
    {
        foreach (var kvp in configuration.AsEnumerable())
        {
            if(string.IsNullOrEmpty(kvp.Value))
            {
                //Skip empty configuration values.
                continue;
            }

            //Replace placeholders in the configuration value.
            var result = ConfigPlaceholderRegex.Replace(kvp.Value, match =>
            {
                if (!match.Success)
                {
                    //Return the original value
                    return kvp.Value;
                }

                if (match.Groups.Count != 2)
                {
                    //There is a match, but somehow no group for the placeholder.
                    throw new InvalidConfigurationPlaceholderException(match.ToString());
                }

                var placeholder = match.Groups[1].Value;
                if (placeholder.StartsWith(":")|| placeholder.EndsWith(":"))
                {
                    //Placeholder can't start or end with a colon.
                    throw new InvalidConfigurationPlaceholderException(placeholder);
                }

                //Return the value in the configuration instance.
                return configuration[placeholder];
            });

            //Replace the value int the configuration instance.
            configuration[kvp.Key] = result;
        }

        return configuration;
    }

    private sealed class InvalidConfigurationPlaceholderException:InvalidOperationException
    {
        public InvalidConfigurationPlaceholderException(string placeholder) : base($"Invalid configuration placeholder: '{placeholder}'") { }
    }

    #endregion

    #region GetAppSetting
    
    /// <summary>
    /// GetRequiredAppSetting
    /// Shorthand for GetSection("AppSetting")[key]
    /// </summary>
    /// <param name="configuration">IConfiguration instance.</param>
    /// <param name="key">appsetting key.</param>
    /// <returns>appsetting value.</returns>
    public static string GetReqiuredAppSetting(this IConfiguration configuration,string key)
    {
        var value = configuration.GetSection("AppSetting")?[key];
        return Guard.NotNull(value, nameof(key));
    }
    
    #endregion
}

