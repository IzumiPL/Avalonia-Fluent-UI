using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Control used to display or group settings options within an app, like in
/// the Windows 11 Settings app
/// </summary>
[PseudoClasses(PC_DESCRIPTION)]
[TemplatePart(PART_EXPANDER, typeof(Expander))]
public class ExpanderSettingCard : HeaderedItemsControl, ICommandSource
{
    /// <summary>
    /// Defines the <see cref="IsExpanded"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        Expander.IsExpandedProperty.AddOwner<ExpanderSettingCard>();

    /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty = 
        Button.CommandProperty.AddOwner<ExpanderSettingCard>();

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty = 
        Button.CommandParameterProperty.AddOwner<ExpanderSettingCard>();

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<ExpanderSettingCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<ExpanderSettingCard, object?>(nameof(IconSource));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<ExpanderSettingCard, object?>(nameof(Footer));

    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<ExpanderSettingCard, IDataTemplate?>(nameof(FooterTemplate));

    public static readonly StyledProperty<bool> DescriptionIsVisibleProperty =
        AvaloniaProperty.Register<ExpanderSettingCard, bool>(nameof(DescriptionIsVisible), true);

    public bool DescriptionIsVisible
    {
        get => GetValue(DescriptionIsVisibleProperty);
        set => SetValue(DescriptionIsVisibleProperty, value);
    }

    public IDataTemplate? FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    // NOTE: Don't use Button.Click event here - when SettingsExpanderItem is in the top-level SettingsExpander
    // there is a ToggleButton that is used to raise this event. If we use Button.Click here, and someone is 
    // listening to Button.Click event with handledEventsToo = true, they'll get 2 click events as a result
    /// <summary>
    /// Defines the <see cref="Click"/> event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<ExpanderSettingCard, RoutedEventArgs>(nameof(Click), RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets whether the SettingsExpander is currently expanded
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the Command that is invoked upon clicking the item
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    /// <summary>
    /// Event raised when the SettingsExpander header is clicked
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private const string PART_EXPANDER = "PART_Expander";
    private const string PC_DESCRIPTION = ":description";
    
     protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _expander?.Loaded -= ExpanderLoaded;
        
        _expander = e.NameScope.Get<Expander>(PART_EXPANDER);
        _expander?.Loaded += ExpanderLoaded;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsExpandedProperty)
        {
            // Prevent going to expanded state if we don't have any child items
            // Use the IsAttachedToVisualTree flag here to prevent overwriting 'true' while control
            // is Initializing where IsExpanded may be set before Items
            if (ItemCount == 0 && change.GetNewValue<bool>() && this.IsAttachedToVisualTree())
            {
                // There seems to be an issue here where if we just set IsExpanded = false
                // the property does get set, but the :expanded pseudoclass is never cleared
                // from the Expander. So post to dispatcher to let this prop change notification
                // go through real quick, then change the value to false to get the correct state
                Dispatcher.UIThread.Post(() => IsExpanded = false, DispatcherPriority.Send);
            }
        }
        else if (change.Property == CommandProperty)
        {
            if (((ILogical)this).IsAttachedToLogicalTree)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<ICommand?>();
                if (oldValue != null)
                {
                    oldValue.CanExecuteChanged -= CanExecuteChanged;
                }

                if (newValue != null)
                {
                    newValue.CanExecuteChanged += CanExecuteChanged;
                }
            }

            CanExecuteChanged(this, EventArgs.Empty);
        }
        else if (change.Property == CommandParameterProperty)
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
        else if (change.Property == DescriptionProperty)
        {
            OnDescriptionChanged(change);
        }
    }
    
    private void OnDescriptionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        PseudoClasses.Set(PC_DESCRIPTION, args.NewValue != null);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object recycleKey)
    {
        bool isItem = item is ExpanderSettingCardItem;
        recycleKey = isItem ? null : nameof(ExpanderSettingCardItem);
        return !isItem;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var cont = this.FindDataTemplate(item, ItemTemplate)?.Build(item);

        if (cont is ExpanderSettingCardItem sei)
        {
            sei.DataContext = item;
            sei.IsContainerFromTemplate = true;
            return sei;
        }

        return new ExpanderSettingCardItem();
    }

    /// <summary>
    /// Invoked when the SettingsExpander header is clicked
    /// </summary>
    protected internal virtual void OnClick()
    {
        var args = new RoutedEventArgs(ClickEvent);
        RaiseEvent(args);

        var @param = CommandParameter;
        var command = Command;
        if (!args.Handled && command?.CanExecute(@param) == true)
        {
            command.Execute(@param);
        }
    }
       
    private void ExpanderLoaded(object? sender, RoutedEventArgs e)
    {
        if (_expanderToggleButton != null)
            _expanderToggleButton.Click -= ExpanderToggleButtonClick;

        var header = _expander?.GetTemplateDescendants().OfType<ToggleButton>().FirstOrDefault();
        if (header == null)
            throw new InvalidOperationException("Invalid template for SettingsExpander. Unable to find ToggleButton inside Expander");

        _expanderToggleButton = header;
        _expanderToggleButton.Click += ExpanderToggleButtonClick;
    }

    private void ExpanderToggleButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!(e.Source == _expanderToggleButton))
            return;

        e.Handled = true;
        OnClick();
    }

    private void CanExecuteChanged(object? sender, EventArgs e)
    {
        var command = Command;
        var canExecute = command == null || command.CanExecute(CommandParameter);

        if (canExecute != _commandCanExecute)
        {
            _commandCanExecute = canExecute;
            UpdateIsEffectivelyEnabled();
        }
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) =>
       CanExecuteChanged(sender, e);

    private bool _commandCanExecute = true;
    private Expander? _expander;
    private ToggleButton? _expanderToggleButton;
}
