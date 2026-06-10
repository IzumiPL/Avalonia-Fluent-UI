using System;
using System.Collections.Specialized;
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
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Control used to display or group settings options within an app, like in
/// the Windows 11 Settings app
/// </summary>
[PseudoClasses(SharedPseudoclasses.s_pcAllowClick, s_pcEmpty)]
[TemplatePart(s_tpExpander, typeof(Expander))]
[TemplatePart(s_tpContentHost, typeof(SettingsExpanderItem))]
public partial class SettingsExpander : HeaderedItemsControl, ICommandSource
{
    /// <summary>
    /// Defines the <see cref="Description"/> property
    /// </summary>
    public static readonly StyledProperty<string> DescriptionProperty = 
        AvaloniaProperty.Register<SettingsExpander, string>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<object?> IconSourceProperty = 
        AvaloniaProperty.Register<SettingsExpander, object?>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="Footer"/> property
    /// </summary>
    public static readonly StyledProperty<object> FooterProperty = 
        AvaloniaProperty.Register<SettingsExpander, object>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="FooterTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> FooterTemplateProperty = 
        AvaloniaProperty.Register<SettingsExpander, IDataTemplate>(nameof(FooterTemplate));

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        Expander.IsExpandedProperty.AddOwner<SettingsExpander>();

    /// <summary>
    /// Defines the <see cref="ActionIconSource"/> property
    /// </summary>
    public static readonly StyledProperty<object?> ActionIconSourceProperty = 
        AvaloniaProperty.Register<SettingsExpander, object?>(nameof(ActionIconSource));

    /// <summary>
    /// Defines the <see cref="IsClickEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsClickEnabledProperty = 
        AvaloniaProperty.Register<SettingsExpander, bool>(nameof(IsClickEnabled));

    /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CommandProperty = 
        Button.CommandProperty.AddOwner<SettingsExpander>();

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> CommandParameterProperty = 
        Button.CommandParameterProperty.AddOwner<SettingsExpander>();

    // NOTE: Don't use Button.Click event here - when SettingsExpanderItem is in the top-level SettingsExpander
    // there is a ToggleButton that is used to raise this event. If we use Button.Click here, and someone is 
    // listening to Button.Click event with handledEventsToo = true, they'll get 2 click events as a result
    /// <summary>
    /// Defines the <see cref="Click"/> event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<SettingsExpander, RoutedEventArgs>(nameof(Click), RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

    /// <summary>
    /// Gets or sets the description text
    /// </summary>
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the IconSource for the SettingsExpander
    /// </summary>
    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the Footer content for the SettingsExpander
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the Footer template for the SettingsExpander
    /// </summary>
    public IDataTemplate FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the SettingsExpander is currently expanded
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the Action IconSource when <see cref="IsClickEnabled"/> is true
    /// </summary>
    public object? ActionIconSource
    {
        get => GetValue(ActionIconSourceProperty);
        set => SetValue(ActionIconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the item is clickable which can be used for navigation within an app
    /// </summary>
    /// <remarks>
    /// This property can only be set if no items are added to the SettingsExpander. Attempting to mark
    /// a settings expander clickable and adding child items will throw an exception
    /// </remarks>
    public bool IsClickEnabled
    {
        get => GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the Command that is invoked upon clicking the item
    /// </summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    /// <summary>
    /// Event raised when the SettingsExpander is clicked and IsClickEnabled = true
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private const string s_tpExpander = "Expander";
    private const string s_tpContentHost = "ContentHost";

    private const string s_pcEmpty = ":empty";
    private const string s_pcIconPlaceholder = ":iconPlaceholder";
    
    public SettingsExpander()
    {
        ItemsView.CollectionChanged += ItemsCollectionChanged;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _expander = e.NameScope.Get<Expander>(s_tpExpander);
        // The Expander's template hasn't been loaded yet, so defer until later when it has
        // so we can load the ToggleButton within the template
        _expander.Loaded += ExpanderLoaded;
        _expander.Expanding += ExpanderExpanding;

        _contentHost = e.NameScope.Get<SettingsExpanderItem>(s_tpContentHost);
        _hasAppliedTemplate = true;

        SetIcons();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsClickEnabledProperty)
        {
            var newVal = change.GetNewValue<bool>();
            if (ItemCount > 0 && newVal)
            {
                throw new InvalidOperationException("Cannot set Items and mark IsClickEnabled to true on a SettingsExpander");
            }
                       
            if (_expanderToggleButton != null)
            {
                // Disable pointerover/pressed styles if we aren't clickable (empty or !IsClickEnabled)
                ((IPseudoClasses)_expanderToggleButton.Classes).Set(SharedPseudoclasses.s_pcAllowClick, newVal || ItemCount > 0);

                // When IsClickEnabled == true, we don't allow items but we may want to show an ActionIcon
                // ControlThemes don't let is drill into sub-templates so we have to do this manually here
                // Set a style on the ToggleButton to indicate we want to hide the expand/collapse chevron
                ((IPseudoClasses)_expanderToggleButton.Classes).Set(s_pcEmpty, newVal);
            }                
        }
        else if (change.Property == IsExpandedProperty)
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
                var (oldValue, newValue) = change.GetOldAndNewValue<ICommand>();
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
        else if (change.Property == IconSourceProperty)
        {
            var oldVal = change.OldValue;
            if (oldVal != null)
                _iconCount--;

            var newVal = change.NewValue;
            if (newVal != null)
                _iconCount++;

            SetIcons();
        }
    }

    private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // This fires for collection changes, whether they originate from Items or ItemsSource

        if (IsClickEnabled && ItemsView.Count > 0)
            throw new InvalidOperationException("Cannot set Items and mark IsClickEnabled to true on a SettingsExpander");

        if (_expanderToggleButton is not null)
        {
            // Disable the interaction states if items collection is cleared
            bool isInteractable = ItemCount > 0;
            ((IPseudoClasses)_expanderToggleButton.Classes).Set(SharedPseudoclasses.s_pcAllowClick, isInteractable);
            ((IPseudoClasses)_expanderToggleButton.Classes).Set(s_pcEmpty, !isInteractable); 
        }
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        bool isItem = item is SettingsExpanderItem;
        recycleKey = isItem ? null : nameof(SettingsExpanderItem);
        return !isItem;
    }

    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        var cont = this.FindDataTemplate(item, ItemTemplate)?.Build(item);

        if (cont is SettingsExpanderItem sei)
        {
            sei.DataContext = item;
            sei.IsContainerFromTemplate = true;
            return sei;
        }

        return new SettingsExpanderItem();
    }

    protected override void PrepareContainerForItemOverride(Control container, object item, int index)
    {
        var sei = container as SettingsExpanderItem;

        // If the container was created from a DataTemplate, do NOT call PrepareContainer or it will
        // do another template lookup and then put a item within an item as it sets the normal
        // ContentControl properties. Items created from a DataTemplate are assumed to be
        // initialized, to be sure the DataContext is set in CreateContainer
        if (!sei.IsContainerFromTemplate)
            base.PrepareContainerForItemOverride(container, item, index);

        if (sei.IconSource != null)
            _iconCount++;
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);

        if (container is SettingsExpanderItem sei)
        {
            if (sei.IconSource != null)
                _iconCount--;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var sz = base.MeasureOverride(availableSize);
        SetIcons();
        return sz;
    }

    /// <summary>
    /// Invoked when the SettingsExpander is clicked when IsClickEnabled = true
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
       
    private void ExpanderLoaded(object sender, RoutedEventArgs e)
    {
        // Don't need this anymore, clear it
        _expander.Loaded -= ExpanderLoaded;

        if (_expanderToggleButton != null)
            _expanderToggleButton.Click -= ExpanderToggleButtonClick;

        var header = _expander.GetTemplateChildren().OfType<ToggleButton>().FirstOrDefault();
        if (header == null)
            throw new InvalidOperationException("Invalid template for SettingsExpander. Unable to find ToggleButton inside Expander");

        _expanderToggleButton = header;
        _expanderToggleButton.Click += ExpanderToggleButtonClick;

        bool allowClick = IsClickEnabled;

        // Disable pointerover/pressed styles if we aren't clickable (empty or !IsClickEnabled)
        ((IPseudoClasses)_expanderToggleButton.Classes).Set(SharedPseudoclasses.s_pcAllowClick, IsClickEnabled || ItemCount > 0);

        // When IsClickEnabled == true, we don't allow items but we may want to show an ActionIcon
        // ControlThemes don't let is drill into sub-templates so we have to do this manually here
        // Set a style on the ToggleButton to indicate we want to hide the expand/collapse chevron
        ((IPseudoClasses)_expanderToggleButton.Classes).Set(s_pcEmpty, IsClickEnabled || ItemCount == 0);
    }
    
    private void ExpanderExpanding(object sender, CancelRoutedEventArgs e)
    {
        if (ItemCount == 0 && IsClickEnabled)
        {
            e.Cancel = true;
            e.Handled = true;
        }
    }

    private void ExpanderToggleButtonClick(object sender, RoutedEventArgs e)
    {
        if (!(e.Source == _expanderToggleButton))
            return;

        e.Handled = true;
        OnClick();
    }

    private void CanExecuteChanged(object sender, EventArgs e)
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

    private void SetIcons()
    {
        if (!_hasAppliedTemplate)
            return;

        // If the item count is 0, setting IconSource will automatically handle this
        // by the :icon in the SettingsExpanderItem
        if (ItemCount == 0)
            return;

        bool usePlaceholder = _iconCount > 0;
        ((IPseudoClasses)_contentHost.Classes).Set(s_pcIconPlaceholder, usePlaceholder);

        var rc = GetRealizedContainers();
        foreach (var item in GetRealizedContainers())
        {
            ((IPseudoClasses)item.Classes).Set(s_pcIconPlaceholder, usePlaceholder);
        }
    }

    internal void InvalidateIcons(SettingsExpanderItem item)
    {
        if (item == _contentHost)
            return;

        SetIcons();
    }

    private bool _commandCanExecute = true;
    private Expander _expander;
    private ToggleButton _expanderToggleButton;
    private SettingsExpanderItem _contentHost;
    private int _iconCount = 0;
    private bool _hasAppliedTemplate;
}
