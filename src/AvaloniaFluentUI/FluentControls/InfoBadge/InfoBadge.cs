using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Helpers;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a control for indicating notifications, alerts, new content, 
/// or to attract focus to an area within an app.
/// </summary>
[PseudoClasses(PC_VALUE, PC_FONT_ICON, SharedPseudoclasses.s_pcIcon)]
public class InfoBadge : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Value"/> property
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<InfoBadge, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource?> IconSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, IconSource?>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBadgeTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<InfoBadge, InfoBadgeTemplateSettings>(nameof(TemplateSettings));

    /// <summary>
    /// Gets or sets the integer to be displayed in a numeric InfoBadge.
    /// </summary>
    public string? Value 
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon to be used in an InfoBadge.
    /// </summary>
    public IconSource? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Provides calculated values that can be referenced as TemplatedParent sources when defining 
    /// templates for an InfoBadge. Not intended for general use.
    /// </summary>
    public InfoBadgeTemplateSettings TemplateSettings
    {
        get => GetValue(TemplateSettingsProperty);
        internal set => SetValue(TemplateSettingsProperty, value);
    }

    private const string PC_VALUE = ":value";
    private const string PC_FONT_ICON = ":fonticon";
    
    public InfoBadge()
    {
        TemplateSettings = new InfoBadgeTemplateSettings();
        SizeChanged += HandleSizeChanged;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        OnDisplayKindPropertiesChanged();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var defaultDesSize = base.MeasureOverride(availableSize);

        if (defaultDesSize.Width < defaultDesSize.Height)
        {
            return new Size(defaultDesSize.Height, defaultDesSize.Height);
        }

        return defaultDesSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty || change.Property == IconSourceProperty)
        {
            OnDisplayKindPropertiesChanged();
        }
    }

    private void OnDisplayKindPropertiesChanged()
    {
        var icoSource = IconSource;
        if (!String.IsNullOrWhiteSpace(Value))
        {
            PseudoClasses.Set(PC_VALUE, true);
            PseudoClasses.Set(PC_FONT_ICON, false);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, false);
        }
        else if (icoSource != null)
        {
            TemplateSettings.IconElement = IconHelpers.CreateFromUnknown(icoSource);

            PseudoClasses.Set(PC_FONT_ICON, icoSource is FontIconSource);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, icoSource is not FontIconSource);

            PseudoClasses.Set(PC_VALUE, false);
        }
        else
        {
            PseudoClasses.Set(PC_VALUE, false);
            PseudoClasses.Set(PC_FONT_ICON, false);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, false);
        }
    }

    private void HandleSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        var cornerRadiusValue = args.NewSize.Height * 0.5;
        if (!IsSet(CornerRadiusProperty))
        {
            TemplateSettings.InfoBadgeCornerRadius = new CornerRadius(cornerRadiusValue);
        }
        else
        {
            TemplateSettings.InfoBadgeCornerRadius = new CornerRadius();
        }
    }
}
