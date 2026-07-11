using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using AvaloniaFluentUI.Controls;

namespace Gallery.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    
    private void OnPopupAvatarFlyout(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Avatar ct)
        {
            FlyoutBase.ShowAttachedFlyout(ct);
        }
    }

    private void OnPopupContextMenu(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Panel panel)
        {
            panel.ContextMenu?.Open();
        }
    }
}
