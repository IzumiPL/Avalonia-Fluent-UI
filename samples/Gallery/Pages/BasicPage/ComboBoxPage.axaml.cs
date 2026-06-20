using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ComboBoxPage : ViewBase 
{
    public ComboBoxPage() : base("ComboBox")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ComboBox", ComboBoxCard},
        };
    }
}

