using System.Collections.Generic;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ToolTipPage : ViewBase 
{
    public ToolTipPage() : base("ToolTip")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>() { { "ToolTip", ToolTipCard } };
    }
    
        private void OnNexting(object? sender, WizardNextingEventArgs e)
    {
        if (e.CurrentIndex == 1)
        {
            if ((TokenEdit.Text != "1") || (ProxyEdit.Text != "1"))
            {
                e.Cancel = true;
            }
        }
    }

    private void OnFinished(object? sender, System.EventArgs e)
    {
        WizardView.IsVisible = false;
    }

    private void OnFinishing(object? sender, WizardFinishingEventArgs e)
    {
    }

    private void OnNexted(object? sender, System.EventArgs e)
    {
    }
}
