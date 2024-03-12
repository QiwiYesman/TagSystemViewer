using System;
using System.Threading.Tasks;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using AvaloniaGif;
using TagSystemViewer.Enums;
using TagSystemViewer.Utility;

namespace TagSystemViewer.Views.Controls;

public class UniImageControl: UserControl
{
    private Uri? _source;
    private bool _play = false;
    
    public static readonly DirectProperty<UniImageControl, Uri?> SourceProperty = 
        AvaloniaProperty.RegisterDirect<UniImageControl, Uri?>("Source",
            o => o.Source,
            (o, v) => o.Source = v);
    
    public static readonly DirectProperty<UniImageControl, bool> ToPlayProperty =
        AvaloniaProperty.RegisterDirect<UniImageControl, bool>("ToPlay",
            o => o.ToPlay,
            (o, v) => o.ToPlay = v);

    public bool ToPlay
    { 
        get => _play;
        set
        {
            SetAndRaise(ToPlayProperty, ref _play, value);
            if (Content is not GifImage gif) return;
            if (_play)
            {
                gif.Start();
                return;
            }

            try
            {
                gif.Stop();
            }
            catch
            {
                return;
            }

        }
    }
    public Uri? Source 
    { 
        get => _source;
        set 
        {
            SetAndRaise(SourceProperty, ref _source, value);
            UpdateContent();
        }
    }

    public void SetErrorImage()
    {
        Content = new Image()
        {
            Source = new Bitmap(UriToAsset.OpenThumbnailByExtension("error"))
        };
    }

    public GifImage GetGifImage()
    {
        var image = new GifImage() { SourceUri = Source, AutoStart = true};
        image.Error += SetErrorImage;
        image.AutoStart = false;
        return image;
    }
    
    public void UpdateContent()
    {
        if (Source is null) return ;
        var extension = UriToAsset.GetExtension(Source.OriginalString);
        var type = UriToAsset.GetAssetName(extension);
        try
        {
            switch (type)
            {
                case "gif":
                    Content = GetGifImage();
                    break;
                case "image":
                    Content = new AdvancedImage((Uri?)null) { Source = Source.OriginalString };
                    break;
                default:
                    Content = new AdvancedImage((Uri?)null){Source=UriToAsset.GetAssetPath(type)};
                    break;
            }
        }
        catch (Exception e)
        {
            SetErrorImage();
        }
    }
}