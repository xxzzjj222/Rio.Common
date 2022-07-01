using System.Collections.Specialized;
using System.Text;

namespace Rio.Common.Extensions
{
    /// <summary>
    /// CollectionExtension
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// A NameValueCollection extension method than converts the @this to a dictionary
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as an IDictionary&lt;string,string?&gt;</returns>
        public static IDictionary<string ,string?> ToDictionary(this NameValueCollection? @this)
        {
            var dict=new Dictionary<string ,string?>();

            if(@this!=null)
            {
                foreach (var key in @this.AllKeys)
                {
                    dict.Add(Guard.NotNull(key), @this[key]);
                }
            }

            return dict;
        }

        /// <summary>
        /// 将名值集合转换成字符串，key1=value&amp;key2=value2, k/v会编码
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns>字符串</returns>
        public static string ToQueryString(this NameValueCollection? source)
        {
            if (source == null || source.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var key in source.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                sb.Append("&");
                sb.Append(key.UrlEncode());
                sb.Append("=");
                var val = source.Get(key);
                if (val != null)
                {
                    sb.Append(val.UrlEncode());
                }
            }

            return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : "";
        }
    }
}
