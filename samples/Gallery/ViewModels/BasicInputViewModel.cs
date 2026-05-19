using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class BasicInputViewModel : ViewModelBase
{
    [ObservableProperty]
    private HorizontalAlignment[] _horizontalAlignments =
    [
        HorizontalAlignment.Left,
        HorizontalAlignment.Right,
        HorizontalAlignment.Stretch,
        HorizontalAlignment.Center
    ];

    #region PushButton
    
    [ObservableProperty]
    private double _pushButtonWidth = 256;
    
    [ObservableProperty]
    private double _pushButtonHeight = 35;

    [ObservableProperty]
    private HorizontalAlignment _pushButtonContentAlignment = HorizontalAlignment.Center;

    [ObservableProperty]
    private bool _pushButtonIsDisable;

    [ObservableProperty]
    private double _pushButtonMinimumWidth = 256;

    [ObservableProperty]
    private double _pushButtonMinimumHeight = 35;

    [ObservableProperty]
    private double _pushButtonMaximumWidth = 512;

    [ObservableProperty]
    private double _pushButtonMaximumHeight = 128;
    
    #endregion

    #region ToolButton 

    [ObservableProperty]
    private double _toolButtonWidth = 64;

    [ObservableProperty]
    private double _toolButtonHeight = 35;
    
    [ObservableProperty]
    private HorizontalAlignment _toolButtonContentAlignment = HorizontalAlignment.Center;

    [ObservableProperty]
    private double _toolButtonMinimumWidth = 40;

    [ObservableProperty]
    private double _toolButtonMaximumWidth = 512;

    [ObservableProperty]
    private double _toolButtonMinimumHeight = 35;

    [ObservableProperty]
    private double _toolButtonMaximumHeight = 128;

    [ObservableProperty]
    private bool _toolButtonIsDisable;

    #endregion

    [ObservableProperty]
    private bool _statusSwitchButtonIsDisable;

    [ObservableProperty]
    private bool _splitButtonIsDisable;

    [ObservableProperty]
    private bool _radioButtonIsDisabled;

    [ObservableProperty]
    private bool _hyperlinkButtonIsDisable;

    [ObservableProperty]
    private bool _toggleSwitchButtonIsDisable;

    [ObservableProperty]
    private bool _checkBoxIsDisable;

    [ObservableProperty]
    private bool _checkBoxIsThreeState;

    [ObservableProperty]
    private bool _dropDownButtonIsDisable;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SliderCurrentValueFormat))]
    private double _sliderCurrentValue;

    public string SliderCurrentValueFormat => "当前值" +  SliderCurrentValue.ToString("F");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlinePushButtonGroupName))]
    private bool _outlinePushButtonIsMC = true;
    
    public string? OutlinePushButtonGroupName => OutlinePushButtonIsMC ? null : "OutlinePushButtonGroup1";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlineToolButtonGroupName))]
    private bool _outlineToolButtonIsMC = true;
    
    public string? OutlineToolButtonGroupName => OutlineToolButtonIsMC ? null : "OutlineToolButtonGroup1";

    private string[] _multiSelectionItems = new string[32];
    public string[] MultiSelectionItems => _multiSelectionItems;

    [RelayCommand]
    private void ClearMultiSelectionSelectedItem() => MultiSelectionSelectedItems.Clear();

    [ObservableProperty]
    private ObservableCollection<object> _multiSelectionSelectedItems = new ObservableCollection<object>();

    [ObservableProperty]
    private bool _outlineToolButtonIsDisabled;

    [ObservableProperty]
    private bool _outlinePushButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentDropDownButtonIsDisable;

    [ObservableProperty]
    private bool _subTitleRadioButtonIsDisabled; 

    [ObservableProperty]
    private bool _filledButtonIsDisabled;

    [ObservableProperty]
    private List<string> _items = new List<string>();

    public BasicInputViewModel()
    {
        for (int i = 1; i <= 32; i++)
        {
            Items.Add($"Item {i}");
            _multiSelectionItems[i - 1] = $"Multi Selection Item {i}";

            // MultiSelectionSelectedItems.CollectionChanged += OnMultiSelectionSelectedItemsChanged;
        }
    }

    [RelayCommand]
    private void SelectOddOrEventNumberItems(object value)
    {
        if (int.TryParse(value.ToString(), out int number))
        {
            Console.WriteLine("是 Int");
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

    private void OnMultiSelectionSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        
    }
}
