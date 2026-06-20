using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class NavigationViewPage : ViewBase
{
    public NavigationViewPage() : base("NavigationView")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"NavigationView", NavigationViewCard},
        };
    }
}
