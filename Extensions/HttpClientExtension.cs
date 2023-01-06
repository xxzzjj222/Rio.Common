

using Newtonsoft.Json;
using Rio.Common;
using Rio.Common.Http;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rio.Extensions;

public static class HttpClientExtension
{
    private sealed class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        public BasicAuthenticationHeaderValue(string userName, string password)
            : base("Basic", EncodeCredential(userName, password))
        {

        }

        private static string EncodeCredential(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            return Convert.ToBase64String($"{UrlEncode(userName)}:{UrlEncode(password)}".ToByteArray());
        }

        private static string UrlEncode(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Uri.EscapeDataString(value).Replace("%20", "+");
        }
    }

    public static Task<HttpResponseMessage> PostJsonRequestAsync<T>
        (this HttpClient httpClient
        , string requestUrl
        , T parameter
        , Action<HttpRequestMessage>? requestAction = null
        , CancellationToken cancellationToken = default)
        => HttpJsonRequestAsync(httpClient, HttpMethod.Post, requestUrl, parameter, requestAction, cancellationToken);

    public static Task<HttpResponseMessage> PutJsonRequestAsync<T>
        (this HttpClient httpClient
        , string requestUrl
        , T parameter
        , Action<HttpRequestMessage>? requestAction = null
        , CancellationToken cancellationToken = default)
        => HttpJsonRequestAsync(httpClient, HttpMethod.Put, requestUrl, parameter, requestAction, cancellationToken);

    public static Task<TResponse?> PostJsonAsync<TRequest, TResponse>
        (this HttpClient httpClient
        , string requestUrl
        , TRequest request
        , Action<HttpRequestMessage>? requestAction = null
        , Action<HttpResponseMessage>? responseAction = null
        , CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Post, requestUrl, request, requestAction, responseAction, cancellationToken);

    public static Task<TResponse?> PutJsonAsync<TRequest, TResponse>
        (this HttpClient httpClient
        , string requestUrl
        , TRequest request
        , Action<HttpRequestMessage>? requestAction = null
        , Action<HttpResponseMessage>? responseAction = null
        , CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Put, requestUrl, request, requestAction, responseAction, cancellationToken);

    public static async Task<HttpResponseMessage> HttpJsonRequestAsync<TRequest>
        (this HttpClient httpClient
        , HttpMethod httpMethod
        , string requestUrl
        , TRequest request
        , Action<HttpRequestMessage>? requestAction = null
        , CancellationToken cancellationToken = default)
    {
        Guard.NotNull(httpClient);
        using var requestMessage = new HttpRequestMessage(httpMethod, requestUrl)
        {
            Content = JsonHttpContent.From(request)
        };
        requestAction?.Invoke(requestMessage);
        return await httpClient.SendAsync(requestMessage, cancellationToken);
    }

    public static async Task<TResponse?> ReadJsonResponseAsync<TResponse>
        (this HttpResponseMessage response
        , Action<HttpResponseMessage>? responseAction = null
        , CancellationToken cancellationToken = default)
    {
        Guard.NotNull(response);
        responseAction?.Invoke(response);
#if NET6_0_OR_GREATER
        var responseText=await response.Content.ReadAsStringAsync(cancellationToken);
#else
        var responseText = await response.Content.ReadAsStringAsync();
#endif
        return JsonConvert.DeserializeObject<TResponse>(responseText);
    }

    public static async Task<TResponse?> HttpJsonAsync<TReqeust, TResponse>
        (this HttpClient httpClient
        , HttpMethod httpMethod
        , string requestUrl
        , TReqeust request
        , Action<HttpRequestMessage>? requestAction = null
        , Action<HttpResponseMessage>? responseAction = null
        , CancellationToken cancellationToken = default)
    {
        Guard.NotNull(httpClient);
        using var requestMessage = new HttpRequestMessage(httpMethod, requestUrl)
        {
            Content = JsonHttpContent.From(request)
        };
        requestAction?.Invoke(requestMessage);

        using var response = await httpClient.SendAsync(requestMessage);
        responseAction?.Invoke(response);
#if NET6_0_OR_GREATER
        var responseText=await response.Content.ReadAsStringAsync(cancellationToken);
#else
        var responseText = await response.Content.ReadAsStringAsync();
#endif
        return JsonConvert.DeserializeObject<TResponse>(responseText);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// PatchAsJsonAsync
    /// </summary>
    public static Task<HttpResponseMessage> PatchJsonRequestAsync<T>(this HttpClient httpClient, string requestUrl, T parameter, Action<HttpRequestMessage>? requestAction = null,
        CancellationToken cancellationToken = default)
         => HttpJsonRequestAsync(httpClient, HttpMethod.Patch, requestUrl, parameter, requestAction, cancellationToken);

    /// <summary>
    /// Patch Json request body and get object from json response
    /// </summary>
    public static Task<TResponse?> PatchJsonAsync<TRequest, TResponse>
    (this HttpClient httpClient, string requestUrl,
        TRequest request, Action<HttpRequestMessage>? requestAction = null,
        Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Patch, requestUrl, request, requestAction, responseAction,
            cancellationToken);
#endif

    public static Task<HttpResponseMessage> PostAsFormAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic)
        => httpClient.PostAsync(requestUri, new FormUrlEncodedContent(paramDic));

    public static Task<HttpResponseMessage> PutAsFormAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic)
        => httpClient.PutAsync(requestUri, new FormUrlEncodedContent(paramDic));

    public static Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient
        ,string requestUrl
        ,string filePath
        ,string fileKey="file"
        ,IEnumerable<KeyValuePair<string,string>>? formFields=null)
    {
        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");

        if(formFields != null)
        {
            foreach (var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }

        content.Add(new StreamContent(File.OpenRead(filePath)),fileKey,Path.GetFileName(filePath));

        return httpClient.PostAsync(requestUrl, content);
    }

    public static async Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient
        ,string requestUrl
        ,Stream? file
        ,string fileName
        ,string fileKey="file"
        ,IEnumerable<KeyValuePair<string,string>>? formFields=null)
    {
        if(file == null)
        {
            return await httpClient.PostAsFormAsync(requestUrl, formFields ?? Array.Empty<KeyValuePair<string, string>>());
        }

        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");
        if(formFields != null)
        {
            foreach (var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }
        content.Add(new StreamContent(file), fileKey, fileName);

        return await httpClient.PostAsync(requestUrl,content);
    }

    public static Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient
        , string requestUrl
        , ICollection<string> filePaths
        , IEnumerable<KeyValuePair<string, string>>? formFields = null)
        => httpClient.PostFileAsync(requestUrl, filePaths.Select(p =>
          new KeyValuePair<string, Stream>(Path.GetFileName(p), File.OpenRead(p))), formFields);


    public static async Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient
        ,string requestUri
        ,IEnumerable<KeyValuePair<string,Stream>>? files
        ,IEnumerable<KeyValuePair<string,string>>? formFields=null)
    {
        if(files==null)
        {
            return await httpClient.PostAsFormAsync(requestUri, formFields ?? Array.Empty<KeyValuePair<string, string>>());
        }

        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");
        if(formFields!=null)
        {
            foreach(var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }

        foreach (var file in files)
        {
            content.Add(new StreamContent(file.Value), Path.GetFileNameWithoutExtension(file.Key), Path.GetFileName(file.Key));
        }

        return await httpClient.PostAsync(requestUri,content);
    }

    public static void SetBasicAuthentication(this HttpClient client,string userName,string password)
        =>client.DefaultRequestHeaders.Authorization=new BasicAuthenticationHeaderValue(userName,password);

    public static void SetBasicAuthentication(this HttpRequestMessage request,string userName,string password)
        =>request.Headers.Authorization=new BasicAuthenticationHeaderValue(userName,password);

    public static void SetBasicAuthenticationOAuth(this HttpRequestMessage request, string userName, string password)
        => request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);

    public static void SetToken(this HttpClient client, string scheme, string token)
        => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

    public static void SetToken(this HttpRequestMessage request,string scheme,string token)
        =>request.Headers.Authorization=new AuthenticationHeaderValue(scheme, token);

    public static void SetBearerToken(this HttpClient client, string token)
        => client.SetToken("Bearer", token);

    public static void SetBearerToken(this HttpRequestMessage request, string token)
        => request.SetToken("Bearer", token);
}

