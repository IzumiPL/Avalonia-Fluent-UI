using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Gallery.Controls;

namespace Gallery.Views;

public partial class ScrollView : ViewBase 
{
    public ScrollView()
    {
        InitializeComponent();
        VImage.Source = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/0.jpg")), 2500);
        var bitmap = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/mc.jpg")), 1600);
        HImage.Source = bitmap;
        VHImage.Source = bitmap;
    }
}
