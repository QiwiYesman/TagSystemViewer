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
    private FileExtensions _contentType =FileExtensions.Unknown;
    
    public static readonly DirectProperty<UniImageControl, Uri?> SourceProperty = 
        AvaloniaProperty.RegisterDirect<UniImageControl, Uri?>("Source",
            o => o.Source,
            (o, v) => o.Source = v);

    public static readonly DirectProperty<UniImageControl, FileExtensions> ContentTypeProperty =
        AvaloniaProperty.RegisterDirect<UniImageControl, FileExtensions>("ContentType",
            o => o.ContentType,
            (o, v) => o.ContentType = v);
    
    public static readonly DirectProperty<UniImageControl, bool> ToPlayProperty =
        AvaloniaProperty.RegisterDirect<UniImageControl, bool>("ToPlay",
            o => o.ToPlay,
            (o, v) => o.ToPlay = v);
    public FileExtensions ContentType
    { 
        get => _contentType;
        set
        {
            SetAndRaise(ContentTypeProperty, ref _contentType, value);
            UpdateContent();
        }
    }
    public bool ToPlay
    { 
        get => _play;
        set
        {
            SetAndRaise(ToPlayProperty, ref _play, value);
            if (ContentType != FileExtensions.Gif) return;
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
        set => SetAndRaise(SourceProperty, ref _source, value);
    }

    public void SetErrorImage()
    {
        Content = new Image()
        {
            Source = new Bitmap(ExtensionToAsset.OpenThumbnail(FileExtensions.Error))
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
        try
        {
            switch (ContentType)
            {
                case FileExtensions.Gif:
                    Content = GetGifImage();
                    break;
                case FileExtensions.Image:
                    Content = new AdvancedImage((Uri?)null) { Source = Source.OriginalString };
                    break;
                default:
                    Content = new AdvancedImage((Uri?)null){Source=ExtensionToAsset.GetFileName(ContentType)};
                    break;
            }
        }
        catch (Exception e)
        {
            SetErrorImage();
        }
    }
}