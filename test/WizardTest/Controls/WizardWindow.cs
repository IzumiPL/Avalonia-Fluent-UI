using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Windowing;

namespace WizardTest.Controls;

public class WizardWindow : FluentWindow 
{
    public WizardView WizardView { get; }

    public WizardWindow()
    {
        WizardView = new WizardView
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(0),
            BorderThickness =  new Thickness(0),
            BorderBrush = Brushes.Transparent,
        };

        Content = WizardView;

        MaxButtonIsVisible = false;
        MinButtonIsVisible = false;
        CanResize = false;
        EnabledMica(true);
        // EnabledAcrylicBlue(true);
    }

    public WizardWindow(IEnumerable<Control> pages) : this()
    {
        AddPages(pages);
    }

    public WizardWindow(params Control[] pages) : this((IEnumerable<Control>)pages) { }

    public void AddPage(Control page)
    {
        WizardView.Items.Add(page);
    }

    public void AddPages(IEnumerable<Control> pages)
    {
        foreach (var page in pages)
        {
            WizardView.Items.Add(page);
        }
    }
}

