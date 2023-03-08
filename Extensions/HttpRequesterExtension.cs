using Rio.Common.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Rio.Common.Helpers;

namespace Rio.Extensions;

public static class HttpRequesterExtension
{
    public static IHttpRequester WithUrl(this IHttpRequester httpRequester,string url,IEnumerable<KeyValuePair<string,string>> queryParams)
    {
        var requestUrl = url.IndexOf("?", StringComparison.OrdinalIgnoreCase) > 0 ?
            $"{url}?{queryParams.ToQueryString()}" :
            $"{url}&{queryParams.ToQueryString()}";

        return httpRequester.WithUrl(requestUrl);
    }

    public static IHttpRequester AjaxRequest(this IHttpRequester httpRequester)
    {
        return httpRequester.WithHeaders(new Dictionary<string, string?> { { "X-Requested-With", "XMLHttpRequest" } });
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester,string cookieName,string cookieValue)
    {
        return httpRequester.WithCookie(new Cookie(cookieName,cookieValue));
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester,Cookie cookie)
    {
        return httpRequester.WithCookie(null,cookie);
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester,CookieCollection cookies)
    {
        return httpRequester.WithCookie(null, cookies);
    }

    public static IHttpRequester WithProxy(this IHttpRequester httpRequester,string url)
    {
        return httpRequester.WithProxy(new WebProxy(new Uri(url)));
    }

    public static IHttpRequester WithProxy(this IHttpRequester httpRequester,string url,string userName,string password)
    {
        return httpRequester.WithProxy(new WebProxy(new Uri(url))
        {
            Credentials = new NetworkCredential(userName, password)
        });
    }

    public static IHttpRequester WithXmlParameter<TEntity>(this IHttpRequester httpRequester,TEntity entity)
    {
        return httpRequester.WithParameters(XmlDataSerializer._instance.Value.Serializer(entity),
            "application/xml;charset=UTF-8");
    }

    public static IHttpRequester WithJsonParameter<TEntity>(this IHttpRequester httpRequester, TEntity entity)
    {
        return httpRequester.WithParameters(entity.ToJson().GetBytes(), "application/json;charset=UTF-8");
    }

    public static IHttpRequester WithFormParameters<TEntity>(this IHttpRequester httpRequester,
        IEnumerable<KeyValuePair<string, string>> formParams)
    {
        return httpRequester.WithParameters(formParams.ToQueryString().GetBytes(),
            "application/x-www-form-urlencoded;charset=UTF-8");
    }

    public static IHttpRequester WithFile(this IHttpRequester httpRequester, string filePath, string fileKey = "file",
        IEnumerable<KeyValuePair<string, string>>? formFields = null)
        => httpRequester.WithFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

    public static IHttpRequester WithFiles(this IHttpRequester httpRequester, IEnumerable<string> filePaths,
        IEnumerable<KeyValuePair<string, string>>? formFields)
        => httpRequester.WithFiles(
            filePaths.Select(_ => new KeyValuePair<string, byte[]>(Path.GetFileName(_), File.ReadAllBytes(_))),
            formFields);

    public static string Execute(this IHttpRequester httpRequester) => httpRequester.ExecuteBytes().GetString();

    public static T? Execute<T>(this IHttpRequester httpRequester, T? defaultValue = default) =>
        httpRequester.ExecuteBytes().GetString().ToOrDefault(defaultValue);

    public static Task<string> ExecuteAsync(this IHttpRequester httpRequester)
        => httpRequester.ExecuteBytesAsync().ContinueWith(r => r.Result.GetString());

    public static Task<T?> ExecuteAsync<T>(this IHttpRequester httpRequester, T? defaultValue = default)
        => httpRequester.ExecuteBytesAsync().ContinueWith(r => r.Result.GetString().ToOrDefault(defaultValue));

    public static TEntity ExecuteForJson<TEntity>(this IHttpRequester httpRequester)
        => httpRequester.Execute().JsonToObject<TEntity>();

    public static Task<TEntity> ExecuteForJsonAsync<TEntity>(this IHttpRequester httpRequester)
        => httpRequester.ExecuteAsync().ContinueWith(r => r.Result.JsonToObject<TEntity>());

    public static TEntity ExecuteForXml<TEntity>(this IHttpRequester httpRequester)
        => XmlDataSerializer._instance.Value.Deserializer<TEntity>(httpRequester.ExecuteBytes());

    public static Task<TEntity> ExecuteForXmlAsync<TEntity>(this IHttpRequester httpRequester)
        => httpRequester.ExecuteBytesAsync()
            .ContinueWith(r => XmlDataSerializer._instance.Value.Deserializer<TEntity>(r.Result));
}

