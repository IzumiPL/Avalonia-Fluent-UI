using System;
using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Services;

namespace Gallery.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public virtual string Title => String.Empty;

    public ViewModelBase()
    {
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Title));
    }

    [RelayCommand]
    private void Goto(object value)
    {
        if (value is Button button)
        {
            JumpService.GotoControl(button);
        }
    }
}
