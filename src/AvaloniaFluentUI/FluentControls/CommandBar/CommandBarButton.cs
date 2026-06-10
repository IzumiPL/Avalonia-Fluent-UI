using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Input;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a templated button control to be displayed in an <see cref="CommandBar"/>.
/// </summary>

[PseudoClasses(SharedPseudoclasses.s_pcIcon, SharedPseudoclasses.s_pcLabel, SharedPseudoclasses.s_pcCompact)]
[PseudoClasses(SharedPseudoclasses.s_pcFlyout, s_pcSubmenuOpen, SharedPseudoclasses.s_pcOverflow)]
[PseudoClasses(SharedPseudoclasses.s_pcHotkey)]
public partial class CommandBarButton : Button, ICommandBarElement
{
    /// <summary>
    /// Defines the <see cref="IsInOverflow"/> property
    /// </summary>
    public static readonly DirectProperty<CommandBarButton, bool> IsInOverflowProperty =
        AvaloniaProperty.RegisterDirect<CommandBarButton, bool>(nameof(IsInOverflow),
            x => x.IsInOverflow);

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, IconSource>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="Label"/> property
    /// </summary>
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<CommandBarButton, string>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="DynamicOverflowOrder"/> property
    /// </summary>
    public static readonly DirectProperty<CommandBarButton, int> DynamicOverflowOrderProperty =
        AvaloniaProperty.RegisterDirect<CommandBarButton, int>(nameof(DynamicOverflowOrder),
            x => x.DynamicOverflowOrder, (x, v) => x.DynamicOverflowOrder = v);

    /// <summary>
    /// Defines the <see cref="IsCompact"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CommandBarButton, bool>(nameof(IsCompact));

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<CommandBarButtonTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<CommandBarButton, CommandBarButtonTemplateSettings>(nameof(TemplateSettings));

    /// <summary>
    /// Gets or sets a value that indicates whether the button is shown with no label and reduced padding.
    /// </summary>
    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    /// <summary>
    /// Gets a value that indicates whether this item is in the overflow menu.
    /// </summary>
    public bool IsInOverflow
    {
        get => _isInOverflow;
        internal set
        {
            if (SetAndRaise(IsInOverflowProperty, ref _isInOverflow, value))
            {
                PseudoClasses.Set(SharedPseudoclasses.s_pcOverflow, value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the graphic content of the app bar toggle button.
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the text description displayed on the app bar toggle button.
    /// </summary>
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <inheritdoc/>
    public int DynamicOverflowOrder
    {
        get => _dynamicOverflowOrder;
        set => SetAndRaise(DynamicOverflowOrderProperty, ref _dynamicOverflowOrder, value);
    }

    /// <summary>
    /// Gets the template settings for this CommandBarButton
    /// </summary>
    public CommandBarButtonTemplateSettings TemplateSettings
    {
        get => GetValue(TemplateSettingsProperty);
        private set => SetValue(TemplateSettingsProperty, value);
    }

    private bool _isInOverflow;
    private int _dynamicOverflowOrder;

    private const string s_pcSubmenuOpen = ":submenuopen";
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBarButton"/> class.
    /// </summary>
    public CommandBarButton()
    {
        TemplateSettings = new CommandBarButtonTemplateSettings();
    }

    protected override Type StyleKeyOverride => typeof(CommandBarButton);

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconSourceProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
            TemplateSettings.Icon = IconHelpers.CreateFromUnknown(change.GetNewValue<IconSource>());
        }
        else if (change.Property == LabelProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcLabel, change.NewValue != null);
        }
        else if (change.Property == FlyoutProperty)
        {
            if (change.OldValue is FlyoutBase oldFB)
            {
                oldFB.Closed -= OnFlyoutClosed;
                oldFB.Opened -= OnFlyoutOpened;
            }

            if (change.NewValue is FlyoutBase newFB)
            {
                newFB.Closed += OnFlyoutClosed;
                newFB.Opened += OnFlyoutOpened;

                PseudoClasses.Set(SharedPseudoclasses.s_pcFlyout, true);
                PseudoClasses.Set(s_pcSubmenuOpen, newFB.IsOpen);
            }
            else
            {
                PseudoClasses.Set(SharedPseudoclasses.s_pcFlyout, false);
                PseudoClasses.Set(s_pcSubmenuOpen, false);
            }
        }
        else if (change.Property == HotKeyProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcHotkey, change.NewValue != null);
        }
        else if (change.Property == IsCompactProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcCompact, change.GetNewValue<bool>());
        }
        else if (change.Property == CommandProperty)
        {
            if (change.OldValue is XamlUICommand xamlComOld)
            {
                if (Label == xamlComOld.Label)
                {
                    Label = null;
                }

                if (HotKey == xamlComOld.HotKey)
                {
                    HotKey = null;
                }

                if (ToolTip.GetTip(this).ToString() == xamlComOld.Description)
                {
                    ToolTip.SetTip(this, null);
                }
            }

            if (change.NewValue is XamlUICommand xamlCom)
            {
                if (string.IsNullOrEmpty(Label))
                {
                    Label = xamlCom.Label;
                }

                IconSource = xamlCom.IconSource;

                if (HotKey == null)
                {
                    HotKey = xamlCom.HotKey;
                }

                if (ToolTip.GetTip(this) == null)
                {
                    ToolTip.SetTip(this, xamlCom.Description);
                }
            }
        }
    }

    protected override void OnClick()
    {
        base.OnClick();
        if (IsInOverflow)
        {
            var cb = this.FindLogicalAncestorOfType<CommandBar>();
            if (cb != null)
            {
                cb.IsOpen = false;
            }
        }
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    private void OnFlyoutOpened(object sender, EventArgs e)
    {
        PseudoClasses.Set(s_pcSubmenuOpen, true);
    }

    private void OnFlyoutClosed(object sender, EventArgs e)
    {
        PseudoClasses.Set(s_pcSubmenuOpen, false);
    }
}
