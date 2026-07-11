using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ToolTipPage : ViewBase 
{
    public ToolTipPage() : base("ToolTip")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>() { { "ToolTip", ToolTipCard } };
    }
}

