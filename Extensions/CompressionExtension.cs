//ReSharper disable once CheckNamespace
using System.IO.Compression;
using System.Text;

namespace Rio.Extensions;

public static class CompressionExtension
{
    /// <summary>
    /// A string extension method than compress the given string to GZip byte array with UTF8 encoding.
    /// </summary>
    /// <param name="this">The string to compress to act on.</param>
    /// <returns>The string compressed into a GZip byte array</returns>
    public static byte[] CompressGZip(this string @this)=> @this.CompressGZip(Encoding.UTF8);

    /// <summary>
    /// A string extension method that compress the given string to GZip byte array.
    /// </summary>
    /// <param name="this">The string to compress to act on.</param>
    /// <param name="encoding">The string compressed into a GZip byte array.</param>
    /// <returns></returns>
    public static byte[] CompressGZip(this string @this, Encoding encoding) => encoding.GetBytes(@this).CompressGZip();

    /// <summary>
    /// A byteArray extension method that compress the given byte array to GZip byte array.
    /// </summary>
    /// <param name="bytes">The byteArray to compress to act on.</param>
    /// <returns>The byteArray compressed into a  GZip byte array.</returns>
    public static byte[] CompressGZip(this byte[] bytes)
    {
        using var memoryStream=new MemoryStream();
        using(var zipStream=new GZipStream(memoryStream,CompressionMode.Compress))
        {
            zipStream.Write(bytes);
        }
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> CompressGZipAsync(this byte[] bytes)
    {
        using var memoryStream = new MemoryStream();
        using(var zipStream=new GZipStream(memoryStream,CompressionMode.Compress))
        {
            await zipStream.WriteAsync(bytes);
        }
        return memoryStream.ToArray();
    }

    public static byte[] CompressGzip(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        using(var zipStream=new GZipStream(memoryStream,CompressionMode.Compress))
        {
            stream.CopyTo(zipStream);
        }
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> CompressGZipAsync(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            await stream.CopyToAsync(zipStream);
        }
        return memoryStream.ToArray();
    }

    /// <summary>
    /// A bytep[] extension method that decompress the gzip byte array to byte array.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The gzip byte array to byte array.</returns>
    public static byte[] DecompressGzip(this byte[] @this)
    {
        using var memoryStream = new MemoryStream(@this);
        return memoryStream.DecompressGZip();
    }

    public static byte[] DecompressGZip(this Stream stream)
    {
        using var outStream = new MemoryStream();
        using(var zipStream=new GZipStream(stream,CompressionMode.Decompress))
        {
            zipStream.CopyTo(outStream);
        }
        return outStream.GetBuffer();
    }

    public static async Task<byte[]> DecompressGzipAsync(this byte[] @this)
    {
        using var memoryStream=new MemoryStream(@this);
        return await memoryStream.DecompressGZipAsync();
    }

    public static async Task<byte[]> DecompressGZipAsync(this Stream stream)
    {
        using var outStream = new MemoryStream();
        using(var zipStream=new GZipStream(stream,CompressionMode.Decompress))
        {
            await zipStream.CopyToAsync(outStream);
        }
        return outStream.GetBuffer();
    }

    public static string CompressGZipString(this byte[] bytes)=>bytes.CompressGZipString(Encoding.UTF8);

    public static string CompressGZipString(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes.CompressGZip());

    public static string DecompressGZipString(this byte[] bytes)=>bytes.DecompressGZipString(Encoding.UTF8);

    public static string DecompressGZipString(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes.DecompressGzip());

    /// <summary>
    /// A FileInfo extension method that create a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    public static void CreateGZip(this FileInfo @this)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(@this.FullName + ".gz" );
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    /// A FileInfo extension method that create a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the zip.</param>
    public static void CreateGZip(this FileInfo @this,string destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    /// A FileInfo extension method that create a zip file.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for th zip.</param>
    public static void CreateGZip(this FileInfo @this,FileInfo destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var compressedFileStream = File.Create(destination.FullName);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    /// <summary>
    /// A FileInfo extension method that extracts the gzip to directory described by @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    public static void ExtractGZipToDirectory(this FileInfo @this)
    {
        using var originalFileStream= @this.OpenRead();
        var newFileName = Path.GetFileNameWithoutExtension(@this.FullName);

        using var decompressStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
        using var decompressFileStream = File.Create(newFileName);
        decompressStream.CopyTo(decompressFileStream);
    }

    /// <summary>
    /// A FileInfo extension method that extracts the gzip to directory described by @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the.</param>
    public static void ExtractGZipToDirectory(this FileInfo @this,string destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var decompressStream=new GZipStream(originalFileStream,CompressionMode.Decompress);
        using var decompressFileStream=File.Create(destination);
        decompressStream.CopyTo(decompressFileStream);
    }

    /// <summary>
    /// A FileInfo extension method than extracts the gzip to directory described by @this.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destination">Destination for the.</param>
    public static void ExtractGZipToDirectory(this FileInfo @this,FileInfo destination)
    {
        using var originalFileStream = @this.OpenRead();
        using var decompressStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
        using var decompressFileStream=File.Create(destination.FullName);
        decompressStream.CopyTo(decompressFileStream);
    }

    /// <summary>
    /// Open a zip archive at the specified path and in the specified mode.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="mode">One of the enumeration values that specifies the action than are allowed on the entries in the opened archive.</param>
    /// <returns></returns>
    public static ZipArchive OpenZipFile(this FileInfo @this,ZipArchiveMode mode)
    {
        return ZipFile.Open(@this.FullName,mode);
    }

    public static ZipArchive OpenZipFile(this FileInfo @this,ZipArchiveMode mode,Encoding entryNameEncoding)
    {
        return ZipFile.Open(@this.FullName,mode, entryNameEncoding);
    }

    public static ZipArchive OpenReadFile(this FileInfo @this)
    {
        return ZipFile.OpenRead(@this.FullName);
    }

    public static void ExtractZipFileToDirectory(this FileInfo @this,string destinationDirectoryName)
    {
        ZipFile.ExtractToDirectory(@this.FullName,destinationDirectoryName);
    }

    public static void ExtractZipFileToDirectory(this FileInfo @this,string destinationDirectoryName,Encoding entryNameEncoding)
    {
        ZipFile.ExtractToDirectory(@this.FullName,destinationDirectoryName,entryNameEncoding);
    }

    public static void ExtractZipFileToDirectory(this FileInfo @this,DirectoryInfo destinationDirectory)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectory.FullName);
    }

    public static void ExtractZipFileToDirectory(this FileInfo @this,DirectoryInfo destinationDirectory,Encoding entryNameEncoding)
    {
        ZipFile.ExtractToDirectory(@this.FullName, destinationDirectory.FullName, entryNameEncoding);
    }

    /// <summary>
    /// Create a zip archive that contains the files and directory from the sepcified.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="destinationArchiveFileName">The path to of the archive to be created, specified as a relative or absoulte path, a relative path is interpreted as relative to the current working directory.</param>
    public static void CreateZipFile(this DirectoryInfo @this,string destinationArchiveFileName)
    {
        ZipFile.CreateFromDirectory(@this.FullName,destinationArchiveFileName);
    }

    public static void CreateZipFile(this DirectoryInfo @this, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFileName, compressionLevel, includeBaseDirectory);
    }

    public static void CreateZipFile(this DirectoryInfo @this,string destinationArchiveFileName,CompressionLevel compressionLevel,bool includeBaseDirectory,Encoding entryNameEncoding)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFileName,compressionLevel,includeBaseDirectory,entryNameEncoding);
    }

    public static void CreateZipFile(this DirectoryInfo @this,FileInfo destinationArchiveFile)
    {
        ZipFile.CreateFromDirectory(@this.FullName,destinationArchiveFile.FullName);
    }

    public static void CreateZipFile(this DirectoryInfo @this,FileInfo destinationArchiveFile,CompressionLevel compressionLevel,bool includeBaseDirectory)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFile.FullName, compressionLevel, includeBaseDirectory);
    }

    public static void CreateZipFile(this DirectoryInfo @this,FileInfo destinationArchiveFile,CompressionLevel compressionLevel,bool includeBaseDirectory,Encoding entryNameEncoding)
    {
        ZipFile.CreateFromDirectory(@this.FullName, destinationArchiveFile.FullName, compressionLevel, includeBaseDirectory, entryNameEncoding);
    }
}

