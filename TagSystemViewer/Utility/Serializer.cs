using System.IO;
using Newtonsoft.Json;

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