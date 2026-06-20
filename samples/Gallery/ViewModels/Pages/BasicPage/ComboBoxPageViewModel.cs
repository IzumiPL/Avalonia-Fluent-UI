using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class ComboBoxPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ComboBox");
    
    [ObservableProperty]
    private List<string> _items = new List<string>();
    
    private string[] _multiSelectionItems = new string[32];
    
    public string[] MultiSelectionItems => _multiSelectionItems;

    [ObservableProperty]
    private ObservableCollection<object> _multiSelectionSelectedItems = new ObservableCollection<object>(["Multi Selection Item 1"]);

    public ComboBoxPageViewModel()
    {
        for (int i = 1; i <= 32; i++)
        {
            Items.Add($"Item {i}");
            _multiSelectionItems[i - 1] = $"Multi Selection Item {i}";
        }

        MultiSelectionSelectedItems.CollectionChanged += (_, _) =>
        {
            Console.WriteLine(MultiSelectionSelectedItems.Count);
        };
    }

    [RelayCommand]
    private void SelectOddOrEventNumberItems(object value)
    {
        if (int.TryParse(value.ToString(), out int number))
        {
            MultiSelectionSelectedItems.Clear();
            foreach (var item in MultiSelectionItems)
            {
                if (int.TryParse(item.Split(" ")[^1], out int iv))
                {
                    if (iv % 2 == number)
                    {
                        MultiSelectionSelectedItems.Add(item);
                    }
                }
            }
        }
    }
    
    [RelayCommand]
    private void ClearMultiSelectionSelectedItem() => MultiSelectionSelectedItems.Clear();
}
