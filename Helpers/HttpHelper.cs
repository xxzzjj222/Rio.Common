﻿
using Rio.Common.Http;
using Rio.Extensions;
using System.Net;

namespace Rio.Common.Helpers;

public static class HttpHelper
{
    #region Constants

    #region UploadFileHeaderFormat

    /// <summary>
    /// 文件头
    /// 0:fileKey
    /// 1:fileName
    /// </summary>
    internal const string FileHeaderFormat =
        "Content-Disposition: form-data: name=\"{0}\"; filename=\"{1}\"\r\n"
        + "Content-Type: application/octet-stream\r\n\r\n";

    internal const string FormDataFormat = "\r\n--{2}\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

    #endregion UploadFileHeaderFormat

    #endregion Constants

    public static readonly HashSet<string> WellKnownContentHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpKnownHeaderNames.ContentDisposition,
        HttpKnownHeaderNames.ContentEncoding,
        HttpKnownHeaderNames.ContentLanguage,
        HttpKnownHeaderNames.ContentLength,
        HttpKnownHeaderNames.ContentLocation,
        HttpKnownHeaderNames.ContentMD5,
        HttpKnownHeaderNames.ContentRange,
        HttpKnownHeaderNames.ContentType,
        HttpKnownHeaderNames.Expires,
        HttpKnownHeaderNames.LastModified
    };

    public static bool IsWellKnownContentHeader(string header)
        => WellKnownContentHeaders.Contains(header);

    #region WebRequest

    #region HttpGet

    public static string HttpGetString(string url,IEnumerable<KeyValuePair<string,string>>? customHeaders,WebProxy? proxy)
    {
        var request = WebRequest.CreateHttp(url);
        request.UserAgent = GetUserAgent();
        request.Method = "GET";

        if(null!=customHeaders)
        {
            foreach(var header in customHeaders)
            {
                if(header.Key.EqualsIgnoreCase("REFERER"))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if(header.Key.EqualsIgnoreCase("User-Agent"))
                {
                    request.UserAgent = header.Value;
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if(null!=proxy)
        {
            request.Proxy = proxy;
        }

        return request.GetResponseString();
    }

    public static byte[] HttpGetForBytes(string url)
        =>HttpGetForBytes(url,null,null);

    public static byte[] HttpGetForBytes(string url,IEnumerable<KeyValuePair<string,string>>? customHeaders)
        =>HttpGetForBytes(url,customHeaders,null);

    public static byte[] HttpGetForBytes(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders, WebProxy? proxy)
    {
        var request = WebRequest.CreateHttp(url);
        request.UserAgent = GetUserAgent();
        request.Method = "GET";

        if (null != customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase("REFERER"))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("User-Agent"))
                {
                    request.UserAgent = header.Value;
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if(null!=proxy)
        {
            request.Proxy=proxy;
        }

        return request.GetResponseBytesSafe();
    }

    public static async Task<byte[]> HttpGetForBytesAsync(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders, WebProxy? proxy)
    {
        var request = WebRequest.CreateHttp(url);
        request.UserAgent = GetUserAgent();
        request.Method = "GET";

        if (null != customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase("REFERER"))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("User-Agent"))
                {
                    request.UserAgent = header.Value;
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (null != proxy)
        {
            request.Proxy = proxy;
        }

        return await request.GetResponseBytesSafeAsync().ConfigureAwait(false);
    }

    #endregion HttpGet

    #region HttpPost
    #endregion HttpPost

    #endregion WebRequest

    #region UserAgents

    private static readonly string[] MobileUserAgents =
    {
        "Mozilla/5.0 (iPhone 84; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Version/10.0 MQQBrowser/7.8.0 Mobile/14G60 Safari/8536.25 MttCustomUA/2 QBWebViewType/1 WKType/1",
        "Mozilla/5.0 (Linux; Android 7.0; STF-AL10 Build/HUAWEISTF-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 V1_AND_SQ_7.2.0_730_YYB_D QQ/7.2.0.3270 NetType/4G WebP/0.3.0 Pixel/1080",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 MicroMessenger/6.5.18 NetType/WIFI Language/en",
        "Mozilla/5.0 (Linux; Android 5.1.1; vivo Xplay5A Build/LMY47V; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/48.0.2564.116 Mobile Safari/537.36 T7/9.3 baiduboxapp/9.3.0.10 (Baidu; P1 5.1.1)",
        "Mozilla/5.0 (Linux; U; Android 7.0; zh-cn; STF-AL00 Build/HUAWEISTF-AL00) AppleWebKit/537.36 (KHTML, like Gecko)Version/4.0 Chrome/37.0.0.0 MQQBrowser/7.9 Mobile Safari/537.36",
        "Mozilla/5.0 (iPhone 92; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.0 MQQBrowser/7.7.2 Mobile/14F89 Safari/8536.25 MttCustomUA/2 QBWebViewType/1 WKType/1",
        "Mozilla/5.0 (Linux; U; Android 6.0.1; zh-CN; SM-C7000 Build/MMB29M) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/40.0.2214.89 UCBrowser/11.6.2.948 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; U; Android 5.1.1; zh-cn; MI 4S Build/LMY47V) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.146 Mobile Safari/537.36 XiaoMi/MiuiBrowser/9.1.3",
        "Mozilla/5.0 (Linux; Android 7.0; MIX Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 V1_AND_SQ_7.2.0_730_YYB_D QQ/7.2.0.3270 NetType/WIFI WebP/0.3.0 Pixel/1080",
        "Mozilla/5.0 (Linux; Android 7.1.1; MI 6 Build/NMF26X; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 MicroMessenger/6.5.13.1100 NetType/WIFI Language/zh_CN",
        "Mozilla/5.0 (Linux; U; Android 7.0; zh-cn; MIX Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.146 Mobile Safari/537.36 XiaoMi/MiuiBrowser/9.2.2",
        "Mozilla/5.0 (Linux; U; Android 6.0.1; zh-cn; MIX Build/MXB48T) AppleWebKit/537.36 (KHTML, like Gecko)Version/4.0 Chrome/37.0.0.0 MQQBrowser/7.8 Mobile Safari/537.36"
    };

    private static readonly string[] DesktopUserAgents =
    {
        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.103 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.38 Safari/537.36",
        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36",
        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.97 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0",
        "Opera/9.80 (Windows NT 6.2; Win64; x64) Presto/2.12.388 Version/12.17",
        "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
        "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 OPR/26.0.1656.60",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:45.0) Gecko/20100101 Firefox/45.0",
        "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36",
        "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.122 Safari/534.30",
        "Mozilla/5.0 (Windows NT 5.1; rv:5.0) Gecko/20100101 Firefox/5.0",
        "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50",
        "Mozilla/5.0 (Windows; U; Windows NT 5.1; ) AppleWebKit/534.12 (KHTML, like Gecko) Maxthon/3.0 Safari/534.12"
    };

    private static readonly string[] WeChatUserAgents =
    {
        "Mozilla/5.0 (Linux; Android 6.0; 1503-M02 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 Mobile MQQBrowser/6.2 TBS/036558 Safari/537.36 MicroMessenger/6.3.25.861 NetType/WIFI Language/zh_CN",
        "Mozilla/5.0 (Linux; Android 5.1; OPPO R9tm Build/LMY47I; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043220 Safari/537.36 MicroMessenger/6.5.7.1041 NetType/4G Language/zh_CN",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 9_3 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Mobile/13E233 MicroMessenger/6.3.15 NetType/WIFI Language/zh_CN",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_2_1 like Mac OS X) AppleWebKit/602.4.6 (KHTML, like Gecko) Mobile/14D27 MicroMessenger/6.5.6 NetType/4G Language/zh_CN"
    };

    public static string GetUserAgent(bool isMobileUserAgent=false)
    {
        return isMobileUserAgent ? MobileUserAgents[SecurityHelper.Random.Next(MobileUserAgents.Length)] : DesktopUserAgents[SecurityHelper.Random.Next(DesktopUserAgents.Length)];
    }

    public static string GetWeChatUserAgent()
        =>WeChatUserAgents[SecurityHelper.Random.Next(WeChatUserAgents.Length)];

    #endregion UserAgents
}

