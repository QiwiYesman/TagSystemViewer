using System.Collections.Generic;
using System.Linq;
using TagSystemViewer.Utility;

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
        var dict = new Dictionary<string, DatabaseConfig>()
        {
            [CurrentName] = this
        };
        Serializer.SerializeToFileJson(dict, path);
    }

    public static DatabaseConfig FromFile(string path)
    {
        var dict =Serializer.DeserializeFromFileJson<Dictionary<string, DatabaseConfig>>(path);
        if (dict is null) return new();
        var config = dict.Values.First();
        config.CurrentName = dict.Keys.First();
        return config;
    }
    public bool IsEmpty => Count == 0;
}