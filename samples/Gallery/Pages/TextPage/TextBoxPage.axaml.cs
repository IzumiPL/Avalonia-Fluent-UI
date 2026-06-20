using System.Collections.Generic;
using Avalonia.Input;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class TextBoxPage : ViewBase
{
    public TextBoxPage() :  base("TextBox")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TextBox", TextBoxCard},
            {"PasswordBox", PasswordBoxCard},
        };
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        var box = (SearchTextBox)sender!;
        if (e.Key == Key.Enter && (StbCheBox.IsChecked ?? false))
        {
            SearchResult.Text = LocalizationService.Instance.GetString("WhatToSearchFor") + ": " + box?.Text;
        }
    }
}
