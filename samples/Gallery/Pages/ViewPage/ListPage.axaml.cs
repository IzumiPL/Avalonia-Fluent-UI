using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ListPage : ViewBase
{
    public ListPage() : base("List")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ListBox", ListBoxCard},
        };
    }
}
