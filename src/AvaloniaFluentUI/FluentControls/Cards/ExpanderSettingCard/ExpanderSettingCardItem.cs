using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// An item displayed within a <see cref="ExpanderSettingCard"/>
/// </summary>
// [PseudoClasses(s_pcFooterBottom, SharedPseudoclasses.s_pcFooter, s_pcContent, s_pcDescription)]
[PseudoClasses(PC_PRESSED)]
public class ExpanderSettingCardItem : HeaderedContentControl, ICommandSource
{
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<ExpanderSettingCardItem, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<ExpanderSettingCardItem, object?>(nameof(IconSource));

    public static readonly StyledProperty<bool> IsClickEnabledProperty =
        AvaloniaProperty.Register<ExpanderSettingCardItem, bool>(nameof(IsClickEnabled));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ExpanderSettingCardItem, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ExpanderSettingCardItem, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public bool IsClickEnabled
    {
        get => GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
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
    
    /// <summary>
    /// Defines the <see cref="Click"/> event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        ExpanderSettingCard.ClickEvent;

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    /// <summary>
    /// Event raised when the SettingsExpander is clicked and IsClickEnabled = true
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    internal bool IsContainerFromTemplate { get; set; }

    private bool _commandCanExecute = true;
    private bool _isPressed;
    private IDisposable? _adaptiveWidthDisposable;
    private double _adaptiveWidthTrigger = 460;

    private const string PC_PRESSED = ":pressed";
    private const string PC_DESCRIPTION = ":description";

    private const string RES_ADAPTIVE_WIDTH_TRIGGER = "SettingsExpanderItemAdaptiveWidthTrigger";
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _adaptiveWidthDisposable = this.GetResourceObservable(RES_ADAPTIVE_WIDTH_TRIGGER)
            .Subscribe(OnAdaptiveWidthValueChanged);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DescriptionProperty)
        {
            OnDescriptionChanged(change);
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
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        if (Command != null)
        {
            Command.CanExecuteChanged += CanExecuteChanged;
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        _adaptiveWidthDisposable?.Dispose();
        _adaptiveWidthDisposable = null;

        if (Command != null)
        {
            Command.CanExecuteChanged -= CanExecuteChanged;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (IsClickEnabled && !e.Handled)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isPressed = true;
                PseudoClasses.Set(":pressed", true);
                e.Handled = true;
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (IsClickEnabled && !e.Handled && e.Pointer.Captured != null)
        {
            // We do this because we don't get PointerExited events when the pointer
            // has a control captured - but to match normal behavior when moving the
            // pointer outside the control bounds - we want to keep track of it so
            // we can take the pressed state away if the pointer moves outside so we
            // don't trigger a click event if you release the pointer outside
            var pt = e.GetCurrentPoint(this);
            if (new Rect(Bounds.Size).Contains(pt.Position))
            {
                _isPressed = true;
                PseudoClasses.Set(PC_PRESSED, true);
            }
            else
            {
                _isPressed = false;
                PseudoClasses.Set(PC_PRESSED, false);
            }
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_isPressed && IsClickEnabled)
        {
            _isPressed = false;
            PseudoClasses.Set(PC_PRESSED, false);

            if (!e.Handled)
            {
                e.Handled = true;

                OnClick();
            }
        }       
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _isPressed = false;
        PseudoClasses.Set(PC_PRESSED, false);
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter" || presenter.Name == "FooterPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    /// <summary>
    /// Invoked when the SettingsExpanderItem is clicked when IsClickEnabled = true
    /// </summary>
    protected virtual void OnClick()
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

    private void OnDescriptionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        PseudoClasses.Set(PC_DESCRIPTION, args.NewValue != null);
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

    private void OnAdaptiveWidthValueChanged(object? value)
    {
        if (value == AvaloniaProperty.UnsetValue || value == null)
            return;

        _adaptiveWidthTrigger = Unsafe.Unbox<double>(value);
        InvalidateMeasure();
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) =>
        CanExecuteChanged(sender, e);
}
