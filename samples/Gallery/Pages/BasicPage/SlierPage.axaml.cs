using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SliderPage : ViewBase 
{
    public SliderPage() : base("Slider")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>() { { "Slider", SliderCard }, };
    }
}

