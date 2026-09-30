using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace Gallery.Pages.CustomPage;

public partial class SegmentedPage : UserControl
{
    private static readonly Random Random = new();

    private static readonly string[] Colors = { "#E60087", "#A8A8A8", "#f08080", "#f0e68c", "#3cb371", "#d59ad5", "#FFB70707", "#FFD16163", "#FF56D1BB", "#FF94D125" };

    private static readonly string[] Sentences =
    {
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
        "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
        "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.",
        "Maecenas faucibus mollis interdum, sed posuere consectetur est at lobortis.",
        "Aenean lacinia bibendum nulla sed consectetur.",
        "Donec ullamcorper nulla non metus auctor fringilla.",
        "Curabitur blandit tempus porttitor.",
        "Nullam id dolor id nibh ultricies vehicula ut id elit.",
        "Vestibulum id ligula porta felis euismod semper.",
        "Cras mattis consectetur purus sit amet fermentum.",
        "Morbi leo risus, porta ac consectetur ac, vestibulum at eros.",
        "Praesent commodo cursus magna, vel scelerisque nisl consectetur et.",
        "Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh."
    };
    
    public SegmentedPage()
    {
        InitializeComponent();
        Generate();
    }

    public void Generate()
    {
        GenerateRectangles();
        GenerateDescription();
    }

    private void GenerateRectangles()
    {
        RectanglesHost.Children.Clear();

        var count = Random.Shared.Next(4, 7);

        var rightCount = count - 1;

        RectanglesHost.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        RectanglesHost.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

        var leftColumn = new ColumnDefinition(1.1, GridUnitType.Star);

        RectanglesHost.ColumnDefinitions.Clear();
        RectanglesHost.ColumnDefinitions.Add(leftColumn);
        RectanglesHost.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        RectanglesHost.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

        var rowCount = (int)Math.Ceiling(rightCount / 2.0);

        for (var i = 0; i < rowCount; i++)
        {
            RectanglesHost.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
        }

        const double spacing = 12;

        RectanglesHost.RowSpacing = spacing;
        RectanglesHost.ColumnSpacing = spacing;

        var largeRectangle = CreateRectangle();

        Grid.SetColumn(largeRectangle, 0);
        Grid.SetRow(largeRectangle, 0);
        Grid.SetRowSpan(largeRectangle, rowCount);

        RectanglesHost.Children.Add(largeRectangle);

        for (var i = 0; i < rightCount; i++)
        {
            var rectangle = CreateRectangle();

            var row = i / 2;
            var column = i % 2 + 1;

            Grid.SetRow(rectangle, row);
            Grid.SetColumn(rectangle, column);

            RectanglesHost.Children.Add(rectangle);
        }
    }

    private static Border CreateRectangle()
    {
        var color = Colors[Random.Shared.Next(Colors.Length)];

        return new Border
        {
            Background = new SolidColorBrush(Color.Parse(color)),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };
    }

    private void GenerateDescription()
    {
        var count = Random.Shared.Next(2, 4);
        var sentences = new List<string>();

        for (var i = 0; i < count; i++)
        {
            sentences.Add(Sentences[Random.Shared.Next(Sentences.Length)]);
        }

        DescriptionText.Text = string.Join(" ", sentences);
    }
}
