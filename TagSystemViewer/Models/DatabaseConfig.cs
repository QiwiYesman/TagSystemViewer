using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

    public bool IsAccessible(string dbName)
    {
        TryGetValue(dbName, out var path);
        return path is not null && File.Exists(path);
    }

    public bool IsAccessibleCurrent()
    {
        return IsAccessible(CurrentName);
    }
    public bool PickCorrectPath()
    {
        foreach (var dbName in Keys)
        {
            if (!IsAccessible(dbName)) continue;
            CurrentName = dbName;
            return true;
        }

        return false;
    }

    public async Task CreateDefault()
    {
        string defaultDbName = "default";
        string defaultDbPath = "db_default.sqlite3";
        var conn = Database.DbNewConnection(defaultDbPath);
        await Database.CreateTables(conn);
        Add(defaultDbName, defaultDbPath);
        CurrentName = defaultDbName;
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