using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class BorderPage : ViewBase
{
    public BorderPage() : base("Border")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Border", BorderCard},
            {"Canvas", CanvasCard},
        };
    }
}
