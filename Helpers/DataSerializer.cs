using System.Runtime.CompilerServices;
using Rio.Extensions;
using System.Xml.Serialization;
using Rio.Common.Compressor;

namespace Rio.Common.Helpers;

public interface IDataSerializer
{
    byte[] Serializer<T>(T obj);

    T Deserializer<T>(byte[] bytes);
}

public class XmlDataSerializer:IDataSerializer
{
    internal static Lazy<XmlDataSerializer> _instance = new();

    public T Deserializer<T>(byte[] bytes)
    {
        if(typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }

        using var ms = new MemoryStream(bytes);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(ms)!;
    }

    public byte[] Serializer<T>(T obj)
    {
        if(typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }
        if(obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        using var ms = new MemoryStream();
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(ms, obj);
        return ms.ToArray();
    }
}

public class JsonDataSerializer:IDataSerializer
{
    public T Deserializer<T>(byte[] bytes)
    {
        if(typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }

        return bytes.GetString().JsonToObject<T>()??throw new ArgumentNullException(nameof(bytes));
    }

    public byte[] Serializer<T>(T obj)
    {
        if(typeof(Task).IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException(Resource.TaskCanNotBeSerialized);
        }
        if(obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return obj.ToJson().GetBytes();
    }
}

public sealed class CompressDataSerializer:IDataSerializer
{
    private readonly IDataSerializer _serializer;
    private readonly IDataCompressor _compressor;

    public CompressDataSerializer(IDataSerializer serializer, IDataCompressor compressor)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
    }

    public byte[] Serializer<T>(T obj) => _compressor.Compress(_serializer.Serializer(obj));

    public T Deserializer<T>(byte[] bytes) => _serializer.Deserializer<T>(_compressor.Decompress(bytes));
}


