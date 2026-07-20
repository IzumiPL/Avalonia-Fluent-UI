using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Primitives;
using AvaloniaFluentUI.Windowing;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents and enhanced dialog with enhanced button, command, and progress support
/// </summary>
[PseudoClasses(s_pcHosted, s_pcHidden, SharedPseudoclasses.s_pcOpen)]
[PseudoClasses(SharedPseudoclasses.s_pcHeader, s_pcSubheader, SharedPseudoclasses.s_pcIcon, s_pcFooter, s_pcFooterAuto, s_pcExpanded)]
[PseudoClasses(s_pcProgress, s_pcProgressError, s_pcProgressSuspend)]
[PseudoClasses(s_pcHeaderForeground, s_pcIconForeground)]
[TemplatePart(s_tpButtonsHost, typeof(ItemsControl))]
[TemplatePart(s_tpCommandsHost, typeof(ItemsControl))]
[TemplatePart(s_tpMoreDetailsButton, typeof(Button))]
[TemplatePart(s_tpProgressBar, typeof(ProgressBar))]
public partial class TaskDialog : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> property
    /// </summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TaskDialog, string>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Header"/> property
    /// </summary>
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<TaskDialog, string>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="SubHeader"/> property
    /// </summary>
    public static readonly StyledProperty<string> SubHeaderProperty =
        AvaloniaProperty.Register<TaskDialog, string>(nameof(SubHeader));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        AvaloniaProperty.Register<TaskDialog, IconSource>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="Buttons"/> property
    /// </summary>
    public static readonly DirectProperty<TaskDialog, IList<TaskDialogButton>> ButtonsProperty =
        AvaloniaProperty.RegisterDirect<TaskDialog, IList<TaskDialogButton>>(nameof(Buttons),
            x => x.Buttons, (x,v) => x.Buttons = v);

    /// <summary>
    /// Defines the <see cref="Commands"/> property
    /// </summary>
    public static readonly DirectProperty<TaskDialog, IList<TaskDialogCommand>> CommandsProperty =
        AvaloniaProperty.RegisterDirect<TaskDialog, IList<TaskDialogCommand>>(nameof(Commands),
            x => x.Commands, (x, v) => x.Commands = v);

    /// <summary>
    /// Defines the <see cref="FooterVisibility"/> property
    /// </summary>
    public static readonly StyledProperty<TaskDialogFooterVisibility> FooterVisibilityProperty =
        AvaloniaProperty.Register<TaskDialog, TaskDialogFooterVisibility>(nameof(FooterVisibility));

    /// <summary>
    /// Defines the <see cref="IsFooterExpanded"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsFooterExpandedProperty =
        AvaloniaProperty.Register<TaskDialog, bool>(nameof(IsFooterExpanded));

    /// <summary>
    /// Defines the <see cref="Footer"/> property
    /// </summary>
    public static readonly StyledProperty<object> FooterProperty =
        AvaloniaProperty.Register<TaskDialog, object>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="FooterTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> FooterTemplateProperty =
        AvaloniaProperty.Register<TaskDialog, IDataTemplate>(nameof(FooterTemplate));

    /// <summary>
    /// Defines the <see cref="ShowProgressBar"/> property
    /// </summary>
    public static readonly StyledProperty<bool> ShowProgressBarProperty =
        AvaloniaProperty.Register<TaskDialog, bool>(nameof(ShowProgressBar));

    /// <summary>
    /// Defines the <see cref="HeaderBackground"/> property
    /// </summary>
    public static readonly StyledProperty<IBrush> HeaderBackgroundProperty =
        AvaloniaProperty.Register<TaskDialog, IBrush>(nameof(HeaderBackground));

    /// <summary>
    /// Defines the <see cref="HeaderForeground"/> property
    /// </summary>
    public static readonly StyledProperty<IBrush> HeaderForegroundProperty =
        AvaloniaProperty.Register<TaskDialog, IBrush>(nameof(HeaderForeground));

    /// <summary>
    /// Defines the <see cref="IconForeground"/> property
    /// </summary>
    public static readonly StyledProperty<IBrush> IconForegroundProperty =
        AvaloniaProperty.Register<TaskDialog, IBrush>(nameof(IconForeground));

    /// <summary>
    /// Gets or sets the title of the dialog
    /// </summary>
    /// <remarks>
    /// This is the window caption of the dialog displayed in the title bar. For platforms 
    /// where windowing is not supported, this property has no effect.
    /// </remarks>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog header text
    /// </summary>
    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog sub header text
    /// </summary>
    public string SubHeader
    {
        get => GetValue(SubHeaderProperty);
        set => SetValue(SubHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog Icon
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets the list of buttons that display at the bottom of the TaskDialog
    /// </summary>
    public IList<TaskDialogButton> Buttons
    {
        get => _buttons;
        set => SetAndRaise(ButtonsProperty, ref _buttons, value);
    }

    /// <summary>
    /// Gets the list of Commands displayed in the TaskDialog
    /// </summary>
    public IList<TaskDialogCommand> Commands
    {
        get => _commands;
        set => SetAndRaise(CommandsProperty, ref _commands, value);
    }

    /// <summary>
    /// Gets or sets the visibility of the Footer area
    /// </summary>
    public TaskDialogFooterVisibility FooterVisibility
    {
        get => GetValue(FooterVisibilityProperty);
        set => SetValue(FooterVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the footer is visible
    /// </summary>
    public bool IsFooterExpanded
    {
        get => GetValue(IsFooterExpandedProperty);
        set => SetValue(IsFooterExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the footer content
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the IDataTemplate for the footer content
    /// </summary>
    public IDataTemplate FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this TaskDialog shows a progress bar
    /// </summary>
    public bool ShowProgressBar
    {
        get => GetValue(ShowProgressBarProperty);
        set => SetValue(ShowProgressBarProperty, value);
    }

    /// <summary>
    /// Gets or sets the background of the header region of the task dialog
    /// </summary>
    public IBrush HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground of the header text for the TaskDialog
    /// </summary>
    public IBrush HeaderForeground
    {
        get => GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground of the <see cref="IconSource"/> for the TaskDialog
    /// </summary>
    public IBrush IconForeground
    {
        get => GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the root visual that should host this dialog
    /// </summary>
    /// <remarks>
    /// For TaskDialogs declared in Xaml, this is automatically set. If you declare a 
    /// TaskDialog in C#, you MUST set this property before showing the dialog to prevent
    /// and error. For desktop platforms, set it to the Window that should own the dialog.
    /// For others, set it to the root TopLevel.
    /// </remarks>
    public Visual XamlRoot { get; set; }

    /// <summary>
    /// Raised when the TaskDialog is beginning to open, but is not yet visible
    /// </summary>
    public event TypedEventHandler<TaskDialog, EventArgs> Opening;

    /// <summary>
    /// Raised when the TaskDialog is opened and ready to be shown on screen
    /// </summary>
    public event TypedEventHandler<TaskDialog, EventArgs> Opened;

    /// <summary>
    /// Raised when the TaskDialog is beginning to close
    /// </summary>
    public event TypedEventHandler<TaskDialog, TaskDialogClosingEventArgs> Closing;

    /// <summary>
    /// Raised when the TaskDialog is closed
    /// </summary>
    public event TypedEventHandler<TaskDialog, EventArgs> Closed;

    private IList<TaskDialogButton> _buttons;
    private IList<TaskDialogCommand> _commands;

    private const string s_tpButtonsHost = "ButtonsHost";
    private const string s_tpCommandsHost = "CommandsHost";
    private const string s_tpProgressBar = "ProgressBar";
    private const string s_tpMoreDetailsButton = "MoreDetailsButton";

    private const string s_pcHidden = ":hidden";
    private const string s_pcHosted = ":hosted";
    private const string s_pcSubheader = ":subheader";
    private const string s_pcFooter = ":footer";
    private const string s_pcFooterAuto = ":footerAuto";
    private const string s_pcExpanded = ":expanded";
    private const string s_pcProgress = ":progress";
    private const string s_pcProgressError = ":progressError";
    private const string s_pcProgressSuspend = ":progressSuspend";
    private const string s_pcHeaderForeground = ":headerForeground";
    private const string s_pcIconForeground = ":iconForeground";

    private const string s_cFATDCom = "FA_TaskDialogCommand";
    
    public TaskDialog()
    {
        PseudoClasses.Add(s_pcHidden);
        _buttons = new List<TaskDialogButton>();
        _commands = new List<TaskDialogCommand>();

        AddHandler(Button.ClickEvent, OnButtonClick, RoutingStrategies.Bubble, true);
        AddHandler(KeyDownEvent, OnKeyDownPreview, RoutingStrategies.Tunnel, true);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_moreDetailsButton != null)
        {
            _moreDetailsButton.Click -= MoreDetailsButtonClick;
        }

        base.OnApplyTemplate(e);

        _buttonsHost = e.NameScope.Get<ItemsControl>(s_tpButtonsHost);
        _commandsHost = e.NameScope.Get<ItemsControl>(s_tpCommandsHost);

        _moreDetailsButton = e.NameScope.Find<Button>(s_tpMoreDetailsButton);

        _progressBar = e.NameScope.Find<ProgressBar>(s_tpProgressBar);

        if (_moreDetailsButton != null)
        {
            _moreDetailsButton.Click += MoreDetailsButtonClick;
        }        
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FooterVisibilityProperty)
        {
            var val = change.GetNewValue<TaskDialogFooterVisibility>();

            PseudoClasses.Set(s_pcFooterAuto, val == TaskDialogFooterVisibility.Auto);
            PseudoClasses.Set(s_pcFooter, val != TaskDialogFooterVisibility.Never);
            PseudoClasses.Set(s_pcExpanded, val == TaskDialogFooterVisibility.Always);
        }
        else if (change.Property == IsFooterExpandedProperty)
        {
            if (FooterVisibility != TaskDialogFooterVisibility.Always)
                PseudoClasses.Set(s_pcExpanded, change.GetNewValue<bool>());
        }
        else if (change.Property == ShowProgressBarProperty)
        {
            PseudoClasses.Set(s_pcProgress, change.GetNewValue<bool>());
        }
        else if (change.Property == IconSourceProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
        }
        else if (change.Property == HeaderProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcHeader, change.NewValue != null);
        }
        else if (change.Property == SubHeaderProperty)
        {
            PseudoClasses.Set(s_pcSubheader, change.NewValue != null);
        }
        else if (change.Property == HeaderForegroundProperty)
        {
            PseudoClasses.Set(s_pcHeaderForeground, change.NewValue != null);
        }
        else if (change.Property == IconForegroundProperty)
        {
            PseudoClasses.Set(s_pcIconForeground, change.NewValue != null);
        }
    }
    
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // GH #539
        // If TaskDialog is declared in Xaml, OnLoaded will get called when the Xaml is loaded, which is not 
        // desirable (particularly the focus part). Only run this code and set focus if we're opening
        if (_isOpening)
        {
            SetButtons();
            SetCommands();
            TrySetInitialFocus();
        }       
    }

    private void OnKeyDownPreview(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Hide();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            if (_defaultButton != null && _defaultButton.DataContext is TaskDialogButton b)
            {
                if (b.Command?.CanExecute(b.CommandParameter) == true)
                {
                    b.Command.Execute(b.CommandParameter);
                }

                b.RaiseClick();

                if (b is TaskDialogCommand com && !com.ClosesOnInvoked)
                    return;

                CloseCore(b.DialogResult);
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Shows the TaskDialog
    /// </summary>
    /// <param name="showHosted">Optional parameter that specifies whether this dialog should show in the OverlayLayer even on windowing platforms. Defaults to false</param>
    /// <returns>The TaskDialog result corresponding to the command/button used to close the dialog</returns>
    /// <remarks>
    /// Before calling this method, you MUST set <see cref="XamlRoot"/> property to the TopLevel/Window that should
    /// own or host this Dialog. If you declare the dialog in Xaml, this is done automatically since 
    /// the dialog is already attached to the visual tree
    /// </remarks>
    public async Task<object> ShowAsync(bool showHosted = false)
    {
        bool declaredInXaml = this.IsAttachedToVisualTree();
        if (!declaredInXaml && XamlRoot == null)
        {
            throw new InvalidOperationException("XamlRoot not set on TaskDialog. This should be set to the TopLevel that should own or host the dialog.");
        }

        // See OnLoaded
        _isOpening = true;

        OnOpening();
        
        var owner = XamlRoot ?? VisualRoot as Visual;

        void UnparentDialog()
        {
            _xamlOwner = (Control)Parent;
            if (_xamlOwner is Panel p)
            {
                _xamlOwnerChildIndex = p.Children.IndexOf(this);
                p.Children.RemoveAt(_xamlOwnerChildIndex);
            }
            else if (_xamlOwner is ContentControl icc)
            {
                icc.Content = null;
            }
            else if (_xamlOwner is ContentPresenter icp)
            {
                icp.Content = null;
            }
            else if (_xamlOwner is Decorator d)
            {
                d.Child = null;
            }
        }

        object result = null;
        _previousFocus = TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement();

        if (showHosted || !(owner is WindowBase))
        {
            // Hosted in OverlayLayer
            _tcs = new TaskCompletionSource<object>();
            if (declaredInXaml)
            {
                UnparentDialog();
            }

            var host = new DialogHost
            {
                Content = this
            };
            _host = host;

            var overlayLayer = OverlayLayer.GetOverlayLayer(owner);
            if (overlayLayer == null)
                throw new InvalidOperationException("Unable to find OverlayLayer for hosting the TaskDialog");

            overlayLayer.Children.Add(host);
            PseudoClasses.Set(s_pcHosted, true);
            IsVisible = true;

            // v2 - Added this so dialog materializes in the Visual Tree now since for some reason
            //      items in the OverlayLayer materialize at the absolute last moment making init
            //      a very difficult task to do
            // v2-preview6: This doesn't appear necessary anymore...will preserve this for now
            // but has to be removed to solve GH#315
            //(overlayLayer.GetVisualRoot() as ILayoutRoot).LayoutManager.ExecuteInitialLayoutPass();

            OnOpened();

            PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, true);
            PseudoClasses.Set(s_pcHidden, false);

            result = await _tcs.Task;
        }
        else
        {
            if (declaredInXaml)
            {
                UnparentDialog();
            }

            PseudoClasses.Set(s_pcHidden, false);
            PseudoClasses.Set(s_pcHosted, false);

            var host = new FluentWindow()
            {
                CanResize = false,
                MaxButtonIsVisible = false,
                MinButtonIsVisible = false,
                CanMinimize = false,
                CanMaximize = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowAsDialog = true,
                Content = this,
                MinWidth = 100,
                MinHeight = 100
            };

            if (_host == null)
            {
                host[!Window.TitleProperty] = this[!TitleProperty];
                host.Opened += (s, e) =>
                {
                    OnOpened();

                    TrySetInitialFocus();
                };
                host.Closing += (s, e) =>
                {
                    if (_ignoreWindowClosingEvent)
                        return;

                    // Cancel the window event now, and we'll use our normal closing logic to determine
                    // if we should actually cancel
                    e.Cancel = true;
                    CloseCore(TaskDialogStandardResult.None);
                };
            }            
            
            _host = host;
            IsVisible = true;

            result = await host.ShowDialog<object>(owner as Window);
        }

        _isOpening = false;
        OnClosed();
        _host = null;

        _previousFocus?.Focus();

        return result ?? TaskDialogStandardResult.None;
    }

    /// <summary>
    /// Hides the TaskDialog with a <see cref="TaskDialogStandardResult.None"/> result
    /// </summary>
    public void Hide()
    {
        CloseCore(TaskDialogStandardResult.None);
    }

    /// <summary>
    /// Hides the dialog with the specified dialog result
    /// </summary>
    public void Hide(object result)
    {
        CloseCore(result);
    }

    protected virtual void OnOpening()
    {
        Opening?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnOpened()
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClosing(TaskDialogClosingEventArgs args)
    {
        Closing?.Invoke(this, args);
    }

    protected virtual void OnClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void CloseCore(object result)
    {
        if (_hasDeferralActive)
            return;

        var args = new TaskDialogClosingEventArgs(result);

        var deferral = new Deferral(() =>
        {
            Dispatcher.UIThread.VerifyAccess();
            _hasDeferralActive = false;
            if (args.Cancel)
                return;

            FinalCloseDialog(result);
        });

        args.SetDeferral(deferral);

        _hasDeferralActive = true;
        args.IncrementDeferralCount();
        OnClosing(args);
        args.DecrementDeferralCount();
    }

    private async void FinalCloseDialog(object result)
    {
        void ReturnDialogToParent()
        {
            if (_xamlOwner == null)
                return;

            if (_xamlOwner is Panel p)
            {
                p.Children.Insert(_xamlOwnerChildIndex, this);
            }
            else if (_xamlOwner is Decorator d)
            {
                d.Child = this;
            }
            else if (_xamlOwner is ContentControl icc)
            {
                icc.Content = this;
            }
            else if (_xamlOwner is ContentPresenter icp)
            {
                icp.Content = this;
            }
        }

        if (_host is Window w)
        {            
            _ignoreWindowClosingEvent = true;

            w.Close(result);
            IsVisible = false;

            w.Content = null;
            ReturnDialogToParent();

            PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, false);
            PseudoClasses.Set(s_pcHidden, true);

            _ignoreWindowClosingEvent = false;
        }
        else if (_host is DialogHost dh)
        {
            IsHitTestVisible = false;

            Focus();

            PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, false);
            PseudoClasses.Set(s_pcHidden, true);

            // Let the close animation finish (now 0.167s in new WinUI update...)
            // We'll wait just a touch longer to be sure
            await Task.Delay(200);

            IsHitTestVisible = true;
            IsVisible = false;

            dh.Content = null;
            ReturnDialogToParent();

            var overlayLayer = OverlayLayer.GetOverlayLayer(dh);
            // If OverlayLayer isn't found here, this may be a reentrant call (hit ESC multiple times quickly, etc)
            // Don't fail, and return. If this isn't reentrant, there's bigger issues...
            if (overlayLayer == null)
                return;

            overlayLayer.Children.Remove(dh);

            _tcs.TrySetResult(result);
        }
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (_hasDeferralActive)
            return;

        // TaskDialogCommandHost is a TaskDialogButtonHost, this captures everything
        if (e.Source is Visual v && v.FindAncestorOfType<TaskDialogButtonHost>(true) is TaskDialogButtonHost b)
        {
            // DataContext for the hosts are the user defined buttons/commands, get the dialog from that
            if (b.DataContext is TaskDialogControl tdb)
            {
                if (tdb is TaskDialogCommand com && !com.ClosesOnInvoked)
                    return;

                Hide(tdb.DialogResult);
            }
        }
    }

    public void SetProgressBarState(double value, TaskDialogProgressState state)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_progressBar != null)
            {
                _progressBar.Value = value;

                _progressBar.IsIndeterminate = (state & TaskDialogProgressState.Indeterminate) == TaskDialogProgressState.Indeterminate;

                if (_currentProgressState != state)
                {
                    _currentProgressState = state;

                    PseudoClasses.Set(s_pcProgressError, (state & TaskDialogProgressState.Error) == TaskDialogProgressState.Error);
                    PseudoClasses.Set(s_pcProgressSuspend, (state & TaskDialogProgressState.Suspended) == TaskDialogProgressState.Suspended);
                }
            }
        });
    }

    private void MoreDetailsButtonClick(object sender, RoutedEventArgs e)
    {
        IsFooterExpanded = !IsFooterExpanded;
    }

    private void SetButtons()
    {
        if (_buttons == null)
            return;

        List<TaskDialogButtonHost> buttons = new List<TaskDialogButtonHost>();
        bool foundDefault = false;
        for (int i = 0; i < _buttons.Count; i++)
        {
            var button = _buttons[i];
            var b = new TaskDialogButtonHost
            {
                [!ContentProperty] = _buttons[i][!TaskDialogControl.TextProperty],
                [!TaskDialogButtonHost.IconSourceProperty] = button[!TaskDialogButton.IconSourceProperty],
                DataContext = button,
                [!IsEnabledProperty] = button[!TaskDialogControl.IsEnabledProperty],
                [!Button.CommandParameterProperty] = button[!TaskDialogButton.CommandParameterProperty],
                [!Button.CommandProperty] = button[!TaskDialogButton.CommandProperty]
            };

            if (button.IsDefault)
            {
                if (foundDefault)
                    throw new InvalidOperationException("Cannot set 'IsDefault' property on more than one item in a TaskDialog");

                foundDefault = true;
                b.Classes.Add(SharedPseudoclasses.s_cAccent);
                _defaultButton = b;
            }
            buttons.Add(b);
        }

        _buttonsHost.ItemsSource = buttons;
    }

    private void SetCommands()
    {
        if (_commands == null)
            return;

        List<Control> commands = new List<Control>();

        bool foundDefault = _defaultButton != null;
        int iconCount = 0;
        int normalCommandCount = 0;
        for (int i = 0; i < _commands.Count; i++)
        {
            if (_commands[i] is TaskDialogCheckBox tdcb)
            {
                var com = new CheckBox
                {
                    [!ContentProperty] = tdcb[!TaskDialogControl.TextProperty],
                    DataContext = tdcb,
                    [!IsEnabledProperty] = tdcb[!TaskDialogControl.IsEnabledProperty],
                    [!ToggleButton.IsCheckedProperty] = tdcb[!TaskDialogRadioButton.IsCheckedProperty]
                };

                com.Classes.Add(s_cFATDCom);

                commands.Add(com);                
            }
            else if (_commands[i] is TaskDialogRadioButton tdrb)
            {
                var com = new RadioButton
                {
                    [!ContentProperty] = tdrb[!TaskDialogControl.TextProperty],
                    DataContext = tdrb,
                    [!IsEnabledProperty] = tdrb[!TaskDialogControl.IsEnabledProperty],
                    [!ToggleButton.IsCheckedProperty] = tdrb[!TaskDialogRadioButton.IsCheckedProperty]
                };

                com.Classes.Add(s_cFATDCom);

                commands.Add(com);
            }
            else if (_commands[i] is TaskDialogCommand tdc)
            {
                var com = new TaskDialogCommandHost
                {
                    [!ContentProperty] = tdc[!TaskDialogControl.TextProperty],
                    DataContext = tdc,
                    [!IsEnabledProperty] = tdc[!TaskDialogControl.IsEnabledProperty],
                    [!Button.CommandParameterProperty] = tdc[!TaskDialogButton.CommandParameterProperty],
                    [!Button.CommandProperty] = tdc[!TaskDialogButton.CommandProperty],
                    [!TaskDialogButtonHost.IconSourceProperty] = tdc[!TaskDialogButton.IconSourceProperty]
                };

                if (tdc.IsDefault)
                {
                    if (foundDefault)
                        throw new InvalidOperationException("Cannot set 'IsDefault' property on more than one item in a TaskDialog");

                    foundDefault = true;
                    com.Classes.Add(SharedPseudoclasses.s_cAccent);
                    _defaultButton = com;
                }

                commands.Add(com);

                // Icons are only supported on "normal" TaskDialogCommands
                if (tdc.IconSource != null)
                    iconCount++;
                normalCommandCount++;
            }
        }

        if (iconCount != normalCommandCount)
        {
            // We have an item with no icon - force it to display as if one
            // was present so that its aligned with the others
            for (int i = 0; i < commands.Count; i++)
            {
                (commands[i].Classes as IPseudoClasses).Set(SharedPseudoclasses.s_pcIcon, true);
            }
        }

        _commandsHost.ItemsSource = commands;
    }

    private void TrySetInitialFocus()
    {
        var curFocus = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement() as Control;
        bool setFocus = false;
        if (curFocus?.FindAncestorOfType<TaskDialog>() == null)
        {
            setFocus = true;
        }

        // User requested something to be focused, don't override their choice
        if (!setFocus)
            return;

        // Default button gets priority focus
        if (_defaultButton != null)
        {
            _defaultButton.Focus();
#if DEBUG
            Logger.TryGet(LogEventLevel.Debug, "TaskDialog")?.Log("TrySetInitialFocus", "Set initial focus to requested DefaultButton");
#endif
        }
        else
        {
            var fm = TopLevel.GetTopLevel(this).FocusManager;
            // TODO: v3 - does this work?
            var next = FocusManager.FindFirstFocusableElement(this);
            if (next != null)
            {
                next.Focus();
            }
            else
            {
                this.Focus();
            }

#if DEBUG
            Logger.TryGet(LogEventLevel.Debug, "TaskDialog")?.Log("TrySetInitialFocus", "Set initial focus to {next}", next);
#endif
        }
    }

    private ItemsControl _buttonsHost;
    private ItemsControl _commandsHost;
    private ProgressBar _progressBar;
    private Button _moreDetailsButton;

    private TaskDialogProgressState _currentProgressState = TaskDialogProgressState.Normal;

    private Button _defaultButton;

    public Control _xamlOwner;
    private int _xamlOwnerChildIndex;
    private Control _host;
    private TaskCompletionSource<object> _tcs;
    internal bool _hasDeferralActive = false;

    private IInputElement _previousFocus;
    private bool _ignoreWindowClosingEvent;
    private bool _isOpening;
}
