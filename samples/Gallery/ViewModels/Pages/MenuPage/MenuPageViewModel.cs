using System.ComponentModel;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class MenuPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Menu");
    
    [ObservableProperty]
    private string _commandText = "NULL";

    [RelayCommand]
    private void OnClickedMenuItem(string value) => CommandText = value;
}
