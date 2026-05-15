using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.UI.Controls;

[TemplatePart("PART_ScrollViewer", typeof(IScrollable))]
[TemplatePart("PART_PreviousButton", typeof(ToolButton))]
[TemplatePart("PART_NextButton", typeof(ToolButton))]
public class DotPager : ListBox
{
    private ToolButton _previousButton;
    private ToolButton _nextButton;
    
    public DotPager()
    {
        
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedItemProperty)
        {
            if (SelectedItem is DotItem item && Scroll != null)
            {
                var point = item.TranslatePoint(new Point(0, 0), this);
                if (!point.HasValue) { return; }

                double target = point.Value.Y + item.Bounds.Height / 2 - Scroll.Viewport.Height / 2;
                Scroll.Offset = Scroll.Offset.WithY(target);
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_nextButton != null)
        {
            _nextButton.Click -= OnNextClicked;
        }

        if (_previousButton != null)
        {
            _previousButton.Click -= OnPreviousClicked;
        }
        
        _previousButton = e.NameScope.Find<ToolButton>("PART_PreviousButton");
        _nextButton = e.NameScope.Find<ToolButton>("PART_NextButton");
        
        if (_nextButton != null)
        {
            _nextButton.Click += OnNextClicked;
        }

        if (_previousButton != null)
        {
            _previousButton.Click += OnPreviousClicked;
        }
    }

    private void OnNextClicked(object sender, RoutedEventArgs e)
    {
        if (SelectedIndex < ItemCount)
        {
            SetCurrentValue(SelectedIndexProperty, SelectedIndex + 1);
        }
    }

    private void OnPreviousClicked(object sender, RoutedEventArgs e)
    {
        if (SelectedIndex > 0)
        {
            SetCurrentValue(SelectedIndexProperty, SelectedIndex - 1);
        }
    }


    protected  override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
	{
		return new DotItem();
	}

	protected  override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
	{
		return NeedsContainer<DotItem>(item, out recycleKey);
	}
}
