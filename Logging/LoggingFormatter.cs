using System.Collections;
using System.Globalization;
using System.Text;

namespace Rio.Common.Logging;

internal struct FormatterLogValue
{
    public string Msg { get; set; }

    public Dictionary<string,object?>? Values { get; set; }

    public FormatterLogValue(string msg,Dictionary<string,object?>? values)
    {
        Msg = msg;
        Values = values;
    }
}

internal static class LoggingFormatter
{
    public static FormatterLogValue Format(string msgTemplate,object?[]? values)
    {
        if(values==null || values.Length==0)
            return new FormatterLogValue(msgTemplate,null);

        var formatter = new LogValuesFormatter(msgTemplate);
        var msg=formatter.FormatWithValues(values);
        var dic=formatter.GetValues(values)
            .ToDictionary(x=>x.Key,x=>x.Value);

        return new FormatterLogValue(msg, dic);
    }

    private class LogValuesFormatter
    {
        private const string NullValue = "(null)";
        private static readonly char[] _formatDelimiters = { ';' };
        private readonly string _format;
        private readonly List<string> _valueNames = new();

        public LogValuesFormatter(string format)
        {
            OriginalFormat = format;

            var sb = new StringBuilder();
            var scanIndex = 0;
            var endIndex = format.Length;

            while(scanIndex < endIndex)
            {
                var openBraceIndex=FindBraceIndex(format,'{',scanIndex,endIndex);
                var closeBraceIndex=FindBraceIndex(format,'}',openBraceIndex,endIndex);

                if(openBraceIndex==closeBraceIndex)
                {
                    sb.Append(format,scanIndex,endIndex-scanIndex);
                    scanIndex = endIndex;
                }
                else
                {
                    var formatDelimiterIndex=FindIndexOfAny(format,_formatDelimiters,openBraceIndex,closeBraceIndex);

                    sb.Append(format, scanIndex, openBraceIndex - scanIndex + 1);
                    sb.Append(_valueNames.Count.ToString(CultureInfo.InvariantCulture));
                    _valueNames.Add(format.Substring(openBraceIndex + 1, formatDelimiterIndex - openBraceIndex - 1));
                    sb.Append(format, formatDelimiterIndex, closeBraceIndex - formatDelimiterIndex + 1);

                    scanIndex = closeBraceIndex + 1;
                }
            }

            _format=sb.ToString();
        }

        private string OriginalFormat { get; }

        private static int FindBraceIndex(string format,char brace,int startIndex,int endIndex)
        {
            var braceIndex = endIndex;
            var scanIndex = startIndex;
            var braceOccurrenceCount = 0;

            while (scanIndex < endIndex)
            {
                if (braceOccurrenceCount > 0 && format[scanIndex] != brace)
                {
                    if (braceOccurrenceCount % 2 == 0)
                    {
                        braceOccurrenceCount = 0;
                        braceIndex = endIndex;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (format[scanIndex] == brace)
                {
                    if (brace == '}')
                    {
                        if (braceOccurrenceCount == 0)
                        {
                            braceIndex = scanIndex;
                        }
                    }
                    else
                    {
                        braceIndex = scanIndex;
                    }

                    braceOccurrenceCount++;
                }
                scanIndex++;
            }

            return braceIndex;
        }

        private static int FindIndexOfAny(string format,char[] chars,int startIndex,int endIndex)
        {
            var findIndex = format.IndexOfAny(chars, startIndex, endIndex - startIndex);
            return findIndex == -1 ? endIndex : findIndex;
        }

        public string FormatWithValues(object?[]? values)
        {
            if(values!=null)
            {
                for(var i=0;i<values.Length;i++)
                {
                    values[i] = FormatArgument(values[i]);
                }
            }

            return string.Format(CultureInfo.InvariantCulture, _format, values ?? Array.Empty<object>());
        }

        public IEnumerable<KeyValuePair<string,object?>> GetValues(object?[] values)
        {
            var valueArray = new KeyValuePair<string, object?>[values.Length + 1];
            for(var index=0;index!=_valueNames.Count;++index)
            {
                valueArray[index]=new KeyValuePair<string, object?>(_valueNames[index], values[index]);
            }

            valueArray[valueArray.Length - 1] = new KeyValuePair<string, object?>("{OriginalFormat}", OriginalFormat);
            return valueArray;
        }

        private static object FormatArgument(object? value)
        {
            if(value==null)
            {
                return NullValue;
            }

            if(value is string)
            {
                return value;
            }

            if(value is IEnumerable enumerable)
            {
                return string.Join(", ", enumerable.Cast<object>().Select(o => o ?? NullValue));
            }

            return value;
        }
    }
}

