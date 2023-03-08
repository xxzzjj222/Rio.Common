
using Rio.Extensions;

namespace Rio.Common.Compressor;

public interface IDataCompressor
{
    byte[] Compress(byte[] sourceData);

    Task<byte[]> CompressAsync(byte[] sourceData);

    byte[] Decompress(byte[] compressData);

    Task<byte[]> DecompressAsync(byte[] compressData);
}

public sealed class NullDataCompressor : IDataCompressor
{
    public byte[] Compress(byte[] sourceData)
    {
        return sourceData;
    }

    public Task<byte[]> CompressAsync(byte[] sourceData)
    {
        return Task.FromResult(sourceData);
    }

    public byte[] Decompress(byte[] compressData)
    {
        return compressData;
    }

    public Task<byte[]> DecompressAsync(byte[] compressData)
    {
        return Task.FromResult(compressData);
    }
}

public class GZipDataCompressor : IDataCompressor
{
    public byte[] Compress(byte[] sourceData) => sourceData.CompressGZip();

    public async Task<byte[]> CompressAsync(byte[] sourceData) => await sourceData.CompressGZipAsync();

    public byte[] Decompress(byte[] compressData) => compressData.DecompressGzip();

    public async Task<byte[]> DecompressAsync(byte[] compressData) => await compressData.DecompressGzipAsync();
}