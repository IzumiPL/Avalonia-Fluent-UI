using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class MenuPage : ViewBase
{
    public MenuPage() : base("Menu")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"MenuBar", MenuBarCard},
        };
    }
}
