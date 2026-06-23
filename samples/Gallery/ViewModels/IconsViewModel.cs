using System.Collections.Generic;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class IconsViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Icon");

    [ObservableProperty]
    private SegmentedItem _currentItem;

    partial void OnCurrentItemChanged(SegmentedItem value)
    {
        ToggleView($"{value.Tag}");
    }

    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    private readonly Dictionary<string, ViewModelBase> _viewModels;
    
    public IconsViewModel()
    {
        _viewModels = new Dictionary<string, ViewModelBase>
        {
            {"FluentIcon", FluentIconPageViewModel},
            {"FontIcon", FontIconPageViewModel},
            {"SymbolIcon", SymbolIconPageViewModel},
        };

        CurrentViewModel = FluentIconPageViewModel;
    }
    
    private FluentIconPageViewModel FluentIconPageViewModel { get; } =  new FluentIconPageViewModel();
    private FontIconPageViewModel FontIconPageViewModel { get; } =  new FontIconPageViewModel();
    private SymbolIconPageViewModel SymbolIconPageViewModel { get; } =  new SymbolIconPageViewModel();

    private void ToggleView(string page)
    {
        if (_viewModels.TryGetValue(page, out var viewModel) && viewModel != CurrentViewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
