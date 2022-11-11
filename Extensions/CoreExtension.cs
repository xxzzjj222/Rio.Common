using Rio.Common;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Rio.Extensions;

public static class CoreExtension
{
    #region Boolean

    /// <summary>
    /// A bool extension method that execute an Action if the value is true.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    public static void IfTrue(this bool @this, Action? action)
    {
        if (@this)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// A bool extension method that execute an Acition if the value is false.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="action"></param>
    public static void IfFalse(this bool @this, Action? action)
    {
        if (!@this)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// A bool extension method that show the trueValue when @this is true, otherwise show the falseValue.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="trueValue"></param>
    /// <param name="falseValue"></param>
    /// <returns></returns>
    public static string ToString(this bool @this, string trueValue, string falseValue)
    {
        return @this ? trueValue : falseValue;
    }

    #endregion Boolean

    #region Byte

    /// <summary>
    /// Returns the larger of two 8-bit unsigned integers.
    /// </summary>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    public static byte Max(this byte val1, byte val2)
    {
        return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two 8-bit unsigned integers.
    /// </summary>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    public static byte Min(this byte val1, byte val2)
    {
        return Math.Min(val1, val2);
    }

    #endregion Byte

    #region ByteArray

    /// <summary>
    /// Converts an array of 8-bit unsigned integers to its equivalent string representation than is encoded with base-64 digits.
    /// </summary>
    /// <param name="inArray"></param>
    /// <returns></returns>
    public static string ToBase64String(this byte[] inArray)
    {
        return Convert.ToBase64String(inArray);
    }

    public static string ToBase64String(this byte[] inArray, Base64FormattingOptions options)
    {
        return Convert.ToBase64String(inArray, options);
    }

    public static string ToBase64String(this byte[] inArray, int offset, int length)
    {
        return Convert.ToBase64String(inArray, offset, length);
    }

    public static string ToBase64String(this byte[] inArray, int offset, int length, Base64FormattingOptions options)
    {
        return Convert.ToBase64String(inArray, offset, length, options);
    }

    /// <summary>
    /// A byte[] extension method that resizes the byte[].
    /// </summary>
    /// <param name="this"></param>
    /// <param name="newSize"></param>
    /// <returns></returns>
    public static byte[] Resize(this byte[] @this, int newSize)
    {
        Array.Resize(ref @this, newSize);
        return @this;
    }

    /// <summary>
    /// A byte[] extension method that converts @this byteArray to a memory stream.
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public static MemoryStream ToMemoryStream(this byte[] byteArray)
    {
        return new(byteArray);
    }

    public static string GetString(this byte[]? byteArray) => byteArray.HasValue() ? byteArray.GetString(Encoding.UTF8) : string.Empty;

    public static string GetString(this byte[] byteArray, Encoding encoding) => encoding.GetString(byteArray);

    #endregion ByteArray

    #region Char

    /// <summary>
    /// A char extension method that repeats a character the specified number of times.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public static string Repeat(this char @this, int repeatCount)
    {
        return new(@this, repeatCount);
    }

    /// <summary>
    /// Converts the specified numeric unicode character to a double-precision floating point number.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static double GetNumberOfValue(this char c)
    {
        return char.GetNumericValue(c);
    }

    /// <summary>
    /// Categorizes a specified Unicode character into a group identified by one of the values.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static UnicodeCategory GetUnicodeCategory(this char c)
    {
        return char.GetUnicodeCategory(c);
    }

    public static bool IsControl(this char c)
    {
        return char.IsControl(c);
    }

    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }

    public static bool IsLetter(this char c)
    {
        return char.IsLetter(c);
    }

    public static bool IsLetterOrDigit(this char c)
    {
        return char.IsLetterOrDigit(c);
    }

    public static bool IsLower(this char c)
    {
        return char.IsLower(c);
    }

    public static bool IsUpper(this char c)
    {
        return char.IsUpper(c);
    }

    public static bool IsNumer(this char c)
    {
        return char.IsNumber(c);
    }

    public static bool IsSeparator(this char c)
    {
        return char.IsSeparator(c);
    }

    public static bool IsSymbol(this char c)
    {
        return char.IsSymbol(c);
    }

    public static bool IsWhiteSpace(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    public static char ToLower(this char c)
    {
        return char.ToLower(c);
    }

    public static char ToLower(this char c, CultureInfo culture)
    {
        return char.ToLower(c, culture);
    }

    public static char ToLowerInvariant(this char c)
    {
        return char.ToLowerInvariant(c);
    }

    public static char ToUpper(this char c)
    {
        return char.ToUpper(c);
    }

    public static char ToUpper(this char c, CultureInfo culture)
    {
        return char.ToUpper(c, culture);
    }

    public static char ToUpperInvariant(this char c)
    {
        return char.ToUpperInvariant(c);
    }

    #endregion Char

    #region DateTime

    /// <summary>
    /// A DateTime extension method that ages the given this.
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static int Age(this DateTime @this)
    {
        if (DateTime.Today.Month < @this.Month || DateTime.Today.Month == @this.Month && DateTime.Today.Day < @this.Day)
        {
            return DateTime.Today.Year - @this.Year - 1;
        }
        return DateTime.Today.Year - @this.Year;
    }

    public static bool IsDateEqual(this DateTime date, DateTime dateToCompare) => date.Date == dateToCompare.Date;

    public static bool IsToday(this DateTime @this) => @this.Date == DateTime.Today;

    public static bool IsWeekDay(this DateTime @this) => !(@this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday);

    public static bool IsWeekendDay(this DateTime @this) => @this.DayOfWeek == DayOfWeek.Saturday || @this.DayOfWeek == DayOfWeek.Sunday;

    public static DateTime StartOfDay(this DateTime @this) => new(@this.Year, @this.Month, @this.Day);

    public static DateTime StartOfMonth(this DateTime @this) => new(@this.Year, @this.Month, 1);

    public static DateTime StartOfWeek(this DateTime @this, DayOfWeek startDayOfWeek = DayOfWeek.Sunday)
    {
        var start = new DateTime(@this.Year, @this.Month, @this.Day);

        if (start.DayOfWeek != startDayOfWeek)
        {
            var d = startDayOfWeek - start.DayOfWeek;
            if (d <= 0)
            {
                return start.AddDays(d);
            }
            return start.AddDays(-7 + d);
        }

        return start;
    }

    public static DateTime StartOfYear(this DateTime @this) => new(@this.Year, 1, 1);

    public static TimeSpan ToEpochTimeSpan(this DateTime @this) => @this.ToUniversalTime().Subtract(new DateTime(1970, 1, 1));

    public static bool InRange(this DateTime @this, DateTime minValue, DateTime maxValue) => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static DateTime ConvertTime(this DateTime dateTime, TimeZoneInfo destinationTimeZone) => TimeZoneInfo.ConvertTime(dateTime, destinationTimeZone);

    public static DateTime ConvertTime(this DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone) => TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);

    public static DateTime ConvertTimeFromUTC(this DateTime dateTime, TimeZoneInfo destinationTimeZone) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, destinationTimeZone);

    public static DateTime ConvertTimeToUTC(this DateTime dateTime) => TimeZoneInfo.ConvertTimeToUtc(dateTime);

    public static DateTime ConvertTimeToUTC(this DateTime dateTime, TimeZoneInfo sourceTimeZone) => TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);

    /// <summary>
    /// ToDateString("yyyy-MM-dd")
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string ToStandardDateString(this DateTime @this) => @this.ToString("yyyy-MM-dd");

    /// <summary>
    /// ToTimeString("yyyy-MM-dd HH:mm:ss")
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string ToStandardTimeString(this DateTime @this) => @this.ToString("yyyy-MM-dd HH:mm:ss");

    #endregion DateTime

    #region Decimal

    public static bool InRange(this decimal @this, decimal minValue, decimal maxValue) => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static decimal Abs(this decimal value) => Math.Abs(value);

    public static decimal Ceiling(this decimal d) => Math.Ceiling(d);

    public static decimal Floor(this decimal d) => Math.Floor(d);

    public static decimal Max(this decimal val1, decimal val2) => Math.Max(val1, val2);

    public static decimal Min(this decimal val1, decimal val2) => Math.Min(val1, val2);

    public static decimal Round(this decimal d) => Math.Round(d);

    public static decimal Round(this decimal d, int decimals) => Math.Round(d, decimals);

    public static decimal Round(this decimal d, MidpointRounding mode) => Math.Round(d, mode);

    public static decimal Round(this decimal d, int decimals, MidpointRounding mode) => Math.Round(d, decimals, mode);

    public static int Sign(this decimal value) => Math.Sign(value);

    public static decimal Truncate(this decimal d) => Math.Truncate(d);

    public static decimal ToMoney(this decimal @this) => Math.Round(@this, 2);

    #endregion Decimal

    #region Delegate

    public static Delegate Combine(this Delegate a, Delegate b) => Delegate.Combine(a, b);

    public static Delegate? Remove(this Delegate source, Delegate value) => Delegate.Remove(source, value);

    public static Delegate? RemoveAll(this Delegate source, Delegate value) => Delegate.RemoveAll(source, value);

    #endregion Delegate

    #region Double

    public static bool InRange(this double @this, double minValue, double maxValue) => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static double Abs(this double value) => Math.Abs(value);

    public static double Acos(this double d) => Math.Acos(d);

    public static double Asin(this double d) => Math.Asin(d);

    public static double Atan(this double d) => Math.Atan(d);

    public static double Atan2(this double y, double x) => Math.Atan2(y, x);

    public static int Ceiling(this double a) => Convert.ToInt32(Math.Ceiling(a));

    public static double Cos(this double d) => Math.Cos(d);

    public static double Cosh(this double value) => Math.Cosh(value);

    public static double Exp(this double d) => Math.Exp(d);

    public static double Floor(this double d) => Math.Floor(d);

    public static double Log(this double d) => Math.Log(d);

    public static double Log(this double d, double newBase) => Math.Log(d, newBase);

    public static double Log10(this double d) => Math.Log10(d);

    public static double Max(this double val1, double val2) => Math.Max(val1, val2);

    public static double Min(this double val1, double val2) => Math.Min(val1, val2);

    public static double Pow(this double x, double y) => Math.Pow(x, y);

    public static double Round(this double a) => Math.Round(a);

    public static double Round(this double value, int digits) => Math.Round(value, digits);

    public static double Round(this double value, MidpointRounding mode) => Math.Round(value, mode);

    public static double Round(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

    public static int Sign(this double value) => Math.Sign(value);

    public static double Sin(this double a) => Math.Sin(a);

    public static double Sinh(this double value) => Math.Sinh(value);

    public static double Sqrt(this double d) => Math.Sqrt(d);

    public static double Tan(this double a) => Math.Tan(a);

    public static double Tanh(this double value) => Math.Tanh(value);

    public static double Truncate(this double d) => Math.Truncate(d);

    public static double ToMoney(this double @this) => Math.Round(@this, 2);

    #endregion Double

    #region Enum

    public static bool In(this Enum @this, params Enum[] values)
    {
        return Array.IndexOf(values, @this) >= 0;
    }

    public static string GetDescription(this Enum value)
    {
        var stringValue = value.ToString();
        var attr = value.GetType().GetField(stringValue)?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? stringValue;
    }

    #endregion Enum

    #region Guid

    public static bool IsNullOrEmpty(this Guid? @this) => !@this.HasValue || @this == Guid.Empty;

    public static bool IsNotNullOrEmpty(this Guid? @this) => @this.HasValue && @this.Value != Guid.Empty;

    public static bool IsEmpty(this Guid @this) => @this == Guid.Empty;

    public static bool IsNotEmpty(this Guid @this) => @this != Guid.Empty;

    #endregion Guid

    #region short

    public static bool InRange(this short @this, short minValue, short maxValue) => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static bool FactorTo(this short @this, short factorNumber) => factorNumber % @this == 0;

    public static bool IsEvan(this short @this) => @this % 2 == 0;

    public static bool IsOdd(this short @this) => @this % 2 != 0;

    /// <summary>
    /// An Int16 extension method that query if @this is prime
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static bool IsPrime(this short @this)
    {
        if (@this == 1 || @this == 2)
        {
            return true;
        }

        if (@this % 2 == 0)
        {
            return false;
        }

        var sqrt = (short)Math.Sqrt(@this);
        for (long t = 3; t <= sqrt; t = t + 2)
        {
            if (@this % t == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static byte[] GetBytes(this short value) => BitConverter.GetBytes(value);

    public static short Max(this short val1, short val2) => Math.Max(val1, val2);

    public static short Min(this short val1, short val2) => Math.Max(val1, val2);

    public static int Sign(this short value) => Math.Sign(value);

    /// <summary>
    /// Converts a short value from host byte order to network byte order.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static short HostToNetworkOrder(this short host) => IPAddress.HostToNetworkOrder(host);

    /// <summary>
    /// Covnerts a short value from network byte order to host byte order.
    /// </summary>
    /// <param name="network"></param>
    /// <returns></returns>
    public static short NetworkToHostOrder(this short network) => IPAddress.NetworkToHostOrder(network);

    #endregion short

    #region int 

    public static bool InRange(this int @this, int minValue, int maxValue) => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static bool FactorOf(this int @this, int factorNumber) => factorNumber % @this == 0;

    public static bool IsEven(this int @this) => @this % 2 == 0;

    public static bool IsOdd(this int @this) => @this % 2 != 0;

    public static bool IsMultipleOf(this int @this, int factor) => @this % factor == 0;

    public static bool IsPrime(this int @this)
    {
        if (@this == 1 || @this == 2)
        {
            return true;
        }

        if (@this % 2 == 0)
        {
            return false;
        }

        var sqrt = (int)Math.Sqrt(@this);
        for (long t = 3; t <= sqrt; t++)
        {
            if (@this % t == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static byte[] GetBytes(this int value) => BitConverter.GetBytes(value);

    public static string ConvertFromUtf32(this int utf32) => char.ConvertFromUtf32(utf32);

    public static int DaysInMonth(this int year, int month) => DateTime.DaysInMonth(year, month);

    public static bool IsLeapYear(this int year) => DateTime.IsLeapYear(year);

    public static int Abs(this int value) => Math.Abs(value);

    public static long BigMul(this int a, int b) => Math.BigMul(a, b);

    public static int DivRem(this int a, int b, out int result) => Math.DivRem(a, b, out result);

    public static int Max(this int val1, int val2) => Math.Max(val1, val2);

    public static int Min(this int val1, int val2) => Math.Min(val1, val2);

    public static int Sign(this int value) => Math.Sign(value);

    #endregion int

    #region long

    public static TimeSpan FromTicks(this long value) => TimeSpan.FromTicks(value);

    public static byte[] GetBytes(this long value) => BitConverter.GetBytes(value);

    public static double Int64BitsToDouble(this long value) => BitConverter.Int64BitsToDouble(value);

    #endregion long

    #region object

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">The Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A T.</returns>
    public static T? AsOrDefault<T>(this object? @this)
    {
        if (@this is null)
        {
            return default;
        }
        try
        {
            return (T?)@this;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A T.</returns>
    public static T AsOrDefault<T>(this object? @this, T defaultValue)
    {
        if (@this is null)
        {
            return defaultValue;
        }
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static T AsOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        if (@this is null)
        {
            return defaultValueFactory();
        }
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValueFactory();
        }
    }

    /// <summary>
    /// A System.Object extension method that toes the given this.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns></returns>
    public static T To<T>(this object? @this)
    {
#nullable disable

        if (@this == null || @this == DBNull.Value)
        {
            return (T)(object)null;
        }
#nullable restore

        var targetType = typeof(T).Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if (sourceType == targetType)
        {
            return (T)@this;
        }
        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertFrom(targetType))
        {
            return (T)converter.ConvertTo(@this, targetType)!;
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return (T)converter.ConvertFrom(@this)!;
        }

        return (T)Convert.ChangeType(@this, targetType);
    }

    public static object? To(this object? @this, Type type)
    {
        if (@this == null || @this == DBNull.Value)
        {
            return null;
        }

        var targetType = type.Unwrap();
        var sourceType = @this.GetType().Unwrap();

        if (sourceType == targetType)
        {
            return @this;
        }

        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            return converter.ConvertTo(@this, targetType);
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return converter.ConvertFrom(sourceType);
        }

        return Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    /// A System.Object extension method tha converts this object to an or default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="defaultValueFactory"></param>
    /// <returns></returns>
    public static T ToOrDefault<T>(this object? @this, Func<object?, T> defaultValueFactory)
    {
        try
        {
            return (T)@this.To(typeof(T))!;
        }
        catch (Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    public static T ToOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        return @this.ToOrDefault(_ => defaultValueFactory());
    }

    public static object? ToOrDefault(this object? @this, Type type)
    {
        Guard.NotNull(type, nameof(type));
        try
        {
            return @this.To(type);
        }
        catch (Exception)
        {
            return type.GetDefaultValue();
        }
    }

    public static T? ToOrDefault<T>(this object? @this)
    {
        return @this.ToOrDefault(_ => default(T));
    }

    public static T ToOrDefault<T>(this object? @this, T defaultValue)
    {
        return @this.ToOrDefault(_ => defaultValue);
    }

    public static bool IsAssignableFrom<T>(this object @this)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(typeof(T));
    }

    public static bool IsAssignableFrom(this object @this, Type targetType)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(targetType);
    }

    public static T Chain<T>(this T @this, Action<T>? action)
    {
        action?.Invoke(@this);

        return @this;
    }

    public static T? NullIf<T>(this T @this, Func<T, bool>? predicate) where T : class
    {
        if (predicate?.Invoke(@this) == true)
        {
            return default;
        }
        return @this;
    }

    public static TResult? GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func)
    {
        try
        {
            return func.Invoke(@this);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultValue)
    {
        try
        {
            return func.Invoke(@this);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// A TType extension method that tries.
    /// </summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValue">The catch value.</param>
    /// <returns>A TResult</returns>
    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue)
    {
        try
        {
            return tryFunction(@this);
        }
        catch (Exception)
        {
            return catchValue;
        }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory)
    {
        try
        {
            return tryFunction.Invoke(@this);
        }
        catch (Exception)
        {
            return catchValueFactory.Invoke(@this);
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, out TResult? result)
    {
        try
        {
            result = tryFunction.Invoke(@this);
            return true;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch (Exception)
        {
            result = catchValue;
            return false;
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch (Exception)
        {
            result = catchValueFactory(@this);
            return false;
        }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction, Action<TType> catchAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch (Exception)
        {
            catchAction(@this);
            return false;
        }
    }

    /// <summary>
    /// A T extension method that check  if the value is  between inclusively the minValue and maxValue.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>true if the value is between inclusively the minValue and maxValue, other false.</returns>
    public static bool InRange<T>(this T @this, T minValue, T maxValue) where T : IComparable<T>
    {
        return @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;
    }

    /// <summary>
    /// A T extension method that query if 'source' is the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsDefault<T>(this T source)
    {
        return source is null || source.Equals(default(T));
    }

    /// <summary>
    /// An object extension method than converts the @this to string or return an empty string if the value is null.
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string ToSafeString(this object? @this) => $"{@this}";

    #endregion object

    #region object[]

    public static Type[] GetTypeArray(this object[] args) => Type.GetTypeArray(args);

    #endregion object[]

    #region Random

    public static T OneOf<T>(this Random @this, params T[] values) => values[@this.Next(values.Length)];

    public static bool CoinToss(this Random @this) => @this.Next(2) == 0;

    #endregion Random

    #region string

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? @this) => string.IsNullOrEmpty(@this);

    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? @this) => !string.IsNullOrEmpty(@this);

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? @this) => string.IsNullOrWhiteSpace(@this);

    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? @this) => !string.IsNullOrWhiteSpace(@this);

    public static string Intern(this string str) => string.Intern(str);

    public static string? IsInterned(this string str) => string.IsInterned(str);

    public static string Join<T>(this string separator, IEnumerable<T> values) => string.Join(separator, values);

    public static bool IsMatch(this string input, string pattern) => Regex.IsMatch(input, pattern);

    public static bool IsMatch(this string input, string pattern, RegexOptions options) => Regex.IsMatch(input, pattern, options);

    public static string Concatenate(this IEnumerable<string> stringCollection) => string.Join(string.Empty, stringCollection);

    public static string Concatenate<T>(this IEnumerable<T> source, Func<T, string> func) => string.Join(string.Empty, source.Select(func));

    public static bool Contains(this string @this, string value) => Guard.NotNull(@this, nameof(@this)).IndexOf(value, StringComparison.Ordinal) != -1;

    public static bool Contains(this string @this, string value, StringComparison comparsionType) => Guard.NotNull(@this, nameof(@this)).IndexOf(value, comparsionType) != -1;

    public static string Extract(this string @this, Func<char, bool> predicate) => new(Guard.NotNull(@this, nameof(@this)).ToCharArray().Where(predicate).ToArray());

    public static string RemoveWhere(this string @this, Func<char, bool> predicate) => new(Guard.NotNull(@this, nameof(@this)).ToCharArray().Where(x => !predicate(x)).ToArray());

    public static string FormatWith(this string @this, params object[] values) => string.Format(Guard.NotNull(@this, nameof(@this)), values);

    public static bool IsLike(this string @this, string pattern)
    {
        var regexPattern = "^" + Regex.Escape(pattern) + "$";

        regexPattern = regexPattern.Replace(@"\[!", "[^")
            .Replace(@"\[", "[")
            .Replace(@"\]", "]")
            .Replace(@"\?", ".")
            .Replace(@"\*", ".*")
            .Replace(@"\#", @"\d");

        return Regex.IsMatch(Guard.NotNull(@this, nameof(@this)), regexPattern);
    }

    public static string SafeSubstring(this string @this, int startIndex)
    {
        if (startIndex < 0 || startIndex >= @this.Length)
        {
            return string.Empty;
        }
        return @this.Substring(startIndex);
    }

    public static string SafeSubstring(this string str, int startIndex, int length)
    {
        Guard.NotNull(str, nameof(str));
        if(startIndex<0 || startIndex>=str.Length||length<0)
        {
            return string.Empty;
        }
        return str.Substring(startIndex, Math.Min(length, str.Length - startIndex));
    }

    public static string Sub(this string @this,int startIndex)
    {
        Guard.NotNull(@this, nameof(@this));

        if(startIndex>=0)
        {
            return @this.SafeSubstring(startIndex);
        }

        if(Math.Abs(startIndex)>@this.Length)
        {
            return string.Empty;
        }

        return @this.Substring(@this.Length + startIndex);
    }

    public static string Repear(this string @this, int repeatCount)
    {
        Guard.NotNull(@this, nameof(@this));
        if (@this.Length == 1)
        {
            return new string(@this[0], repeatCount);
        }

        var sb = new StringBuilder(@this.Length * repeatCount);
        while (repeatCount-- > 0)
        {
            sb.Append(@this);
        }

        return sb.ToString();
    }

    public static string Reverse(this string? @this)
    {
        if(@this.IsNullOrWhiteSpace())
        {
            return @this ?? string.Empty;
        }

        var chars = @this.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static string[] Split(this string @this, string separator, StringSplitOptions option = StringSplitOptions.None) => Guard.NotNull(@this, nameof(@this)).Split(new[] { separator }, option);

    public static byte[] ToByteArray(this string @this) => Encoding.UTF8.GetBytes(Guard.NotNull(@this, nameof(@this)));

    public static byte[] ToByteArray(this string @this, Encoding encoding) => encoding.GetBytes(Guard.NotNull(@this, nameof(@this)));

    public static byte[] GetBytes(this string @this) => Guard.NotNull(@this, nameof(@this)).GetBytes(Encoding.UTF8);

    public static byte[] GetBytes(this string str, Encoding encoding) => encoding.GetBytes(Guard.NotNull(str, nameof(str)));

    public static T ToEnum<T>(this string @this) => (T)Enum.Parse(typeof(T), Guard.NotNull(@this, nameof(@this)));

    public static string ToTitleCase(this string @this) => new CultureInfo("en-US").TextInfo.ToTitleCase(Guard.NotNull(@this, nameof(@this)));

    public static string ToTitleCase(this string @this, CultureInfo cultureInfo) => cultureInfo.TextInfo.ToTitleCase(Guard.NotNull(@this, nameof(@this)));

    public static string Truncate(this string @this, int maxLength) => Guard.NotNull(@this, nameof(@this)).Truncate(maxLength, "...");

    public static string Truncate(this string @this,int maxLength,string suffix)
    {
        if(Guard.NotNull(@this).Length<=maxLength)
        {
            return @this;
        }
        return @this.Substring(0, maxLength - suffix.Length) + suffix;
    }

    public static bool EqualsIgnoreCase(this string? s1, string? s2) => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

    #endregion string

    #region StringBuilder

    public static string Substring(this StringBuilder @this, int startIndex) => @this.ToString(startIndex, @this.Length - startIndex);

    public static string Substring(this StringBuilder @this, int startIndex, int length) => @this.ToString(startIndex, length);

    public static StringBuilder AppendJoin<T>(this StringBuilder @this, string separator, IEnumerable<T> values) => @this.Append(string.Join(separator, values));

    public static StringBuilder AppendLineJoin<T>(this StringBuilder @this, string separator, IEnumerable<T> values) => @this.AppendLine(string.Join(separator, values));

    public static StringBuilder AppenIf(this StringBuilder builder,string text,bool condition)
    {
        Guard.NotNull(builder, nameof(builder));
        if (condition)
        {
            builder.Append(text);
        }
        return builder;
    }

    public static StringBuilder AppendIf(this StringBuilder builder, Func<string> textFactory, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));
        if (condition)
        {
            builder.Append(textFactory());
        }
        return builder;
    }

    public static StringBuilder AppendLineIf(this StringBuilder builder, string text, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));

        if (condition)
        {
            builder.AppendLine(text);
        }
        return builder;
    }

    /// <summary>
    /// Append text when condition is true
    /// </summary>
    /// <param name="builder">StringBuilder</param>
    /// <param name="textFactory">text factory to produce text for appendding</param>
    /// <param name="condition">condition to evaluate</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendLineIf(this StringBuilder builder, Func<string> textFactory, bool condition)
    {
        Guard.NotNull(builder, nameof(builder));

        if (condition)
        {
            builder.AppendLine(textFactory());
        }
        return builder;
    }

    #endregion StringBulder

    #region TimeSpan

    public static DateTime Ago(this TimeSpan @this) => DateTime.Now.Subtract(@this);

    public static DateTime FromNow(this TimeSpan @this) => DateTime.Now.Add(@this);

    public static DateTime UtcAgo(this TimeSpan @this) => DateTime.UtcNow.Subtract(@this);

    public static DateTime UtcFromNow(this TimeSpan @this) => DateTime.UtcNow.Add(@this);

    #endregion TimeSpan

    #region Type

    /// <summary>
    /// GetUnderlyingType if nullable else return self.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type Unwrap(this Type type) 
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type))) ?? type;


    public static Type? GetUnderlyingType(this Type? type) 
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type)));


    private static readonly ConcurrentDictionary<Type, object> DefaultValues = new();

    public static object? GetDefaultValue(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsValueType && type != typeof(void)
            ? DefaultValues.GetOrAdd(type, Activator.CreateInstance)
            : null;
    }

    public static T? CreateInstance<T>(this Type @this, params object?[]? args) => (T?)Activator.CreateInstance(@this, args);

    public static bool HasEmptyConstructor(this Type type) => Guard.NotNull(type, nameof(type)).GetConstructors().Any(c => c.GetParameters().Length == 0);

    public static bool IsNullableType(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    #endregion Type
}
