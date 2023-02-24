using Rio.Common.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rio.Extensions;

public static class HttpRequesterExtension
{
    public static IHttpRequester WithCookie(this IHttpRequester httpRequester,CookieCollection cookies)
    {
        return httpRequester.WithCookie(cookies);
    }
}

