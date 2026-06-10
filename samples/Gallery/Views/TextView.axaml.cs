using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class TextView : ViewBase
{
    private int value = 1;
    
    public TextView() : base("Text")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TextBlock", TextBlockCard},
            {"TextBox", TextBoxCard},
            {"PasswordBox", PasswordBoxCard},
            {"NumberBox", NumberBoxCard}
        };

        ButtonSpinner.Content = value;
    }
    
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;

        if (textBox?.Text?.Length > 1)
        {
            textBox.Text = textBox.Text.Substring(0, 1);
        }
    }

    private void OnSpin(object? sender, SpinEventArgs e)
    {
        if (e.Direction == SpinDirection.Increase)
        {
            value *= value;
        }
        else
        {
            if (value == 0 ) { return; }
            value /= value;
        }

        ButtonSpinner.Content = value;
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        var box = sender as SearchTextBox;
        if (e.Key == Key.Enter && (StbCheBox.IsChecked ?? false))
        {
            SearchResult.Text = "搜索的内容: " + box?.Text;
        }
    }
}
