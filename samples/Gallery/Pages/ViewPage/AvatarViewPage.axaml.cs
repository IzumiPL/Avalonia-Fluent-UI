using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class AvatarViewPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/AvatarViewPage.axaml"); 
    
    public AvatarViewPage() : base("AvatarView")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Avatar", AvatarCard}
        };
    }
}
