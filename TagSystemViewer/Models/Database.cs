using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace TagSystemViewer.Models;

public static class Database
{

    public static string DbPath(string dbName="db.db") => 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../",
            "Database", dbName);
    
    
    public static SQLiteConnection DbNewConnection(string dbName="db.db")
    {
        var path = DbPath(dbName);
        File.Open(path, FileMode.Truncate);
        return new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex| SQLiteOpenFlags.ReadWrite);
    }
    
    public static SQLiteConnection DbExistingConnection(string dbName="db.db") => 
        new SQLiteConnection(DbPath((dbName)), SQLiteOpenFlags.ReadWrite);

    public static TableQuery<T> SelectAll<T>(this SQLiteConnection db) where T : new() 
        => db.Table<T>();

    public static void CreateTables(this SQLiteConnection db)
    {
        db.CreateTable<Tag>();
        db.CreateTable<Url>();
        db.CreateTable<TaggedUrl>();
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
        //return db.GetAllWithChildren<Url>() ;
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

public static class SelectTaggedUrl
{
    public static string SelectQueryBase =>
        "select u.Id, u.Link from Urls u " +
        "join TaggedUrls tu on u.Id = tu.UrlId " +
        "join Tags t on tu.TagId = t.Id " +
        "where {0} " +
        "group by u.Id";

    public static string SelectNotTags =>
        "u.Id not in " +
        " (select UrlId from TaggedUrls " +
        " where TagId in ({0})) ";

    public static string SelectOrTags =>
        "u.Id in (select UrlId from TaggedUrls" +
        " where TagId in ({0})) ";

    public static string SelectAndTags =>
        "tu.UrlId in (select tu2.UrlId from TaggedUrls tu2" +
        " where tu2.TagId in ({0}) " +
        " group by tu2.UrlId having count(*) = {1}) ";

    public static string FormatBigQueries(List<Tag> tags, string query)
        => string.Format(query, ListToString(tags), tags.Count);
        
    public static string FormatSimpleQueries(List<Tag> tags, string query)
        => string.Format(query, ListToString(tags));
    public static string ListToString(IEnumerable<Tag> tags) =>
        string.Join(", ", tags.Select(tag => $"'{tag.Id}'"));

    public static string Query(List<Tag> andTags, List<Tag> orTags, List<Tag> notTags)
    {
        List<string> partialQueries = new();
        if(andTags.Any()) partialQueries.Add(FormatBigQueries(andTags, SelectAndTags));
        if(orTags.Any()) partialQueries.Add(FormatSimpleQueries(orTags, SelectOrTags));
        if(notTags.Any()) partialQueries.Add(FormatSimpleQueries(notTags, SelectNotTags));
        if (!partialQueries.Any()) return "select * from Urls";
        return string.Format(SelectQueryBase, string.Join(" and ", partialQueries));
    }
}