using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace TagSystemViewer.Models;



public static class Database
{

    public static SQLiteConnection DbNewConnection(string dbPath) =>
        new(dbPath, SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.FullMutex |
                    SQLiteOpenFlags.ReadWrite);

    public static SQLiteConnection DbExistingConnection(string dbPath) => 
        new (dbPath, SQLiteOpenFlags.ReadWrite);

    public static SQLiteConnection? CurrentConnection(DatabaseConfig config) =>
       config.CurrentPath is null ? null : DbExistingConnection(config.CurrentPath);

    public static TableQuery<T> SelectAll<T>(this SQLiteConnection db) where T : new() 
        => db.Table<T>();

    public static void CreateTables(this SQLiteConnection db)
    {
        db.CreateTable<Tag>();
        db.CreateTable<Url>();
        db.CreateTable<TaggedUrl>();
        db.AddDefaultTags();
    }

    public static void AddDefaultTags(this SQLiteConnection db)
    {
        var tagNames = DefaultTagsAndExtensions.TagNames;
        var tags = tagNames.Select(x => new Tag() { Id = 0, Name = x });
        db.InsertAll(tags, typeof(Tag));
    }

    public static List<Url> SelectUrls(this SQLiteConnection db, TagGroups tags)
    {
       var onlyUrlsIds = SelectOnlyUrls(db, 
           tags.AndTags.ToList(), tags.OrTags.ToList(), tags.NotTags.ToList());
       var urlsWithTags = SelectUrlsWithTags(db, onlyUrlsIds);
       return urlsWithTags;
    }

    public static List<Url> SelectUrlsWithTags(this SQLiteConnection db, List<Url> ids)
    {
        try
        {
            return ids.Select(id => db.GetWithChildren<Url>(id.Id, true)).ToList();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return new();
        }
    }

    public static List<Url> SelectOnlyUrls(this SQLiteConnection db,
        List<Tag> andTags, List<Tag> orTags, List<Tag> notTags)
    {
        var query = SelectTaggedUrl.Query(andTags, orTags, notTags);
        return query == "" ? new() : db.Query<Url>(query);
    }
    
}

public class TagGroups
{
    public IEnumerable<Tag> AndTags { get; set; }
    public IEnumerable<Tag> OrTags { get; set; }
    public IEnumerable<Tag> NotTags { get; set; }
}
