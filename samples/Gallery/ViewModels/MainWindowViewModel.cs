using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public override string Title => "Avalonia Fluent UI Gallery";
    
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

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"Navigation Item Changed, ItemName: {item.Tag}");
            Console.WriteLine("------------------------------------------------------------");
        }
    }

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;
    
    private readonly AppConfig? _config;

    public MainWindowViewModel(AppConfig? config)
    {
        _viewModels["Home"] = new HomeViewModel();

        JumpService.OnJumpToControl += (_, model) =>
        {
            TogglePage(model.Page);
            WeakReferenceMessenger.Default.Send(new JumpToControlMessage(model.Page, model.ControlName));
        };

        _viewModelFactories = new Dictionary<string, Func<ViewModelBase>>
        {
            { "Icons", () => new IconsViewModel() },
            
            { "BasicInput", () => new BasicInputViewModel() },
            { "Button", () => new ButtonPageViewModel() },
            { "ComboBox", () => new ComboBoxPageViewModel() },
            { "Slider", () => new SlierPageViewModel() },
            
            { "DialogBoxAndPopup", () => new DialogBoxAndPopupViewModel() },
            { "Dialog", () => new DialogPageViewModel() },
            { "Flyout", () => new FlyoutPageViewModel() },
            { "ShortcutKeyPanel", () => new ShortcutKeyPickerPageViewModel() },
            
            { "Layout", () => new LayoutViewModel() },
            { "Border", () => new BorderPageViewModel() },
            { "Panel", () => new PanelPageViewModel() },
            
            { "Navigation", () => new NavigationViewModel() },
            { "NavigationView", () => new NavigationViewPageViewModel() },
            { "Tabs", () => new TabsPageViewModel() },
            { "SegmentedView", () => new SegmentedViewPageViewModel() },
            { "FrameView", () => new FrameViewPageViewModel() },
            { "BreadcrumbBar", () => new BreadcrumbBarPageViewModel() },
            
            { "Text", () => new TextViewModel() },
            { "TextBlock", () => new TextBlockPageViewModel() },
            { "TextBox",  () => new TextBoxPageViewModel() },
            { "NumberBox", () => new SpinBoxPageViewModel()},
            
            { "View", () => new ViewModel() },
            { "List", () => new ListPageViewModel() },
            { "TreeView", () => new TreeViewPageViewModel() },
            { "CarouselView", () => new CarouselViewPageViewModel() },
            { "Card", () => new CardPageViewModel() },
            { "AvatarView", () => new AvatarViewPageViewModel() },
            { "FileDropPicker", () => new FilesDropPickerPageViewModel() },
            
            { "Scroll", () => new ScrollViewModel() },
            
            { "StatusAndInformation", () => new StatusAndInformationViewModel() },
            
            { "MenuAndToolBar", () => new MenuAndToolBarViewModel() },
            { "Menu", () => new MenuPageViewModel() },
            { "ContextMenu", () => new ContextMenuViewModel() },
            { "CommandBar", () => new CommandBarViewPageViewModel() },
            
            { "DateTime", () => new DateTimeViewModel() },
            
            { "Settings", () => new SettingsViewModel(config) },
        };
        
        _config = config;
        CurrentViewModel = _viewModels["Home"];

        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
        AvaloniaFluentTheme.Instance.ThemeChanged += (_, _) =>
        {
            if (SelectedBorderColor == Colors.Transparent)
            {
                OnPropertyChanged(nameof(BorderBrush));
            }
        };
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BorderWidth))]
    private int _selectedBorderWidthItem = 2;
    
    public int[] BorderWidthItems => [1, 2, 3, 4, 5, 6, 7, 8, 9, 10] ;

    public Thickness BorderWidth => new Thickness(SelectedBorderWidthItem);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BorderBrush))]
    private Color _selectedBorderColor = Colors.Transparent;
    
    public IBrush BorderBrush => SelectedBorderColor == Colors.Transparent ? Brush.Parse(AvaloniaFluentTheme.Instance.IsDarkTheme ? "#484848" : "#D6D6D6") : new SolidColorBrush(SelectedBorderColor);
    
    [RelayCommand]
    private void ToggleTheme() => AvaloniaFluentTheme.Instance.ToggleTheme(); 

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

        if (target == CurrentViewModel) return;

        if (CurrentViewModel is HomeViewModel homeVm && CurrentViewModel != target)
        {
            homeVm.ReleaseImages();
        }

        if (CurrentViewModel != null)
        {
            var currentPageKey = GetKeyByViewModel(CurrentViewModel);
            if (currentPageKey != null)
            {
                Console.WriteLine("------------------------------------------------------------");
                _history.Add(currentPageKey);
                Console.WriteLine($"Load To History: {currentPageKey}");
                Console.WriteLine("------------------------------------------------------------");
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
            
            Console.WriteLine($"Back, Tag: {last}, View: {view.Title}, Trigger Jump To ControlMessage");
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
