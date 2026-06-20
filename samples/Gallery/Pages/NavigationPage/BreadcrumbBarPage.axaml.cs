using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class BreadcrumbBarPage : ViewBase
{
    public BreadcrumbBarPage() : base("BreadcrumbBar")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"BreadcrumbBar", BreadcrumbBarCard},
        };
    }
}
