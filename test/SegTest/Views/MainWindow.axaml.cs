using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;

namespace SegTest.Views;

public partial class MainWindow : FluentWindow 
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }
}
