using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace TagSystemViewer.Models;

[Table("Tags")]
public class Tag
{
    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    
    [Unique, NotNull]
    public string Name {get;set;}
    
    [ManyToMany(typeof(TaggedUrl), CascadeOperations = CascadeOperation.CascadeDelete|CascadeOperation.CascadeRead)]
    public List<Url> Urls { get; set; } = new();

    public override string ToString() => Name;
    
}

[Table("Urls")]
public class Url
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Unique, NotNull]
    public string Link { get; set; }
    
    public DateTime DateAdded { get; set; } = DateTime.Now;

    [ManyToMany(typeof(TaggedUrl), CascadeOperations = CascadeOperation.CascadeDelete|CascadeOperation.CascadeRead)]
    public List<Tag> Tags { get; set; } = new();
    
    public override string ToString() => Link;

}

[Table("TaggedUrls")]
public class TaggedUrl
{
    [ForeignKey(typeof(Url))]
    public int UrlId { get; set; }

    [ForeignKey(typeof(Tag))]
    public int TagId { get; set; }
}


