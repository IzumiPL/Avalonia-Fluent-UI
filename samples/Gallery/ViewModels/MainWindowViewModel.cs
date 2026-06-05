using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Home => LocalizationService.Instance.GetString("Home");
    public string Icon => LocalizationService.Instance.GetString("Icon");
    public string BasicInput => LocalizationService.Instance.GetString("BasicInput");
    public string DialogAndPopup => LocalizationService.Instance.GetString("DialogAndPopup");
    public string Layout => LocalizationService.Instance.GetString("Layout");
    public string Navigation => LocalizationService.Instance.GetString("Navigation");
    public string Text => LocalizationService.Instance.GetString("Text");
    public string View => LocalizationService.Instance.GetString("View");
    public string Scroll => LocalizationService.Instance.GetString("Scroll");
    public string StatusAndInformation => LocalizationService.Instance.GetString("StatusAndInformation");
    public string MenuAndToolBar => LocalizationService.Instance.GetString("MenuAndToolBar");
    public string DateTime => LocalizationService.Instance.GetString("DateTime");
    public string Title => LocalizationService.Instance.GetString("MV_Title");
    public string SearchWatermark => LocalizationService.Instance.GetString("MV_SearchWatermark");
    
    private readonly List<string> _history = new();

    private readonly Dictionary<string, Func<ViewModelBase>> _viewModelFactories;
    private readonly Dictionary<string, ViewModelBase> _viewModels = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    private bool _canGoBack;

    [ObservableProperty]
    private object _navigationViewSelectedItem;

    partial void OnNavigationViewSelectedItemChanged(object value)
    {
        if (value is AvaloniaFluentUI.Controls.NavigationViewItem item)
        {
            TogglePage(item.Tag + "");
        }
    }

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;
    
    private AppConfig? _config;

    public MainWindowViewModel(AppConfig? config)
    {
#if DEBUG
        Debug.WriteLine("MainWindowViewModel Init");
#endif

        var homeVm = new HomeViewModel();
        homeVm.GotoControlEvent += (page, name) =>
        {
            TogglePage(page);
            WeakReferenceMessenger.Default.Send(new JumpToControlMessage(page, name));
        };
        _viewModels["Home"] = homeVm;
        _homeViewModel = homeVm;

        _viewModelFactories = new Dictionary<string, Func<ViewModelBase>>
        {
            { "Icons", () => new IconsViewModel() },
            { "BasicInput", () => new BasicInputViewModel() },
            { "DialogBoxAndPopup", () => new DialogBoxAndPopupViewModel() },
            { "Layout", () => new LayoutViewModel() },
            { "Navigation", () => new NavigationViewModel() },
            { "Text", () => new TextViewModel() },
            { "View", () => new ViewModel() },
            { "Scroll", () => new ScrollViewModel() },
            { "StatusAndInformation", () => new StatusAndInformationViewModel() },
            { "MenuAndToolBar", () => new MenuAndToolBarViewModel() },
            { "DateTime", () => new DateTimeViewModel() },
            { "Settings", () => new SettingsViewModel(config) },
        };
        
        _config = config;
        CurrentViewModel = _viewModels["Home"];


        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Home));
        OnPropertyChanged(nameof(Icon));
        OnPropertyChanged(nameof(BasicInput));
        OnPropertyChanged(nameof(DialogAndPopup));
        OnPropertyChanged(nameof(Layout));
        OnPropertyChanged(nameof(Navigation));
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(View));
        OnPropertyChanged(nameof(Scroll));
        OnPropertyChanged(nameof(StatusAndInformation));
        OnPropertyChanged(nameof(MenuAndToolBar));
        OnPropertyChanged(nameof(DateTime));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(SearchWatermark));
    }

    private ViewModelBase GetOrCreateViewModel(string key)
    {
        if (_viewModels.TryGetValue(key, out var vm))
            return vm;

        if (_viewModelFactories.TryGetValue(key, out var factory))
        {
            vm = factory();
            _viewModels[key] = vm;
            return vm;
        }

        throw new KeyNotFoundException($"ViewModel not found for key: {key}");
    }

    private HomeViewModel _homeViewModel;

    public SettingsViewModel SettingsViewModel
    {
        get
        {
            if (!_viewModels.TryGetValue("Settings", out var vm))
            {
                vm = new SettingsViewModel(_config);
                _viewModels["Settings"] = vm;
            }
            return (SettingsViewModel)vm;
        }
    }

    [RelayCommand]
    private void ToggleTheme() => FluentAvaloniaTheme.Instance.ToggleTheme(); 

    [RelayCommand]
    private void TogglePage(string page)
    {
        ViewModelBase target;
        try
        {
            target = GetOrCreateViewModel(page);
        }
        catch (KeyNotFoundException)
        {
            return;
        }

        if (target == CurrentViewModel)
            return;

        if (CurrentViewModel is HomeViewModel homeVm && CurrentViewModel != target)
        {
            homeVm.ReleaseImages();
        }

        if (CurrentViewModel != null)
        {
            var currentPageKey = GetKeyByViewModel(CurrentViewModel);
            if (currentPageKey != null)
            {
                _history.Add(currentPageKey);
            }
        }

        CurrentViewModel = target;
        CanGoBack = _history.Count > 0;

#if DEBUG
        Debug.WriteLine($"Toggle Page To: {target}");
#endif
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack()
    {
        if (_history.Count <= 0)
            return;

        Console.WriteLine("Go Back");

        var last = _history[^1];
        _history.RemoveAt(_history.Count - 1);

        if (GetOrCreateViewModel(last) is { } view)
        {
            CurrentViewModel = view;
        }

        WeakReferenceMessenger.Default.Send(new JumpToControlMessage(last, null));

        CanGoBack = _history.Count > 0;
    }

    private string? GetKeyByViewModel(ViewModelBase vm)
    {
        foreach (var kvp in _viewModels)
        {
            if (kvp.Value == vm) return kvp.Key;
        }
        return null;
    }
}
