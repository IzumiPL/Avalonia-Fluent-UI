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
    public event EventHandler<SearchTriggeredEventArgs>? OnSearchTriggered;
    
    private const string PART_SEARCH_BUTTON = "PART_SearchButton";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _searchButton?.Click -= OnSearchButtonClick;
        base.OnApplyTemplate(e);
        
        _searchButton = e.NameScope.Find<Button>(PART_SEARCH_BUTTON);
        
        _searchButton?.Click += OnSearchButtonClick;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (IsReturnSearch && e.Key == Key.Enter)
        {
            OnSearchTriggered?.Invoke(this, new SearchTriggeredEventArgs(Text));
        }
    }

    private void OnSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        OnSearchTriggered?.Invoke(this, new SearchTriggeredEventArgs(Text));
    }
}
