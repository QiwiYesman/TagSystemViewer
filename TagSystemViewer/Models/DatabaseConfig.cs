using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TagSystemViewer.Models;

public class DatabaseConfig : Dictionary<string ,string>
{
    public string CurrentName { get; set; } = "";
    public string? CurrentPath
    {
        get
        {
            TryGetValue(CurrentName, out var path);
            return path;
        }
    }
    
    
    public void Save(string path)
    {
        var json = JsonConvert.SerializeObject((CurrentName, this),
            new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.Auto});
        File.WriteAllText(path, json);
    }

    public static DatabaseConfig FromJsonFile(string path)
    {
        using StreamReader reader = new(path);
        var json = reader.ReadToEnd();
        var (item1, config) = JsonConvert.DeserializeObject<(string, DatabaseConfig)>(json,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        config.CurrentName = item1;
        return config;
    }
    
    public bool IsEmpty => Count == 0;
}