using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Test.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [RelayCommand]
    private void ToggleTheme()
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }

    public MainViewModel()
    {
        Items = new List<string>();
        
        for (int i = 1; i <= 100; i++)
        {
            Items.Add($"Combo Box Item {i}");
        }
    }
    
    
    public List<string> Items { get; }
    
    public string CurrentLanguageFormat => LocalizationService.Instance.CurrentLanguage;
    public IReadOnlyList<FilePickerFileType> FileTypeFilter => [new FilePickerFileType("图片")
    {
        Patterns =
        [
            "*.png",
            "*.jpg",
            "*.jpeg",
            "*.gif",
            "*.webp"
        ]
    }];

    [RelayCommand]
    private void OnFileDropped(IReadOnlyList<string> files)
    {
        foreach (var file in files)
        {
            Console.WriteLine($"选择的文件: {file}");
        }

        Console.WriteLine("========================");
    }

    [RelayCommand]
    private void OnFolderDropped(IReadOnlyList<string> folders)
    {
        
        foreach (var folder in folders)
        {
            Console.WriteLine($"选择的文件夹: {folder}");
        }
        
        Console.WriteLine("========================");
    }


    [RelayCommand]
    private void ToggleLanguage(object value)
    {
        string language = value.ToString();
        if (language != LocalizationService.Instance.CurrentLanguage)
        {
            LocalizationService.Instance.SetCulture(value.ToString());
            OnPropertyChanged(nameof(CurrentLanguageFormat));
        }
        
    }
}
