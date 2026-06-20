using Avalonia.Controls;
using AvaloniaFluentUI.Media.Animation;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class FrameViewPage : ViewBase
{
    public FrameViewPage()
    {
        InitializeComponent();
    }
    
    private void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;
        if (listBox == null) return;
        
        switch (listBox.SelectedIndex)
        {
            case 0:
                Frame.Navigate(typeof(FramePage1));
                break;

            case 1:
                Frame.Navigate(typeof(FramePage2), null, new SlideNavigationTransitionInfo());
                break;

            case 2:
                Frame.Navigate(typeof(FramePage3), null, new DrillInNavigationTransitionInfo());
                break;

            case 3:
                Frame.Navigate(typeof(FramePage4), null, new SuppressNavigationTransitionInfo());
                break;
        }
    }
}
