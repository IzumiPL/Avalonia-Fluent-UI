using System;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Icons;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SegTest.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string[] SegmentedItems => ["主页", "应用", "游戏", "主题", "新增功能", "下载", "库"];

    public int[] IndexItems => [0, 1, 2, 3, 4, 5, 6];

    [ObservableProperty]
    private object _currentItem;

    [ObservableProperty]
    private int _currentIndex;

    [ObservableProperty]
    private Orientation _currentOrientation = Orientation.Horizontal;

    public Orientation[] Orientations => [Orientation.Horizontal, Orientation.Vertical];
    public Geometry[] SegmentedIconItems => 
    [
        FluentIcon.Home,
        FluentIcon.GitHub,
        FluentIcon.Music,
        FluentIcon.Video,
        FluentIcon.Code,
        FluentIcon.Edit,
        FluentIcon.Setting
    ];

    partial void OnCurrentItemChanged(object value)
    {
        Console.WriteLine($"Current Item Changed: {value}, Type: {value.GetType()}");
    }

    partial void OnCurrentIndexChanged(int value)
    {
        Console.WriteLine($"Current Index Changed: {value}");
    }

    [ObservableProperty]
    private ViewModelBase? _currentViewModel = null;
    
    public MainWindowViewModel()
    {
        
    }
}
