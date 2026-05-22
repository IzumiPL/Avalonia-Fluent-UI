using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = "PART_SearchButton", Type = typeof(Button))]
public class SearchTextBox : TextBox
{
    private Button _searchButton;
    public event Action<string> OnSearchTriggered;
    
    public static readonly StyledProperty<ICommand> SearchCommandProperty =
        AvaloniaProperty.Register<SearchTextBox, ICommand>(nameof(SearchCommand));

    public static readonly StyledProperty<bool> IsReturnSearchProperty =
        AvaloniaProperty.Register<SearchTextBox, bool>(nameof(IsReturnSearch));

    public bool IsReturnSearch
    {
        get => GetValue(IsReturnSearchProperty);
        set => SetValue(IsReturnSearchProperty, value);
    }

    public ICommand SearchCommand
    {
        get => GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_searchButton != null)
        {
            _searchButton.Click -= OnSearchButtonClick;
            _searchButton.KeyDown -= OnSearchButtonKeyDown;
        }
        base.OnApplyTemplate(e);
        
        _searchButton = e.NameScope.Find<Button>("PART_SearchButton");
        
        if  (_searchButton != null)
        {
            _searchButton.Click += OnSearchButtonClick;
            _searchButton.KeyDown += OnSearchButtonKeyDown;
        }
    }

    private void OnSearchButtonKeyDown(object sender, KeyEventArgs e)
    {
        if (IsReturnSearch && e.Key == Key.Enter)
        {
            OnSearchTriggered?.Invoke(Text);
        }
    }

    private void OnSearchButtonClick(object sender, RoutedEventArgs e)
    {
        OnSearchTriggered?.Invoke(this.Text);
    }
}
