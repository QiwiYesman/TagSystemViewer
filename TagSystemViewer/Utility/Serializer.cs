using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace TagSystemViewer.Utility;

public static class Serializer
{
    public static void SerializeToFileJson(object obj, string filePath)
    {
        using var fileWriter = new StreamWriter(filePath);
        var serializer = new JsonSerializer();
        serializer.Serialize(fileWriter, obj);
    }
    
    public static T? DeserializeFromFileJson<T>(string filePath) where T : class
    {
        using var fileReader = new StreamReader(filePath);
        var serializer = new JsonSerializer();
        return serializer.Deserialize(fileReader, typeof(T)) as T;
    }
}