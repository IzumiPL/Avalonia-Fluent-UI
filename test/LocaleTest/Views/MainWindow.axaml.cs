using AvaloniaFluentUI.Windowing;
using AvaloniaFluentUI.Locale;

namespace LocaleTest.Views;

public partial class MainWindow : FluentWindow 
{
    public MainWindow()
    {
        InitializeComponent();

        Title = LocalizationService.Instance.GetString("Title");

        LocalizationService.Instance.PropertyChanged += (_, _) =>
        {
            SetValue(TitleProperty, LocalizationService.Instance.GetString("Title"));
        };
       
        // EnableWindowEffect(true);

        MinWidth = 800;
        MinHeight = 400;
    }

    private void OnSearch(string value)
    {
    }
}
