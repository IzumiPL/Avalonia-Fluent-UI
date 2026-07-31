using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 带搜索按钮的输入框
/// </summary>
[TemplatePart(Name = PART_SEARCH_BUTTON, Type = typeof(Button))]
public class SearchTextBox : TextBox
{
    public static readonly StyledProperty<ICommand> SearchCommandProperty =
        AvaloniaProperty.Register<SearchTextBox, ICommand>(nameof(SearchCommand));

    public static readonly StyledProperty<bool> IsReturnSearchProperty =
        AvaloniaProperty.Register<SearchTextBox, bool>(nameof(IsReturnSearch));

    /// <summary>
    /// 设置或获取是否启用回车搜索
    /// </summary>
    public bool IsReturnSearch
    {
        get => GetValue(IsReturnSearchProperty);
        set => SetValue(IsReturnSearchProperty, value);
    }

    /// <summary>
    /// 搜索时触发, 接收的参数是<c>搜索的内容</c>
    /// </summary>
    public ICommand SearchCommand
    {
        get => GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }
    
    private Button? _searchButton;
    
    /// <summary>
    /// 搜索时触发
    /// </summary>
    public event Action<string?>? OnSearchTriggered;
    
    private const string PART_SEARCH_BUTTON = "PART_SearchButton";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_searchButton != null)
        {
            _searchButton.Click -= OnSearchButtonClick;
            _searchButton.KeyDown -= OnSearchButtonKeyDown;
        }
        base.OnApplyTemplate(e);
        
        _searchButton = e.NameScope.Find<Button>(PART_SEARCH_BUTTON);
        
        if  (_searchButton != null)
        {
            _searchButton.Click += OnSearchButtonClick;
            _searchButton.KeyDown += OnSearchButtonKeyDown;
        }
    }

    private void OnSearchButtonKeyDown(object? sender, KeyEventArgs e)
    {
        if (IsReturnSearch && e.Key == Key.Enter)
        {
            OnSearchTriggered?.Invoke(Text);
        }
    }

    private void OnSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        OnSearchTriggered?.Invoke(this.Text);
    }
}
