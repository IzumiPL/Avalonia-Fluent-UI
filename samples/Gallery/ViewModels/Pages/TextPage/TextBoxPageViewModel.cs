using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class TextBoxPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("TextBox");
    
    public AutoCompleteFilterMode AutoCompleterMode => SelectedAutoCompleteMode;

    [ObservableProperty]
    private ObservableCollection<string> _autoCompleteItems = 
    [ 
        "cat",
        "camel", 
        "cow",
        "chameleon", 
        "mouse", 
        "lion", 
        "zebra", 
        "Before She Goes",
        "So in Love", 
        "Obsession", 
        "I Hate Falling In Love", 
        "Thinking Of You" 
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoCompleteBoxTextInfo))]
    private string _autoCompleteBoxText = String.Empty;

    public string AutoCompleteBoxTextInfo => $"Input Text: {AutoCompleteBoxText}";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutoCompleterMode))]
    private AutoCompleteFilterMode _selectedAutoCompleteMode = AutoCompleteFilterMode.Contains;

    [ObservableProperty]
    private bool _autoCompleteBoxSettingsPanelIsExpand;

    [RelayCommand]
    private void ExpandAutoCompleteBoxSettingsPane() => AutoCompleteBoxSettingsPanelIsExpand = !AutoCompleteBoxSettingsPanelIsExpand;
    

    [ObservableProperty]
    private string _autoCompleteElement = String.Empty;

    [RelayCommand]
    private void AddAutoCompleteItem()
    {
        if (!String.IsNullOrEmpty(AutoCompleteElement) && !AutoCompleteItems.Contains(AutoCompleteElement))
        {
            AutoCompleteItems.Add(AutoCompleteElement);
        }
    }

    [ObservableProperty]
    private List<AutoCompleteFilterMode> _autoCompleteFilterModeItems = [
        AutoCompleteFilterMode.None,
        AutoCompleteFilterMode.StartsWith,
        AutoCompleteFilterMode.StartsWithCaseSensitive,
        AutoCompleteFilterMode.StartsWithOrdinal,
        AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive,
        AutoCompleteFilterMode.Contains,
        AutoCompleteFilterMode.ContainsCaseSensitive,
        AutoCompleteFilterMode.ContainsOrdinal,
        AutoCompleteFilterMode.ContainsOrdinalCaseSensitive,
        AutoCompleteFilterMode.Equals,
        AutoCompleteFilterMode.EqualsCaseSensitive,
        AutoCompleteFilterMode.EqualsOrdinal,
        AutoCompleteFilterMode.EqualsOrdinalCaseSensitive,
        // AutoCompleteFilterMode.Custom,
    ];
    
    [ObservableProperty]
    private bool _isAccentReturn;

    [ObservableProperty]
    private bool _isAcceptTab;

    [ObservableProperty]
    private string? _watermark = LocalizationService.Instance.GetString("PleaseEnterText");
    
    [ObservableProperty]
    private TextWrapping _textWrapping = TextWrapping.NoWrap;

    [ObservableProperty]
    private string _alternativeCharacters = String.Empty;

    [ObservableProperty]
    private List<TextWrapping> _textWrappingItems = [
        TextWrapping.NoWrap,
        TextWrapping.Wrap,
        TextWrapping.WrapWithOverflow
    ];

    [ObservableProperty]
    private bool _textBoxSettingsPaneIsExpand;

    [RelayCommand]
    private void ExpandTextBoxSettingsPane() => TextBoxSettingsPaneIsExpand = !TextBoxSettingsPaneIsExpand;
    
    [RelayCommand]
    private void Search(object value)
    {
        SearchContent =LocalizationService.Instance.GetString("WhatToSearchFor") + ": " + value;
    }

    [ObservableProperty]
    private string _searchContent = LocalizationService.Instance.GetString("WhatToSearchFor") + ": " + "Null";
    
    // LabelTextBox
    [ObservableProperty]
    private string _prefix = "https://";
    
    [ObservableProperty]
    private string _suffix = ".com";
}
