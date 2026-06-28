using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Test.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Dictionary<string, ViewModelBase> _viewModels;

    [ObservableProperty]
    private SegmentedItem _currentItem;

    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        _viewModels = new Dictionary<string, ViewModelBase>
        {
            { "Main", MainViewModel },
            { "FlipView", FlipViewModel },
            { "Navi", NaviViewModel },
            { "Card", CardViewModel },
            {"InfoBar", InfoBarViewModel},
            {"Flyout", FlyoutViewModel}
        };

        CurrentViewModel = MainViewModel;
    }

    private MainViewModel MainViewModel { get; } = new();
    private FlipViewModel FlipViewModel { get; } = new();
    private NaviViewModel NaviViewModel { get; } = new();
    private CardViewModel CardViewModel { get; } = new();
    private InfoBarViewModel InfoBarViewModel { get; } = new();
    private FlyoutViewModel FlyoutViewModel { get; } = new();

    private int Count { get; set; } = 0;

    [ObservableProperty]
    private string _currentLanguage = "zh-CN";

    public string[] LanguageItems => ["zh-CN", "ja-JP", "en-US"];

    partial void OnCurrentLanguageChanged(string value)
    {
        if (value != LocalizationService.Instance.CurrentLanguage)
        {
            LocalizationService.Instance.SetCulture(value);
        }
    }

    [ObservableProperty]
    private int _currentIndex = 0;

    [RelayCommand]
    private void OnSearch(string value)
    {
        if (String.IsNullOrWhiteSpace(value)) { return; }
        var rs = value.Split("|");
        if (rs.Length != 3) { return; }
        
        if (!int.TryParse(rs[1], out var count)) { return; }
        if (!int.TryParse(rs[2], out var time)) { return; }
        
        if (rs[0] == "Invoke")
        {
            Dispatcher.UIThread.Post(async () =>
            {
            for (int i = 0; i < count; i++)
            {
                TogglePage(Count % 2 == 0 ? "Button" : "Card");
                CurrentIndex = Count % 2 == 0 ? 3 : 4;
                await Task.Delay(time);
                Count++;
            }
            });
        }
    }

    partial void OnCurrentItemChanged(SegmentedItem value)
    {
        var page = value.Tag;
        TogglePage($"{page}");
    }

    private void TogglePage(string page)
    {
        if (_viewModels.TryGetValue(page, out var viewModel) && CurrentViewModel != viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }
}
