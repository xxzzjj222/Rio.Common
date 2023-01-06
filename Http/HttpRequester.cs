using Rio.Common.Helpers;
using Rio.Extensions;
using System.Net;

namespace Rio.Common.Http;

public class HttpResponse
{
    public HttpStatusCode StatusCode { get;set; }

    public byte[]? ResponseBytes { get; set; }

    public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();
}

public class WebRequestHttpRequester:IHttpRequester
{
    public static IHttpRequester New() => new WebRequestHttpRequester();

    #region private fields

    private HttpWebRequest _request = null!;
    private byte[]? _requestDataBytes;
    private string _requestUrl = null!;

    #endregion private fields

    #region ctor

    public WebRequestHttpRequester()
    {

    }

    public WebRequestHttpRequester(string requestUrl):this(requestUrl,HttpMethod.Get)
    {

    }

    public WebRequestHttpRequester(string requestUrl,IDictionary<string,string>? queryDictionary,HttpMethod method)
    {
        _requestUrl = $"{requestUrl}{(requestUrl.Contains("?") ? "&" : "?")}{queryDictionary.ToQueryString()}";
        _request = WebRequest.CreateHttp(requestUrl);
        _request.UserAgent = HttpHelper.GetUserAgent();

        _request.Method = method.Method;
    }

    public WebRequestHttpRequester(string requestUrl,HttpMethod method)
    {
        _requestUrl = requestUrl;
        _request=WebRequest.CreateHttp(requestUrl);
        _request.UserAgent = HttpHelper.GetUserAgent();

        _request.Method = method.Method;
    }

    #endregion ctor

    public IHttpRequester WithUrl(string url)
    {
        _requestUrl= url;
        _request = WebRequest.CreateHttp(url);
        return this;
    }
}
