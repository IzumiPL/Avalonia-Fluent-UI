using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SpinBoxPage : ViewBase
{
    private int value = 1;
    
    public SpinBoxPage() : base("NumberBox")
    {
        InitializeComponent();
        ButtonSpinner.Content = value;
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"NumberBox", NumberBoxCard}
        };
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
}
