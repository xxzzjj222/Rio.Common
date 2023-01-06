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

    public static string ReadToEnd(this Stream @this,Encoding encoding)
    {
        using var sr=new StreamReader(@this,encoding);
        return sr.ReadToEnd();
    }
}
