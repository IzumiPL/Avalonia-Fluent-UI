using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SegmentedViewPage : ViewBase
{
    public SegmentedViewPage()  : base("SegmentedView")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Segmented", SegmentedCard}
        };
    }
}
