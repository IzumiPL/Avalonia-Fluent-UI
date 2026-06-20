using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class TreeViewPage : ViewBase
{
    public TreeViewPage()  : base("TreeView")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TreeView", TreeViewCard},
        };
    }
}
