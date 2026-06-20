using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Controls;
using Gallery.Messages.IconViewMessages;

namespace Gallery.Views;

public partial class IconsView : UserControl
{
    private const int BatchSize = 20;
    private readonly int _iconWidth = 92;
    private readonly int _iconHeight = 92;

    private CheckedBorder? _currentItem;
    private readonly List<CheckedBorder> _allCards = new();

    public IconsView()
    {
        InitializeComponent();

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(100);
            await LoadIconsAsync();
        }, DispatcherPriority.Background);
    }

    private async Task LoadIconsAsync()
    {
        var allIcons = GetAllIcons();
        var iconList = allIcons.ToList();

        foreach (var chunk in Chunk(iconList, BatchSize))
        {
            foreach (var (name, path) in chunk)
            {
                var iconCard = CreateIconCard(name, path);
                UniformGrid.Children.Add(iconCard);
                _allCards.Add(iconCard);
            }

            await Task.Delay(1);
        }
    }

    private static IEnumerable<List<KeyValuePair<string, Geometry>>> Chunk(List<KeyValuePair<string, Geometry>> source, int batchSize)
    {
        for (int i = 0; i < source.Count; i += batchSize)
            yield return source.GetRange(i, Math.Min(batchSize, source.Count - i));
    }

    private CheckedBorder CreateIconCard(string name, Geometry data)
    {
        var iconCard = new CheckedBorder
        {
            Classes = { "IconCard" },
            Width = _iconWidth,
            Height = _iconHeight,
            Child = new StackPanel
            {
                Children =
                {
                    new PathIcon { Name = "PART_PathIcon", Tag = name, Data = data },
                    new TextBlock { Name = "PART_Name", Text = name }
                }
            },
            ContextMenu = new FluentContextMenu
            {
                Items =
                {
                    new MenuItem
                    {
                        Header = "复制Svg",
                        Command = new RelayCommand(() =>
                        {
                            TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(data.ToString());
                            Console.WriteLine(data.ToString());
                        })
                    }
                }
            }
        };

        iconCard.PointerReleased += (sender, e) =>
        {
            if (_currentItem != null) _currentItem.IsChecked = false;

            if (sender is CheckedBorder border)
            {
                var icon = border.FindLogicalDescendantOfType<PathIcon>();
                if (icon == null) return;

                _currentItem = border;
                WeakReferenceMessenger.Default.Send(new CheckedIconChangedMessage((string)icon.Tag!, icon.Data!));
            }

            e.Handled = true;
        };

        return iconCard;
    }

    private Dictionary<string, Geometry> GetAllIcons()
    {
        return typeof(AvaloniaFluentUI.Icons.FluentIcon)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsStatic && f.FieldType == typeof(Geometry))
            .ToDictionary(f => f.Name, f => (Geometry)f.GetValue(null)!);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var columns = (int)UniformGrid.Bounds.Width / (_iconWidth + (int)UniformGrid.ColumnSpacing);
        UniformGrid.Columns = columns;
    }

    private void ApplyFilter(string searchText)
    {
        foreach (var card in _allCards)
        {
            if (card.FindLogicalDescendantOfType<TextBlock>() is { Name: "PART_Name" } tb)
            {
                card.IsVisible = tb.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                card.IsVisible = false;
            }
        }
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e)
    {
        var app = Application.Current;
        if (app != null)
        {
            var theme = app.RequestedThemeVariant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
            app.RequestedThemeVariant = theme;
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is SearchTextBox tb)
        {
            ApplyFilter(tb.Text);
        }
    }
}
