using System.Net;

namespace Rio.Common.Http;

public interface IHttpRequester
{
    #region WithUrl

    IHttpRequester WithUrl(string url);

    #endregion WithUrl

    #region Method

    IHttpRequester WithMethod(HttpMethod method);

    #endregion Method

    #region AddHeader

    IHttpRequester WithHeaders(IEnumerable<KeyValuePair<string, string?>> customHeaders);

    #endregion AddHeader

    #region UserAgent

    IHttpRequester WithUserAgent(string userAgent);

    #endregion UserAgent

    #region Rerferer

    IHttpRequester WithReferer(string referer);

    #endregion Rerferer

    #region Cookie

    IHttpRequester WithCookie(string? url, Cookie cookie);

    IHttpRequester WithCookie(string? url, CookieCollection cookies);

    #endregion Cookie

    #region Proxy

    IHttpRequester WithProxy(IWebProxy proxy);

    #endregion Proxy

    #region Parameter

    IHttpRequester WithParameters(byte[] requestBytes, string contentByte);

    IHttpRequester WithFile(string fileName, byte[] fileBytes, string fileKey = "file"
        , IEnumerable<KeyValuePair<string, string>>? fileFields = null);

    IHttpRequester WithFile(IEnumerable<KeyValuePair<string, byte[]>> files
        , IEnumerable<KeyValuePair<string, string>>? fileFields = null);

    #endregion Parameter

    #region Execute

    byte[] ExecuteBytes();

    Task<byte[]> ExecuteBytesAsync();

    HttpResponse ExecuteForResponse();

    Task<HttpResponse> ExecuteForResponseAsync();

    #endregion Execute
}

