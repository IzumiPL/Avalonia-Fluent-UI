using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Locale;

namespace Gallery.Views;

public partial class SettingsView : UserControl 
{
    public SettingsView()
    {
#if DEBUG
        Debug.WriteLine("SettingsView Init");
#endif
        InitializeComponent();
    }
}
