using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Windows.Input;
using Avalonia.Controls.Metadata;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Input;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a command in a <see cref="FAMenuFlyout"/> control.
/// </summary>
[PseudoClasses(SharedPseudoclasses.s_pcHotkey)]
[PseudoClasses(SharedPseudoclasses.s_pcPressed)]
public partial class MenuFlyoutItem : MenuFlyoutItemBase, ICommandSource
{
    /// <summary>
    /// Defines the <see cref="Text"/> property
    /// </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MenuFlyoutItem, string>(nameof(Text));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        SettingsExpander.IconSourceProperty.AddOwner<MenuFlyoutItem>();

    /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CommandProperty =
        Button.CommandProperty.AddOwner<MenuFlyoutItem>();

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<MenuFlyoutItem>();

    /// <summary>
    /// Defines the <see cref="HotKey"/> property
    /// </summary>
    public static readonly StyledProperty<KeyGesture> HotKeyProperty =
        Button.HotKeyProperty.AddOwner<MenuFlyoutItem>();

    /// <summary>
    /// Defines the <see cref="InputGesture"/> property
    /// </summary>
    public static readonly StyledProperty<KeyGesture> InputGestureProperty =
        AvaloniaProperty.Register<MenuFlyoutItem, KeyGesture>(nameof(InputGesture));

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<MenuFlyoutItemTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<MenuFlyoutItem, MenuFlyoutItemTemplateSettings>(nameof(TemplateSettings));

    /// <summary>
    /// Gets or sets the text content of a MenuFlyoutItem.
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the graphic content of the menu flyout item.
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the KeyGesture that should invoke this MenuFlyoutItem
    /// </summary>
    public KeyGesture HotKey
    {
        get => GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the input gesture displayed by the MenuFlyoutItem
    /// </summary>
    /// <remarks>
    /// This property is equivalent to WinUI's KeyboardAcceleratorTextOverride
    /// property. It allows you to specify a key gesture without mapping to 
    /// a hotkey. This property takes priority over <see cref="HotKey"/>
    /// </remarks>
    public KeyGesture InputGesture
    {
        get => GetValue(InputGestureProperty);
        set => SetValue(InputGestureProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to invoke when the item is pressed.
    /// </summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the <see cref="Command"/> property.
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets the template settings for this MenuFlyoutItem
    /// </summary>
    public MenuFlyoutItemTemplateSettings TemplateSettings
    {
        get => GetValue(TemplateSettingsProperty);
        private set => SetValue(TemplateSettingsProperty, value);
    }

    protected override bool IsEnabledCore => base.IsEnabledCore && _canExecute;

    /// <summary>
    /// Defines the <see cref="Click"/> event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = MenuItem.ClickEvent;

    /// <summary>
    /// Raised when this MenuFlyoutItem is invoked
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }
    
    /// <summary>
    /// Create instance of <see cref="MenuFlyoutItem"/>.
    /// </summary>
    public MenuFlyoutItem()
    {
        TemplateSettings = new MenuFlyoutItemTemplateSettings();
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        if (_hotkey != null)
        {
            HotKey = _hotkey;
        }

        base.OnAttachedToLogicalTree(e);

        if (Command != null)
        {
            Command.CanExecuteChanged += CanExecuteChanged;
            CanExecuteChanged(this, null);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        if (HotKey != null)
        {
            _hotkey = HotKey;
            HotKey = null;
        }

        base.OnDetachedFromLogicalTree(e);

        if (Command != null)
        {
            Command.CanExecuteChanged -= CanExecuteChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // For this, we make assumption that if you set Hotkey or assign a XamlUICommand,
        // you won't also set the InputGesture to something different, so setting HotKey
        // or using a XamlUICommand will automatically set the InputGesture property

        if (change.Property == CommandProperty)
        {
            var oldCommand = change.GetOldValue<ICommand>();
            var newCommand = change.GetNewValue<ICommand>();

            if (oldCommand is XamlUICommand oldXaml)
            {
                if (Text == oldXaml.Label)
                {
                    Text = null;
                }

                if (InputGesture == oldXaml.HotKey)
                {
                    HotKey = null;
                }
            }

            if (newCommand is XamlUICommand newXaml)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    Text = newXaml.Label;
                }

                if (IconSource == null)
                {
                    IconSource = newXaml.IconSource;
                }

                if (InputGesture == null)
                {
                    HotKey = newXaml.HotKey;
                }
            }

            if (((ILogical)this).IsAttachedToLogicalTree)
            {
                if (oldCommand != null)
                {
                    oldCommand.CanExecuteChanged -= CanExecuteChanged;
                }

                if (newCommand != null)
                {
                    newCommand.CanExecuteChanged += CanExecuteChanged;
                }
            }

            CanExecuteChanged(this, null);
        }
        else if (change.Property == CommandParameterProperty)
        {
            CanExecuteChanged(this, null);
        }
        else if (change.Property == InputGestureProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcHotkey, change.NewValue != null);
        }
        else if (change.Property == HotKeyProperty)
        {
            var kg = change.GetNewValue<KeyGesture>();
            InputGesture = kg;
        }
        else if (change.Property == IconSourceProperty)
        {
            TemplateSettings.Icon = IconHelpers.CreateFromUnknown(change.GetNewValue<IconSource>());
        }
    }

    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcPressed, true);
        }
    }

    /// <inheritdoc />
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcPressed, false);
        }
    }

    /// <summary>
    /// Raise <see cref="MenuItem.ClickEvent"/> and invke <see cref="Command"/> if ti is set.
    /// </summary>
    protected virtual void OnClick()
    {
        RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent, this));

        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }

    internal void RaiseClick()
    {
        OnClick();
    }

    private void CanExecuteChanged(object sender, EventArgs e)
    {
        var canExec = Command == null || Command.CanExecute(CommandParameter);

        if (canExec != _canExecute)
        {
            _canExecute = canExec;
            UpdateIsEffectivelyEnabled();
        }
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged(sender, e);

    private bool _canExecute = true;
    private KeyGesture _hotkey;
}
