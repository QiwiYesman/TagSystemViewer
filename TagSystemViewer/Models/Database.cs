using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
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

    public static void CreateTables(this SQLiteAsyncConnection db)
    {
        db.CreateTableAsync<Tag>();
        db.CreateTableAsync<Url>();
        db.CreateTableAsync<TaggedUrl>();
        db.AddDefaultTags();
    }

    public static void AddDefaultTags(this SQLiteAsyncConnection db)
    {
        var tagNames = DefaultTagsAndExtensions.TagNames;
        var tags = tagNames.Select(x => new Tag { Id = 0, Name = x });
        db.InsertAllAsync(tags, typeof(Tag));
    }

    public static async Task<List<Url>> SelectUrls(this SQLiteAsyncConnection db, TagGroups tags)
    {
       var onlyUrlsIds = await SelectOnlyUrls(db, 
           tags.AndTags.ToList(), tags.OrTags.ToList(), tags.NotTags.ToList());
       var urlsWithTags = await SelectUrlsWithTags(db, onlyUrlsIds);
       return urlsWithTags;
    }
    
    public static async Task<List<Url>> SelectUrlsWithTags(this SQLiteAsyncConnection db, List<Url> ids)
    {
        List<Url> urls = new();
        try
        {
            foreach (var urlWithIdOnly in ids)
            {
                var fullUrl = await db.GetWithChildrenAsync<Url>(urlWithIdOnly.Id);
                urls.Add(fullUrl);
            }

            return urls;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return urls;
        }
    }

    public static async Task<List<Url>> SelectOnlyUrls(this SQLiteAsyncConnection db,
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
