using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Controls;

namespace Gallery.Controls;

public class InfoBarHostViewBase : ViewBase 
{
    public InfoBarHostViewBase()
    {
        
    }

    public InfoBarHostViewBase(string page) : base(page)
    {
        
    }
    
    protected override Type StyleKeyOverride => typeof(InfoBarHostViewBase);
    
    public InfoBarHost InfoBarHost { get; private set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        InfoBarHost = e.NameScope.Find<InfoBarHost>("InfoBarHost");
    }
}

