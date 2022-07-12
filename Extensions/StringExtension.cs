using System.Web;

namespace Rio.Common.Extensions
{
    /// <summary>
    /// StringExtension
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Encodes a URL string
        /// </summary>
        /// <param name="str">The text to encode</param>
        /// <returns>An encoded string</returns>
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }
    }
}
