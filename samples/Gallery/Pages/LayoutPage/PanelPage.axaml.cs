using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Gallery.Controls;
using Gallery.Extensions;

namespace Gallery.Pages;

public partial class PanelPage : ViewBase
{
    public PanelPage() : base("Panel")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Grid", GridCard},
            {"RelativePanel", RelativePanelCard},
            {"StackPanel", StackPanelCard},
            {"Expander", ExpanderCard}
        };
        
        InitWrapPanel();
        InitUniformGridPanel();
    }
    
    private void InitUniformGridPanel()
    {
#if DEBUG
        Debug.WriteLine("InitUniformGridPanel");
#endif

        string[] colors = ["#E6194B", "#3CB44B", "#FFE119", "#4363D8", "#F58231", "#911EB4", "#42D4F4", "#F032E6", "#BFEF45", "#FABEBE", "#469990", "#E6BEFF", "#9A6324", "#FFFAC8", "#800000", "#AAFFC3", "#808000", "#FFD8B1", "#000075", "#A9A9A9", "#E06666", "#6FA8DC", "#8E7CC3", "#FFD966", "#93C47D"];
        foreach (var color in colors)
        {
            UniformGrid.Children.Add(new Rectangle{ Fill = Brush.Parse(color), RadiusX = 8, RadiusY = 8});
        }
    }

    private void InitWrapPanel()
    {
        var random = new Random();
        for (int i = 0; i < 24; i++)
        {
            AddRectangleToWrapPanel(random);
        }
    }

    private void OnRowsChanged(object? sender, TextChangedEventArgs e)
    {
        Grid.RowDefinitions = new RowDefinitions(GetDefinitions(RowsEdit.Text.ToIntOrZero()));
    }

    private void OnColumnsChanged(object? sender, TextChangedEventArgs e)
    {
        Console.WriteLine(e.ToString());
        Grid.ColumnDefinitions = new ColumnDefinitions(GetDefinitions(ColumnsEdit.Text.ToIntOrZero()));
    }
    
    private string GetDefinitions(int count)
    {
        string definitions = string.Empty;
        for (int i = 0; i < count - 1; i++)
        {
            definitions += "*,";
        }

        return definitions + "*";
    }
    
    private void AddRectangleToWrapPanel(Random random)
    {
        byte r = (byte)random.Next(256);
        byte g = (byte)random.Next(256);
        byte b = (byte)random.Next(256);
        // WrapPanel.Children.Add(new Rectangle { Fill = new SolidColorBrush(Color.FromRgb(r, g, b)), RadiusX = 8, RadiusY = 8, Width = random.Next(64, 255), Height = 64 });
        WrapPanel.Children.Add(new Rectangle { Fill = new SolidColorBrush(Color.FromRgb(r, g, b)), RadiusX = 8, RadiusY = 8, Width = 64, Height = 64 });
    }

    private void OnClickedAddControlToWrapPanel(object? sender, RoutedEventArgs e)
    {
        AddRectangleToWrapPanel(new Random());
    }

    private void OnClickedRemoveControlOfWrapPanel(object? sender, RoutedEventArgs e)
    {
        int count = WrapPanel.Children.Count;
        if (count > 0)
        {
            WrapPanel.Children.RemoveAt(count - 1);
        }
    }
}
