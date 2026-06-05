using System;
using System.Collections.Generic;
using System.IO;

namespace FlipViewTest.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public List<string> Images { get; }

    public MainWindowViewModel()
    {
        Images = new List<string>();
        
        foreach (var file in Directory.GetFiles(@"C:\Users\Administrator\OneDrive\Pictures\Background"))
        {
            if (file.Split(".")[^1] == "png")
            {
                Images.Add(file);
            }
        }
    }
}
