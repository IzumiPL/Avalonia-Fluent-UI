using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Presents a asynchronous dialog to the user.
/// </summary>

[PseudoClasses(SharedPseudoclasses.s_pcHidden, SharedPseudoclasses.s_pcOpen)]
[PseudoClasses(s_pcPrimary, s_pcSecondary, s_pcClose)]
[PseudoClasses(s_pcFullSize)]
[TemplatePart(s_tpPrimaryButton, typeof(Button))]
[TemplatePart(s_tpSecondaryButton, typeof(Button))]
[TemplatePart(s_tpCloseButton, typeof(Button))]
public partial class ContentDialog : ContentControl, ICustomKeyboardNavigation
{
    /// <summary>
    /// Defines the <see cref="CloseButtonCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CloseButtonCommandProperty =
        AvaloniaProperty.Register<ContentDialog, ICommand>(nameof(CloseButtonCommand));

    /// <summary>
    /// Defines the <see cref="CloseButtonCommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> CloseButtonCommandParameterProperty =
        AvaloniaProperty.Register<ContentDialog, object>(nameof(CloseButtonCommandParameter));

    /// <summary>
    /// Defines the <see cref="CloseButtonText"/> property
    /// </summary>
    public static readonly StyledProperty<string> CloseButtonTextProperty =
        AvaloniaProperty.Register<ContentDialog, string>(nameof(CloseButtonText));

    /// <summary>
    /// Defines the <see cref="DefaultButton"/> property
    /// </summary>
    public static readonly StyledProperty<ContentDialogButton> DefaultButtonProperty =
        AvaloniaProperty.Register<ContentDialog, ContentDialogButton>(nameof(DefaultButton), ContentDialogButton.None);

    /// <summary>
    /// Defines the <see cref="IsPrimaryButtonEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsPrimaryButtonEnabledProperty =
        AvaloniaProperty.Register<ContentDialog, bool>(nameof(IsPrimaryButtonEnabled), true);

    /// <summary>
    /// Defines the <see cref="IsSecondaryButtonEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsSecondaryButtonEnabledProperty =
        AvaloniaProperty.Register<ContentDialog, bool>(nameof(IsSecondaryButtonEnabled), true);

    /// <summary>
    /// Defines the <see cref="PrimaryButtonCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> PrimaryButtonCommandProperty =
        AvaloniaProperty.Register<ContentDialog, ICommand>(nameof(PrimaryButtonCommand));

    /// <summary>
    /// Defines the <see cref="PrimaryButtonCommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> PrimaryButtonCommandParameterProperty =
        AvaloniaProperty.Register<ContentDialog, object>(nameof(PrimaryButtonCommandParameter));

    /// <summary>
    /// Defines the <see cref="PrimaryButtonText"/> property
    /// </summary>
    public static readonly StyledProperty<string> PrimaryButtonTextProperty =
        AvaloniaProperty.Register<ContentDialog, string>(nameof(PrimaryButtonText));

    /// <summary>
    /// Defines the <see cref="SecondaryButtonCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> SecondaryButtonCommandProperty =
        AvaloniaProperty.Register<ContentDialog, ICommand>(nameof(SecondaryButtonCommand));

    /// <summary>
    /// Defines the <see cref="SecondaryButtonCommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> SecondaryButtonCommandParameterProperty =
        AvaloniaProperty.Register<ContentDialog, object>(nameof(SecondaryButtonCommandParameter));

    /// <summary>
    /// Defines the <see cref="SecondaryButtonText"/> property
    /// </summary>
    public static readonly StyledProperty<string> SecondaryButtonTextProperty =
        AvaloniaProperty.Register<ContentDialog, string>(nameof(SecondaryButtonText));

    /// <summary>
    /// Defines the <see cref="Title"/> property
    /// </summary>
    public static readonly StyledProperty<object> TitleProperty =
        AvaloniaProperty.Register<ContentDialog, object>(nameof(Title), "");

    /// <summary>
    /// Defines the <see cref="TitleTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> TitleTemplateProperty =
        AvaloniaProperty.Register<ContentDialog, IDataTemplate>(nameof(TitleTemplate));

    /// <summary>
    /// Defines the <see cref="FullSizeDesired"/> property
    /// </summary>
    public static readonly StyledProperty<bool> FullSizeDesiredProperty =
        AvaloniaProperty.Register<ContentDialog, bool>(nameof(FullSizeDesired));

    /// <summary>
    /// Gets or sets the command to invoke when the close button is tapped.
    /// </summary>
    public ICommand CloseButtonCommand
    {
        get => GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the close button.
    /// </summary>
    public object CloseButtonCommandParameter
    {
        get => GetValue(CloseButtonCommandParameterProperty);
        set => SetValue(CloseButtonCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string CloseButtonText
    {
        get => GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates which button on the dialog is the default action.
    /// </summary>
    public ContentDialogButton DefaultButton
    {
        get => GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dialog's primary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled
    {
        get => GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dialog's secondary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled
    {
        get => GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to invoke when the primary button is tapped.
    /// </summary>
    public ICommand PrimaryButtonCommand
    {
        get => GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the primary button.
    /// </summary>
    public object PrimaryButtonCommandParameter
    {
        get => GetValue(PrimaryButtonCommandParameterProperty);
        set => SetValue(PrimaryButtonCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string PrimaryButtonText
    {
        get => GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to invoke when the secondary button is tapped.
    /// </summary>
    public ICommand SecondaryButtonCommand
    {
        get => GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the secondary button.
    /// </summary>
    public object SecondaryButtonCommandParameter
    {
        get => GetValue(SecondaryButtonCommandParameterProperty);
        set => SetValue(SecondaryButtonCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string SecondaryButtonText
    {
        get => GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    public object Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the title template.
    /// </summary>
    public IDataTemplate TitleTemplate
    {
        get => GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the Dialog should show full screen
    /// On WinUI3, at least desktop, this just show the dialog at 
    /// the maximum size of a contentdialog.
    /// </summary>
    public bool FullSizeDesired
    {
        get => GetValue(FullSizeDesiredProperty);
        set => SetValue(FullSizeDesiredProperty, value);
    }

    /// <summary>
    /// Occurs before the dialog is opened
    /// </summary>
    public event TypedEventHandler<ContentDialog, EventArgs> Opening;

    /// <summary>
    /// Occurs after the dialog is opened.
    /// </summary>
    public event TypedEventHandler<ContentDialog, EventArgs> Opened;

    /// <summary>
    /// Occurs after the dialog starts to close, but before it is closed and before the Closed event occurs.
    /// </summary>
    public event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> Closing;

    /// <summary>
    /// Occurs after the dialog is closed.
    /// </summary>
    public event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> Closed;

    /// <summary>
    /// Occurs after the primary button has been tapped.
    /// </summary>
    public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> PrimaryButtonClick;

    /// <summary>
    /// Occurs after the secondary button has been tapped.
    /// </summary>
    public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> SecondaryButtonClick;

    /// <summary>
    /// Occurs after the close button has been tapped.
    /// </summary>
    public event TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> CloseButtonClick;


    private const string s_tpPrimaryButton = "PrimaryButton";
    private const string s_tpSecondaryButton = "SecondaryButton";
    private const string s_tpCloseButton = "CloseButton";

    private const string s_pcPrimary = ":primary";
    private const string s_pcSecondary = ":secondary";
    private const string s_pcClose = ":close";
    private const string s_pcFullSize = ":fullsize";
    
    public ContentDialog()
    {
        PseudoClasses.Add(SharedPseudoclasses.s_pcHidden);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_primaryButton != null)
            _primaryButton.Click -= OnButtonClick;
        if (_secondaryButton != null)
            _secondaryButton.Click -= OnButtonClick;
        if (_closeButton != null)
            _closeButton.Click -= OnButtonClick;

        base.OnApplyTemplate(e);

        _primaryButton = e.NameScope.Get<Button>(s_tpPrimaryButton);
        _primaryButton.Click += OnButtonClick;
        _secondaryButton = e.NameScope.Get<Button>(s_tpSecondaryButton);
        _secondaryButton.Click += OnButtonClick;
        _closeButton = e.NameScope.Get<Button>(s_tpCloseButton);
        _closeButton.Click += OnButtonClick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FullSizeDesiredProperty)
        {
            OnFullSizedDesiredChanged(change);
        }
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "Content")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // See OnKeyUp for reasoning
        if (!e.Handled && (e.Key == Key.Enter || e.Key == Key.Escape))
        {
            _hotkeyDownVisual = e.Source as Visual;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Handled)
        {
            base.OnKeyUp(e);
            return;
        }

        // HACK: If a default button is set, and the content dialog is opened by enter or escape key press on a button
        // the KeyUp event is raised on the focused item within the dialog, instead of the original button
        // (Avalonia related issue #9626) and will immediately close the dialog
        // We store the source of the key down and if it doesn't match, or hasn't been set yet, we ignore
        // this key up event so we don't inadvertantly close the dialog
        if ((e.Key == Key.Enter || e.Key == Key.Escape) && (_hotkeyDownVisual == null || _hotkeyDownVisual != (Visual)e.Source))
        {
            base.OnKeyUp(e);
            return;
        }

        switch (e.Key)
        {
            case Key.Escape:
                HideCore();
                e.Handled = true;
                break;

            case Key.Enter:
                var defButton = DefaultButton;

                // v2 - Only handle 'Enter' if the default button is set
                //      Otherwise, we'll let the event go as normal - if focus is currently
                //      on a button, 'Enter' should invoke that button
                if (defButton != ContentDialogButton.None)
                {
                    switch (defButton)
                    {
                        case ContentDialogButton.Primary:
                            OnButtonClick(_primaryButton, null);
                            break;

                        case ContentDialogButton.Secondary:
                            OnButtonClick(_secondaryButton, null);
                            break;

                        case ContentDialogButton.Close:
                            OnButtonClick(_closeButton, null);
                            break;
                    }
                    e.Handled = true;
                }

                break;
        }
        base.OnKeyUp(e);
    }

    /// <summary>
    /// Begins an asynchronous operation to show the dialog.
    /// </summary>
    public Task<ContentDialogResult> ShowAsync() => ShowAsyncCoreForTopLevel(null);

    /// <summary>
    /// Begins an asynchronous operation to show the dialog using the specified window
    /// </summary>
    public Task<ContentDialogResult> ShowAsync(Window w) => ShowAsyncCoreForTopLevel(w);

    /// <summary>
    /// Begins an asynchronous operation to show the dialog using the specified top level
    /// </summary>
    /// <remarks>
    /// Use this when an ApplicationLifetime is unavailable (such as in headless unit tests)
    /// </remarks>
    public Task<ContentDialogResult> ShowAsync(TopLevel tl) => ShowAsyncCoreForTopLevel(tl);

    /// <summary>
    /// Shows the content dialog on the specified window asynchronously.
    /// </summary>
    /// <remarks>
    /// Note that the placement parameter is not implemented and only accepts <see cref="ContentDialogPlacement.Popup"/>
    /// </remarks>
    private Task<ContentDialogResult> ShowAsyncCore(Window window, ContentDialogPlacement placement = ContentDialogPlacement.Popup) =>
        ShowAsyncCoreForTopLevel((TopLevel)window);

    private async Task<ContentDialogResult> ShowAsyncCoreForTopLevel(TopLevel topLevel)
    {
        _tcs = new TaskCompletionSource<ContentDialogResult>();

        OnOpening();

        if (Parent != null)
        {
            _originalHost = (Control)Parent;
            switch (_originalHost)
            {
                case Panel p:
                    _originalHostIndex = p.Children.IndexOf(this);
                    p.Children.Remove(this);
                    break;
                case Decorator d:
                    d.Child = null;
                    break;
                case ContentControl cc:
                    cc.Content = null;
                    break;
                case ContentPresenter cp:
                    cp.Content = null;
                    break;
            }
        }

        _host ??= new DialogHost();

        _host.Content = this;

        OverlayLayer ol = null;

        if (topLevel != null)
        {
            ol = OverlayLayer.GetOverlayLayer(topLevel);
        }
        else
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime al)
            {
                var windows = al.Windows;
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].IsActive)
                    {
                        topLevel = windows[i];
                        break;
                    }
                }

                if (topLevel == null)
                {
                    if (al.MainWindow == null)
                        throw new NotSupportedException("No TopLevel root found to parent ContentDialog");

                    topLevel = al.MainWindow;
                }

                ol = OverlayLayer.GetOverlayLayer(topLevel);
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime sl)
            {
                topLevel = TopLevel.GetTopLevel(sl.MainView);
                ol = OverlayLayer.GetOverlayLayer(sl.MainView);
            }
            else
            {
                throw new InvalidOperationException("No TopLevel found for ContentDialog and no ApplicationLifetime is set. " +
                    "Please either supply a valid ApplicationLifetime or TopLevel to ShowAsync()");
            }
        }

        if (ol == null)
            throw new InvalidOperationException("Unable to find OverlayLayer from given TopLevel");

        _lastFocus = topLevel.FocusManager.GetFocusedElement();

        ol.Children.Add(_host);

        // Make the dialog visible
        IsVisible = true;
        PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, false);
        PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, true);

        // Delay futher initializing until after the dialog has loaded. We sub here and unsbu in DialogLoaded
        // because ContentDialog can be declared in Xaml prior to showing, and we don't want Loaded to trigger
        // initialization if we're not actually showing the dialog
        Loaded += DialogLoaded;

        return await _tcs.Task;
    }

    /// <summary>
    /// Closes the current <see cref="ContentDialog"/> without a result (<see cref="ContentDialogResult"/>.<see cref="ContentDialogResult.None"/>)
    /// </summary>
    public void Hide() => Hide(ContentDialogResult.None);

    /// <summary>
    /// Closes the current <see cref="ContentDialog"/> with the given <see cref="ContentDialogResult"/> <para>ddd</para>
    /// </summary>
    /// <param name="dialogResult">The <see cref="ContentDialogResult"/> to return</param>
    public void Hide(ContentDialogResult dialogResult)
    {
        _result = dialogResult;
        HideCore();
    }

    /// <summary>
    /// Called when the primary button is invoked
    /// </summary>
    protected virtual void OnPrimaryButtonClick(ContentDialogButtonClickEventArgs args)
    {
        PrimaryButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the secondary button is invoked
    /// </summary>
    protected virtual void OnSecondaryButtonClick(ContentDialogButtonClickEventArgs args)
    {
        SecondaryButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the close button is invoked
    /// </summary>
    protected virtual void OnCloseButtonClick(ContentDialogButtonClickEventArgs args)
    {
        CloseButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the ContentDialog is requested to be opened
    /// </summary>
    protected virtual void OnOpening()
    {
        Opening?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called after the ContentDialog is initialized but just before its presented on screen
    /// </summary>
    protected virtual void OnOpened()
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the ContentDialog has been requested to close, but before it actually closes
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnClosing(ContentDialogClosingEventArgs args)
    {
        Closing?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the ContentDialog has been closed and removed from the tree
    /// </summary>
    protected virtual void OnClosed(ContentDialogClosedEventArgs args)
    {
        Closed?.Invoke(this, args);
    }

    private void HideCore()
    {
        // v2 - No longer disabling the dialog during a deferral so we need to make sure that if
        //      multiple requests to close come in, we don't handle them
        if (_hasDeferralActive)
            return;

        // v2- Changed to match logic in TeachingTip for deferral, fixing #239 where cancel
        //     was being handled before the deferral.
        var args = new ContentDialogClosingEventArgs(_result);

        var deferral = new Deferral(() =>
        {
            Dispatcher.UIThread.VerifyAccess();
            _hasDeferralActive = false;

            if (!args.Cancel)
            {
                FinalCloseDialog();
            }
        });

        args.SetDeferral(deferral);
        _hasDeferralActive = true;

        args.IncrementDeferralCount();
        OnClosing(args);
        args.DecrementDeferralCount();
    }

    // Internal only for UnitTests
    internal void SetupDialog()
    {
        if (_primaryButton == null)
            throw new InvalidOperationException("Attempted to setup ContentDialog but the template has not been applied yet.");

        PseudoClasses.Set(s_pcPrimary, !string.IsNullOrEmpty(PrimaryButtonText));
        PseudoClasses.Set(s_pcSecondary, !string.IsNullOrEmpty(SecondaryButtonText));
        PseudoClasses.Set(s_pcClose, !string.IsNullOrEmpty(CloseButtonText));

        var p = Presenter;
        switch (DefaultButton)
        {
            case ContentDialogButton.Primary:
                if (!_primaryButton.IsVisible)
                {
#if DEBUG
                    Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog", 
                        "DefaultButton was set to Primary, but PrimaryButton is not enabled");
#endif
                    break;
                }

                _primaryButton.Classes.Add(SharedPseudoclasses.s_cAccent);
                _secondaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);
                _closeButton.Classes.Remove(SharedPseudoclasses.s_cAccent);

                _primaryButton.Focus();
#if DEBUG
                Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog", "Set initial focus to PrimaryButton");
#endif


                break;

            case ContentDialogButton.Secondary:
                if (!_secondaryButton.IsVisible)
                {
#if DEBUG
                    Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog",
                        "DefaultButton was set to Secondary, but SecondaryButton is not enabled");
#endif
                    break;
                }

                _secondaryButton.Classes.Add(SharedPseudoclasses.s_cAccent);
                _primaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);
                _closeButton.Classes.Remove(SharedPseudoclasses.s_cAccent);

                _secondaryButton.Focus();
#if DEBUG
                Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog", "Set initial focus to SecondaryButton");
#endif

                break;

            case ContentDialogButton.Close:
                if (!_closeButton.IsVisible)
                {
#if DEBUG
                    Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog",
                        "DefaultButton was set to Close, but CloseButton is not enabled");
#endif
                    break;
                }

                _closeButton.Classes.Add(SharedPseudoclasses.s_cAccent);
                _primaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);
                _secondaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);

                _closeButton.Focus();
#if DEBUG
                Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog", "Set initial focus to CloseButton");
#endif

                break;

            default:
                _closeButton.Classes.Remove(SharedPseudoclasses.s_cAccent);
                _primaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);
                _secondaryButton.Classes.Remove(SharedPseudoclasses.s_cAccent);

                // If no default button is set, try to find a suitable first focus item. If none exist, focus the
                // ContentDialog itself to pull focus away from the main visual tree so weird things don't happen
                // The latter shouldn't happen in 99% of cases as either something in the user content will be able
                // to take focus OR there should always be at least one button which can take focus
                var next = KeyboardNavigationHandler.GetNext(this, NavigationDirection.Next) ?? this;
                next.Focus();

#if DEBUG
                Logger.TryGet(LogEventLevel.Debug, "ContentDialog")?.Log("SetupDialog", "Set initial focus to {next}", next);
#endif
                break;
        }
    }

    // This is the exit point for the ContentDialog
    // This method MUST be called to finalize everything
    private async void FinalCloseDialog()
    {
        // Prevent interaction when closing...double/mutliple clicking on the buttons to close
        // the dialog was calling this multiple times, which would cause the OverlayLayer check
        // below to fail (as this would be removed from the tree). This is a simple workaround
        // to make sure we don't error out
        IsHitTestVisible = false;

        // For a better experience when animating closed, we need to make sure the
        // focus adorner is not showing (if using keyboard) otherwise that will hang
        // around and not fade out and it just looks weird. So focus this to force the
        // adorner to hide, then continue forward.
        Focus();

        PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, true);
        PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, false);

        // Let the close animation finish (now 0.167s in new WinUI update...)
        // We'll wait just a touch longer to be sure
        await Task.Delay(200);

        OnClosed(new ContentDialogClosedEventArgs(_result));

        if (_lastFocus != null)
        {
            _lastFocus.Focus(NavigationMethod.Unspecified);
            _lastFocus = null;
        }

        var ol = OverlayLayer.GetOverlayLayer(_host);
        // If OverlayLayer isn't found here, this may be a reentrant call (hit ESC multiple times quickly, etc)
        // Don't fail, and return. If this isn't reentrant, there's bigger issues...
        if (ol == null)
            return;

        ol.Children.Remove(_host);

        _host.Content = null;

        if (_originalHost != null)
        {
            if (_originalHost is Panel p)
            {
                p.Children.Insert(_originalHostIndex, this);
            }
            else if (_originalHost is Decorator d)
            {
                d.Child = this;
            }
            else if (_originalHost is ContentControl cc)
            {
                cc.Content = this;
            }
            else if (_originalHost is ContentPresenter cp)
            {
                cp.Content = this;
            }
        }

        _hotkeyDownVisual = null;

        IsHitTestVisible = true;
        IsVisible = false;
        
        _tcs.TrySetResult(_result);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        // v2 - No longer disabling the dialog during a deferral so we need to make sure that if
        //      multiple requests to close come in, we don't handle them
        if (_hasDeferralActive)
            return;

        var args = new ContentDialogButtonClickEventArgs();

        var deferral = new Deferral(() =>
        {
            Dispatcher.UIThread.VerifyAccess();
            _hasDeferralActive = false;

            if (args.Cancel)
                return;

            if (sender == _primaryButton)
            {
                if (PrimaryButtonCommand != null && PrimaryButtonCommand.CanExecute(PrimaryButtonCommandParameter))
                {
                    PrimaryButtonCommand.Execute(PrimaryButtonCommandParameter);
                }
                _result = ContentDialogResult.Primary;
            }
            else if (sender == _secondaryButton)
            {
                if (SecondaryButtonCommand != null && SecondaryButtonCommand.CanExecute(SecondaryButtonCommandParameter))
                {
                    SecondaryButtonCommand.Execute(SecondaryButtonCommandParameter);
                }
                _result = ContentDialogResult.Secondary;
            }
            else if (sender == _closeButton)
            {
                if (CloseButtonCommand != null && CloseButtonCommand.CanExecute(CloseButtonCommandParameter))
                {
                    CloseButtonCommand.Execute(CloseButtonCommandParameter);
                }
                _result = ContentDialogResult.None;
            }

            HideCore();
        });

        args.SetDeferral(deferral);
        _hasDeferralActive = true;

        args.IncrementDeferralCount();
        if (sender == _primaryButton)
        {
            OnPrimaryButtonClick(args);
        }
        else if (sender == _secondaryButton)
        {
            OnSecondaryButtonClick(args);
        }
        else if (sender == _closeButton)
        {
            OnCloseButtonClick(args);
        }
        args.DecrementDeferralCount();
    }

    private void OnFullSizedDesiredChanged(AvaloniaPropertyChangedEventArgs e)
    {
        bool newVal = (bool)e.NewValue;
        PseudoClasses.Set(s_pcFullSize, newVal);
    }

    public (bool handled, IInputElement next) GetNext(IInputElement element, NavigationDirection direction)
    {
        var children = this.GetVisualDescendants().OfType<IInputElement>()
            .Where(x => KeyboardNavigation.GetIsTabStop((InputElement)x) && x.Focusable &&
            x.IsEffectivelyVisible && IsEffectivelyEnabled).ToList();

        if (children.Count == 0)
            return (false, null);

        var current = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement();
        if (current == null)
            return (false, null);

        if (direction == NavigationDirection.Next)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == current)
                {
                    if (i == children.Count - 1)
                    {
                        return (true, children[0]);
                    }
                    else
                    {
                        return (true, children[i + 1]);
                    }
                }
            }
        }
        else if (direction == NavigationDirection.Previous)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] == current)
                {
                    if (i == 0)
                    {
                        return (true, children[children.Count - 1]);
                    }
                    else
                    {
                        return (true, children[i - 1]);
                    }
                }
            }
        }

        return (false, null);
    }

    private void DialogLoaded(object sender, RoutedEventArgs args)
    {
        Loaded -= DialogLoaded;

        // Run setup, this will set up the buttons and attempt to set initial focus into the dialog
        SetupDialog();

        // Now force a new layout pass so everything in SetupDialog takes effect
        UpdateLayout();

        // Now that we've fully initialized here, raise the Opened event
        OnOpened();
    }

    // Store the last element focused before showing the dialog, so we can
    // restore it when it closes
    private IInputElement _lastFocus;
    private Control _originalHost;
    private int _originalHostIndex;
    private DialogHost _host;
    private ContentDialogResult _result;
    private TaskCompletionSource<ContentDialogResult> _tcs;
    private Button _primaryButton;
    private Button _secondaryButton;
    private Button _closeButton;
    private bool _hasDeferralActive;
    private Visual _hotkeyDownVisual;
}
