using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Locale;

namespace AvaloniaFluentUI.Controls;

[PseudoClasses(SharedPseudoclasses.s_pcHidden, PC_CLOSE_HIDDEN)]
[PseudoClasses(PC_SUCCESS, PC_WARNING, PC_ERROR, PC_INFORMATIONAL)]
[PseudoClasses(SharedPseudoclasses.s_pcIcon, PC_STANDARD_ICON)]
[PseudoClasses(PC_FOREGROUND_SET)]
[TemplatePart(CLOSE_BUTTON, typeof(Button))]
public class InfoBar : ContentControl
{    
    /// <summary>
    /// Defines the <see cref="IsOpen"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Title"/> property
    /// </summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InfoBar, string>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Message"/> property
    /// </summary>
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<InfoBar, string>(nameof(Message));

    /// <summary>
    /// Defines the <see cref="Severity"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBarSeverity> SeverityProperty =
        AvaloniaProperty.Register<InfoBar, InfoBarSeverity>(nameof(Severity));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource?> IconSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, IconSource?>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="IsIconVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsIconVisible), true);

    /// <summary>
    /// Defines the <see cref="IsClosable"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsClosable), true);

    /// <summary>
    /// Defines the <see cref="CloseButtonCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CloseButtonCommandProperty =
        AvaloniaProperty.Register<InfoBar, ICommand>(nameof(CloseButtonCommand));

    /// <summary>
    /// Defines the <see cref="CloseButtonCommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> CloseButtonCommandParameterProperty =
        AvaloniaProperty.Register<InfoBar, object>(nameof(CloseButtonCommandParameter));

    /// <summary>
    /// Defines the <see cref="ActionButton"/> property
    /// </summary>
    public static readonly StyledProperty<Control> ActionButtonProperty =
        AvaloniaProperty.Register<InfoBar, Control>(nameof(ActionButton));

    /// <summary>
    /// Gets or sets a value that indicates whether the InfoBar is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the title of the InfoBar.
    /// </summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the message of the InfoBar.
    /// </summary>
    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of the InfoBar to apply consistent status color, icon, 
    /// and assistive technology settings dependent on the criticality of the notification.
    /// </summary>
    public InfoBarSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets the graphic content to appear alongside the title and message in the InfoBar.
    /// </summary>
    public IconSource? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the icon is visible in the InfoBar.
    /// </summary>
    public bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the user can close the InfoBar.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to invoke when the close button is clicked in the InfoBar.
    /// </summary>
    public ICommand CloseButtonCommand
    {
        get => GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the close button in the InfoBar.
    /// </summary>
    public object CloseButtonCommandParameter
    {
        get => GetValue(CloseButtonCommandParameterProperty);
        set => SetValue(CloseButtonCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the action button of the InfoBar.
    /// </summary>
    public Control ActionButton
    {
        get => GetValue(ActionButtonProperty);
        set => SetValue(ActionButtonProperty, value);
    }

    /// <summary>
    /// Occurs after the close button is clicked in the InfoBar.
    /// </summary>
    public event TypedEventHandler<InfoBar, EventArgs>? CloseButtonClick;

    /// <summary>
    /// Occurs just before the InfoBar begins to close.
    /// </summary>
    public event TypedEventHandler<InfoBar, InfoBarClosingEventArgs>? Closing;

    /// <summary>
    /// Occurs after the InfoBar is closed.
    /// </summary>
    public event TypedEventHandler<InfoBar, InfoBarClosedEventArgs>? Closed;

    private Button? _closeButton;

    private bool _appliedTemplate;
    private bool _notifyOpen;
    private bool _isVisible;

    private InfoBarCloseReason _lastCloseReason;
    
    private const string CLOSE_BUTTON = "CloseButton";

    private const string PC_SUCCESS = ":success";
    private const string PC_WARNING = ":warning";
    private const string PC_ERROR = ":error";
    private const string PC_INFORMATIONAL = ":informational";
    private const string PC_STANDARD_ICON = ":standardIcon";
    private const string PC_CLOSE_HIDDEN = ":closehidden";
    private const string PC_FOREGROUND_SET = ":foregroundset";
    
    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _appliedTemplate = false;
        _closeButton?.Click -= OnCloseButtonClick;

        base.OnApplyTemplate(e);

        _closeButton = e.NameScope.Find<Button>(CLOSE_BUTTON);
        if (_closeButton != null)
        {
            _closeButton.Click += OnCloseButtonClick;

            ToolTip.SetTip(_closeButton, LocalizationService.Instance.GetString("Close"));
        }

        _appliedTemplate = true;

        UpdateVisibility(_notifyOpen);
        _notifyOpen = false;

        UpdateSeverity();
        UpdateIcon();
        UpdateIconVisibility();
        UpdateCloseButton();
        UpdateForeground();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsOpenProperty)
        {
            if (change.GetNewValue<bool>())
            {
                _lastCloseReason = InfoBarCloseReason.Programmatic;
                UpdateVisibility();
            }
            else
            {
                RaiseClosingEvent();
            }
        }
        else if (change.Property == SeverityProperty)
        {
            UpdateSeverity();
        }
        else if (change.Property == IconSourceProperty)
        {
            UpdateIcon();
            UpdateIconVisibility();
        }
        else if (change.Property == IsIconVisibleProperty)
        {
            UpdateIconVisibility();
        }
        else if (change.Property == IsClosableProperty)
        {
            UpdateCloseButton();
        }
        else if (change.Property == TextElement.ForegroundProperty)
        {
            UpdateForeground();
        }
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        CloseButtonClick?.Invoke(this, EventArgs.Empty);
        _lastCloseReason = InfoBarCloseReason.CloseButton;
        IsOpen = false;
    }

    private void RaiseClosingEvent()
    {
        var args = new InfoBarClosingEventArgs(_lastCloseReason);

        Closing?.Invoke(this, args);

        if (!args.Cancel)
        {
            UpdateVisibility();
            RaiseClosedEvent();
        }
        else
        {
            // The developer has changed the Cancel property to true,
            // so we need to revert the IsOpen property to true.
            IsOpen = true;
        }
    }

    private void RaiseClosedEvent()
    {
        var args = new InfoBarClosedEventArgs(_lastCloseReason);
        Closed?.Invoke(this, args);
    }

    private void UpdateVisibility(bool notify = true, bool force = true)
    {
        if (!_appliedTemplate)
        {
            _notifyOpen = true;
        }
        else
        {
            if (force || IsOpen != _isVisible)
            {
                if (IsOpen)
                {
                    _isVisible = true;
                    PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, false);
                }
                else
                {
                    _isVisible = false;
                    PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, true);
                }
            }
        }
    }

    private void UpdateSeverity()
    {
        if (!_appliedTemplate)
            return; //Template not applied yet

        switch (Severity)
        {
            case InfoBarSeverity.Success:
                PseudoClasses.Set(PC_SUCCESS, true);
                PseudoClasses.Set(PC_WARNING, false);
                PseudoClasses.Set(PC_ERROR, false);
                PseudoClasses.Set(PC_INFORMATIONAL, false);
                break;

            case InfoBarSeverity.Warning:
                PseudoClasses.Set(PC_SUCCESS, false);
                PseudoClasses.Set(PC_WARNING, true);
                PseudoClasses.Set(PC_ERROR, false);
                PseudoClasses.Set(PC_INFORMATIONAL, false);
                break;

            case InfoBarSeverity.Error:
                PseudoClasses.Set(PC_SUCCESS, false);
                PseudoClasses.Set(PC_WARNING, false);
                PseudoClasses.Set(PC_ERROR, true);
                PseudoClasses.Set(PC_INFORMATIONAL, false);
                break;

            default: // default to informational
                PseudoClasses.Set(PC_SUCCESS, false);
                PseudoClasses.Set(PC_WARNING, false);
                PseudoClasses.Set(PC_ERROR, false);
                PseudoClasses.Set(PC_INFORMATIONAL, true);
                break;
        }
    }

    private void UpdateIcon()
    {
        // Skip this logic - used an IconSourceElement in the template instead
        // which automatically handles IconSource -> IconElement for us
    }

    private void UpdateIconVisibility()
    {
        if (!IsIconVisible)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, false);
            PseudoClasses.Set(PC_STANDARD_ICON, false);
        }
        else
        {
            bool hasUserIcon = IconSource != null;
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, hasUserIcon);
            PseudoClasses.Set(PC_STANDARD_ICON, !hasUserIcon);
        }
    }

    private void UpdateCloseButton()
    {
        PseudoClasses.Set(PC_CLOSE_HIDDEN, !IsClosable);
    }

    private void UpdateForeground()
    {
        PseudoClasses.Set(PC_FOREGROUND_SET, this.GetValue(TextElement.ForegroundProperty) != AvaloniaProperty.UnsetValue);
    }
}
