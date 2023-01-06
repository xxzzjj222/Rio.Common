using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Common.Http
{
    public sealed class JsonHttpContent:StringContent
    {
        private const string JsonMediaType = "application/json";

        public JsonHttpContent(object? obj, JsonSerializerSettings? jsonSerializerSettings=null)
            :base(JsonConvert.SerializeObject(obj, jsonSerializerSettings))
        {

        }

        public JsonHttpContent(string content) : this(content, Encoding.UTF8)
        {

        }

        public JsonHttpContent(string content,Encoding encoding)
            :base(content,encoding)
        {

        }

        public static HttpContent From(object? obj,JsonSerializerSettings? jsonSerializerSettings=null)
        {
            if(obj is null)
            {
                return new StringContent(string.Empty, Encoding.UTF8, JsonMediaType);
            }
            return new JsonHttpContent(obj, jsonSerializerSettings);
        }
    }
}
