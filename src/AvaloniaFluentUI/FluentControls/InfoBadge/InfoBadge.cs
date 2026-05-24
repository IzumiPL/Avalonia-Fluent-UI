using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a control for indicating notifications, alerts, new content, 
/// or to attract focus to an area within an app.
/// </summary>
[PseudoClasses(s_pcValue, s_pcFontIcon, SharedPseudoclasses.s_pcIcon, s_pcDot)]
public partial class InfoBadge : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Value"/> property
    /// </summary>
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<InfoBadge, int>(nameof(Value), -1);

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        SettingsExpander.IconSourceProperty.AddOwner<InfoBadge>();

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBadgeTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<InfoBadge, InfoBadgeTemplateSettings>(nameof(TemplateSettings));

    /// <summary>
    /// Gets or sets the integer to be displayed in a numeric InfoBadge.
    /// </summary>
    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon to be used in an InfoBadge.
    /// </summary>
    public IconSource IconSource
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

    private const string s_pcValue = ":value";
    private const string s_pcFontIcon = ":fonticon";
    private const string s_pcDot = ":dot";
    
    public InfoBadge()
    {
        TemplateSettings = new InfoBadgeTemplateSettings();
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

        if (change.Property == ValueProperty)
        {
            if (Value < -1)
                throw new ArgumentOutOfRangeException(nameof(Value));
        }
        else if (change.Property == ValueProperty || change.Property == IconSourceProperty)
        {
            OnDisplayKindPropertiesChanged();
        }
        else if (change.Property == BoundsProperty)
        {
            OnBoundsChanged(change);
        }
    }

    private void OnDisplayKindPropertiesChanged()
    {
        var icoSource = IconSource;
        if (Value >= 0)
        {
            PseudoClasses.Set(s_pcValue, true);

            PseudoClasses.Set(s_pcFontIcon, false);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, false);
            PseudoClasses.Set(s_pcDot, false);
        }
        else if (icoSource != null)
        {
            TemplateSettings.IconElement = IconHelpers.CreateFromUnknown(icoSource);

            PseudoClasses.Set(s_pcFontIcon, icoSource is FontIconSource);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, icoSource is not FontIconSource);

            PseudoClasses.Set(s_pcValue, false);
            PseudoClasses.Set(s_pcDot, false);
        }
        else
        {
            PseudoClasses.Set(s_pcDot, true);

            PseudoClasses.Set(s_pcValue, false);
            PseudoClasses.Set(s_pcFontIcon, false);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, false);
        }
    }

    private void OnBoundsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var rc = (Rect)e.NewValue;
        var cornerRadiusValue = rc.Height / 2;
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
