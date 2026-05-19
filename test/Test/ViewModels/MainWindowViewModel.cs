using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AvaloniaFluentUI.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Test.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    [RelayCommand]
    private void ChangedSelectionTo1()
    {
        MultiSelectedItems.Clear();
        foreach (var item in SegmentedItemSource)
        { 
            if (item is String value)
            {
                if (int.TryParse(value.Split(" ")[^1], out int iv))
                {
                    if (iv % 2 != 0)
                    {
                        MultiSelectedItems.Add(item);
                    }
                }
            }
        }
    }

    [RelayCommand]
    private void ChangedSelectionTo2()
    { 
        MultiSelectedItems.Clear();
        foreach (var item in SegmentedItemSource)
        {
            if (item is String value)
            {
                if (int.TryParse(value.Split(" ")[^1], out int iv))
                {
                    if (iv % 2 == 0)
                    {
                        MultiSelectedItems.Add(item);
                    }
                }
            }
        }
    } 

    [ObservableProperty]
    private ObservableCollection<object> _multiSelectedItems = new ObservableCollection<object>();

    [ObservableProperty]
    private ObservableCollection<string> _segmentedItemSource;

    public MainWindowViewModel()
    {
        SegmentedItemSource = new ObservableCollection<string>();
        
        for (int i = 1; i <= 32; i++)
        {
            SegmentedItemSource.Add($"Segmented Item {i}");
        }

        // MultiSelectedItems.CollectionChanged += OnMultiSelectedItemsChanged;
    }

    partial void OnMultiSelectedItemsChanged(ObservableCollection<object> value)
    {
        Console.WriteLine(true);
    }

    private void OnMultiSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is ObservableCollection<object> items)
        {
            Console.Write($"选中的项目: ");
            foreach (var item in items)
            {
                Console.Write(item + " | ");
            }

            Console.WriteLine();
        }
    }

    [RelayCommand]
    private void UpdateSegmentedItemSource()
    {
        string[] value = new string[128];
        
        for (int i = 1; i <= 128; i++)
        {
            value[i - 1] = $"Segmented Item {i}";
        }
        
        SegmentedItemSource =  new ObservableCollection<string>(value);
    }
    
    [ObservableProperty]
    private string _sT;

    [ObservableProperty]
    private object? _sITM;

    [ObservableProperty]
    private int _sind;

    [RelayCommand]
    private void RandToInd()
    {
        var random = new Random();
        int ind = random.Next(0, 32);
        Console.WriteLine($"To Ind => {ind}");
        Sind = ind;
    }

    partial void OnSITMChanged(object? value)
    {
        Console.WriteLine(value is SegmentedItem);
        Console.WriteLine(value.GetType());
        if (value is SegmentedItem item)
        {
            ST = $"Switch To => {item.Tag}";
        }
        // ST = value + "我是 change 文字 hahahaha";
    }
}
