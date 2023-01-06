using Rio.Common;
using System.Net;
using System.Text;

namespace Rio.Extensions;

public static class NetExtension
{
    public static string GetResponseString(this WebRequest @this)
    {
        using var response = @this.GetResponse();
        return response.ReadToEnd();
    }

    public static byte[] GetResponseBytesSafe(this WebRequest @this)
    {
        using var response = @this.GetResponseSafe();
        return response.ReadAllBytes();
    }

    public static async Task<byte[]> GetResponseBytesSafeAsync(this WebRequest @this)
    {
        using var response = await @this.GetResponseSafeAsync().ConfigureAwait(false);
        return response.ReadAllBytes();
    }

    public static WebResponse GetResponseSafe(this WebRequest @this)
    {
        try
        {
            return @this.GetResponse();
        }
        catch (WebException e)
        {
            return Guard.NotNull(e.Response);
        }
    }

    public static async Task<WebResponse> GetResponseSafeAsync(this WebRequest @this)
    {
        try
        {
            return await @this.GetResponseAsync().ConfigureAwait(false);
        }
        catch(WebException e)
        {
            return Guard.NotNull(e.Response);
        }
    }

    public static string ReadToEnd(this WebResponse response)
    {
        using var stream = response.GetResponseStream()!;
        return stream.ReadToEnd(Encoding.UTF8);
    }

    public static byte[] ReadAllBytes(this WebResponse response)
    {
        using var stream = response.GetResponseStream()!;
        var byteArray = new byte[stream.Length];
        stream.Write(byteArray, 0, byteArray.Length);
        return byteArray;
    }

    public static async Task<byte[]> ReadAllBytesAsync(this WebResponse response)
    {
        using var stream = response.GetResponseStream()!;
        var byteArray = new byte[stream.Length];
        await stream.WriteAsync(byteArray, 0, byteArray.Length).ConfigureAwait(false);
        return byteArray;
    }
}

