using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class TextBlockPage : ViewBase
{
    public TextBlockPage() : base("TextBlock")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TextBlock", TextBlockCard},
        };
    }
}
