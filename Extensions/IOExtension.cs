using System.Runtime.CompilerServices;
using System.Text;

namespace Rio.Extensions;
public static class IOExtension
{
    /// <summary>
    /// 把字节数组全部写入当前流
    /// </summary>
    /// <param name="this">当前流</param>
    /// <param name="byteArray">要写入的字节数组</param>
    public static void Write(this Stream @this, byte[] byteArray)
    {
        @this.Write(byteArray, 0, byteArray.Length);
    }

    /// <summary>
    /// 把字节数组全部写入当前流
    /// </summary>
    /// <param name="this">当前流</param>
    /// <param name="byteArray">要写入的字节数组</param>
    /// <returns></returns>
    public static Task WriteAsync(this Stream @this,byte[] byteArray)
    {
        return @this.WriteAsync(byteArray,0,byteArray.Length);
    }

    public static Stream Append(this Stream @this,Stream stream)
    {
        @this.Write(stream.ToByteArray());
        return @this;
    }

    public static async Task<Stream> AppendAsync(this Stream @this,Stream stream)
    {
        await @this.WriteAsync(await stream.ToByteArrayAsync());
        return @this;
    }

    public static byte[] ToByteArray(this Stream @this)
    {
        if(@this is MemoryStream ms0)
        {
            return ms0.ToArray();
        }

        using var ms=new MemoryStream();
        @this.CopyTo(ms);
        return ms.ToArray();
    }

    public static async Task<byte[]> ToByteArrayAsync(this Stream @this)
    {
        if (@this is MemoryStream ms0)
            return ms0.ToArray();

        using var ms=new MemoryStream();
        await @this.CopyToAsync(ms);
        return ms.ToArray();
    }

    public static string ReadToEnd(this Stream @this)
    {
        using var sr=new StreamReader(@this,Encoding.UTF8);
        return sr.ReadToEnd();
    }

    public static string ReadToEnd(this Stream @this,Encoding encoding)
    {
        using var sr=new StreamReader(@this,encoding);
        return sr.ReadToEnd();
    }

    public static async Task<string> ReadToEndAsync(this Stream @this)
    {
        using var sr = new StreamReader(@this, Encoding.UTF8);
        return await sr.ReadToEndAsync();
    }

    public static async Task<string> ReadToEndAsync(this Stream @this,Encoding encoding)
    {
        using var sr=new StreamReader(@this,encoding);
        return await sr.ReadToEndAsync();
    }
}
