using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace TagSystemViewer.Models;

public static class Database
{

    public static SQLiteAsyncConnection DbNewConnection(string dbPath) =>
        new(dbPath, SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.FullMutex |
                    SQLiteOpenFlags.ReadWrite);

    public static SQLiteAsyncConnection DbExistingConnection(string dbPath) => 
        new (dbPath, SQLiteOpenFlags.ReadWrite);

    public static SQLiteAsyncConnection? CurrentConnection(DatabaseConfig config) =>
       config.CurrentPath is null ? null : DbExistingConnection(config.CurrentPath);

    public static AsyncTableQuery<T> SelectAll<T>(this SQLiteAsyncConnection db) where T : new() 
        => db.Table<T>();

    public static async Task CreateTables(SQLiteAsyncConnection db)
    {
        await db.CreateTableAsync<Tag>();
        await db.CreateTableAsync<Url>();
        await db.CreateTableAsync<TaggedUrl>();
        await AddDefaultTags(db);
    }

    public static async Task AddDefaultTags(SQLiteAsyncConnection db)
    {
        IEnumerable<string> tagNames = ["image", "video", "hoho"];
        var tags = tagNames.Select(x => new Tag() { Id = 0, Name = x });
        await db.InsertAllAsync(tags, typeof(Tag));
    }

    public static async Task<List<Url>> SelectUrls(SQLiteAsyncConnection db, TagGroups tags)
    {
       var onlyUrlsIds = await SelectOnlyUrls(db, 
           tags.AndTags.ToList(), tags.OrTags.ToList(), tags.NotTags.ToList());
       //var urlsWithTags = SelectUrlsWithTags(db, onlyUrlsIds);
       return onlyUrlsIds;
    }
/*
    public static List<Task<Url>> SelectUrlsWithTags(SQLiteAsyncConnection db, List<Url> ids)
    {
        try
        {
            return ids.Select(async id => await db.GetWithChildrenAsync<Url>(id.Id, true)).ToList();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return new();
        }
    }
*/
    public static async Task<List<Url>> SelectOnlyUrls(SQLiteAsyncConnection db,
        List<Tag> andTags, List<Tag> orTags, List<Tag> notTags)
    {
        var query = SelectTaggedUrl.Query(andTags, orTags, notTags);
        return query == "" ? new() : await db.QueryAsync<Url>(query);
    }
    
}

public class TagGroups
{
    public IEnumerable<Tag> AndTags { get; set; }
    public IEnumerable<Tag> OrTags { get; set; }
    public IEnumerable<Tag> NotTags { get; set; }
}
