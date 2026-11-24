using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

[PseudoClasses(PC_PRESSED)]
public class HyperlinkLabel : TextBlock 
{
    public static readonly StyledProperty<Uri?> UriProperty =
        AvaloniaProperty.Register<HyperlinkLabel, Uri?>(nameof(Uri));

    public Uri? Uri
    {
        get => GetValue(UriProperty);
        set => SetValue(UriProperty, value);
    }

    private const string PC_PRESSED = ":pressed";
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PseudoClasses.Set(PC_PRESSED, true);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Set(PC_PRESSED, false);
        var uri = Uri;
        if (uri == null) { return; }
        
        Point point = e.GetPosition(this);
        if (new Rect(Bounds.Size).Contains(point))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                await TopLevel.GetTopLevel(this).Launcher.LaunchUriAsync(uri);
            });
        }   
    }
}

