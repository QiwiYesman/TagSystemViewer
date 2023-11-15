using System.Collections.Generic;
using System.Linq;

namespace TagSystemViewer.Models;

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