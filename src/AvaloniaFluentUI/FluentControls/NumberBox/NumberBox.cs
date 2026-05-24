using System;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a control that can be used to display and edit numbers.
/// </summary>
[PseudoClasses(s_pcSpinVisible, s_pcSpinPopup, s_pcSpinCollapsed)]
[PseudoClasses(s_pcUpDisabled, s_pcDownDisabled)]
[PseudoClasses(SharedPseudoclasses.s_pcHeader)]
[TemplatePart(s_tpDownSpinButton, typeof(RepeatButton))]
[TemplatePart(s_tpPopupDownSpinButton, typeof(RepeatButton))]
[TemplatePart(s_tpUpSpinButton, typeof(RepeatButton))]
[TemplatePart(s_tpPopupUpSpinButton, typeof(RepeatButton))]
[TemplatePart(s_tpInputBox, typeof(TextBox))]
[TemplatePart(s_tpUpDownPopup, typeof(Popup))]
public partial class NumberBox : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="AcceptsExpression"/> property
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsExpressionProperty =
        AvaloniaProperty.Register<NumberBox, bool>(nameof(AcceptsExpression));

    /// <summary>
    /// Defines the <see cref="Description"/> property
    /// </summary>
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<NumberBox, string>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Header"/> property
    /// </summary>
    public static readonly StyledProperty<object> HeaderProperty =
        HeaderedContentControl.HeaderProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="HeaderTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> HeaderTemplateProperty =
        HeaderedContentControl.HeaderTemplateProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="IsWrapEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsWrapEnabledProperty =
        AvaloniaProperty.Register<NumberBox, bool>(nameof(IsWrapEnabled));

    /// <summary>
    /// Defines the <see cref="LargeChange"/> property
    /// </summary>
    public static readonly StyledProperty<double> LargeChangeProperty =
        RangeBase.LargeChangeProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="Minimum"/> property
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty =
        RangeBase.MinimumProperty.AddOwner<NumberBox>(
            new StyledPropertyMetadata<double>(
                defaultValue: double.MinValue,
                coerce: (ao, d1) =>
                {
                    var nb = ao as NumberBox;
                    var max = nb.Maximum;
                    if (d1 > max)
                        d1 = max;
                    nb.CoerceValueIfNeeded(d1, max);
                    return d1;
                }));

    /// <summary>
    /// Defines the <see cref="Maximum"/> property
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        RangeBase.MaximumProperty.AddOwner<NumberBox>(
            new StyledPropertyMetadata<double>(
                defaultValue: double.MaxValue,
                coerce: (ao, d1) =>
                {
                    var nb = ao as NumberBox;
                    var min = nb.Minimum;
                    if (d1 < min)
                        d1 = min;
                    nb.CoerceValueIfNeeded(min, d1);
                    return d1;
                }));

    //Skip NumberFormatter

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> property
    /// </summary>
    public static readonly StyledProperty<string> PlaceholderTextProperty =
        TextBox.WatermarkProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="SelectionHighlightColor"/> property
    /// </summary>
    public static readonly StyledProperty<IBrush> SelectionHighlightColorProperty =
        TextBox.SelectionBrushProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="SmallChange"/> property
    /// </summary>
    public static readonly StyledProperty<double> SmallChangeProperty =
        RangeBase.SmallChangeProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="SpinButtonPlacementMode"/> property
    /// </summary>
    public static readonly StyledProperty<NumberBoxSpinButtonPlacementMode> SpinButtonPlacementModeProperty =
        AvaloniaProperty.Register<NumberBox, NumberBoxSpinButtonPlacementMode>(nameof(SpinButtonPlacementMode),
            NumberBoxSpinButtonPlacementMode.Hidden);

    /// <summary>
    /// Defines the <see cref="Text"/> property
    /// </summary>
    public static readonly DirectProperty<NumberBox, string> TextProperty =
        AvaloniaProperty.RegisterDirect<NumberBox, string>(nameof(Text),
            x => x.Text, (x, v) => x.Text = v, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="NumberBoxValidationMode"/> property
    /// </summary>
    public static readonly StyledProperty<NumberBoxValidationMode> ValidationModeProperty =
       AvaloniaProperty.Register<NumberBox, NumberBoxValidationMode>(nameof(ValidationMode));

    /// <summary>
    /// Defines the <see cref="Value"/> property
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
         RangeBase.ValueProperty.AddOwner<NumberBox>(
             new StyledPropertyMetadata<double>(
                 enableDataValidation: true,
                 coerce: (ao, d1) =>
                 {
                     var nb = ao as NumberBox;
                     var ret = nb.CoerceValueToRange(d1);

                     // If we had to coerce and the coerced value is the same
                     // as the current value, the text won't get updated and will
                     // remain the invalid value, force set, see GH#670
                     if (ret == nb.Value)
                     {
                         nb.UpdateTextToValue();
                     }
                     return ret;
                 }));

    //Skip InputScope

    /// <summary>
    /// Defines the <see cref="TextAlignment"/> property
    /// </summary>
    public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
        TextBlock.TextAlignmentProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Defines the <see cref="SimpleNumberFormat"/> property
    /// </summary>
    public static readonly StyledProperty<string> SimpleNumberFormatProperty =
        AvaloniaProperty.Register<NumberBox, string>(nameof(SimpleNumberFormat));

    /// <summary>
    /// Defines the <see cref="InnerLeftContent"/> property
    /// </summary>
    public static readonly StyledProperty<object> InnerLeftContentProperty =
        TextBox.InnerLeftContentProperty.AddOwner<NumberBox>();

    /// <summary>
    /// Toggles whether the control will accept and evaluate a basic formulaic expression entered as input.
    /// </summary>
    public bool AcceptsExpression
    {
        get => GetValue(AcceptsExpressionProperty);
        set => SetValue(AcceptsExpressionProperty, value);
    }

    /// <summary>
    /// Gets or sets content that is shown below the control. The content should provide guidance 
    /// about the input expected by the control.
    /// </summary>
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the control's header.
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display the content of the control's header.
    /// </summary>
    public IDataTemplate HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Toggles whether line breaking occurs if a line of text extends beyond the available 
    /// width of the control.
    /// </summary>
    public bool IsWrapEnabled
    {
        get => GetValue(IsWrapEnabledProperty);
        set => SetValue(IsWrapEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the value that is added to or subtracted from Value when a large change is made,
    /// such as with the PageUP and PageDown keys.
    /// </summary>
    public double LargeChange
    {
        get => GetValue(LargeChangeProperty);
        set => SetValue(LargeChangeProperty, value);
    }

    /// <summary>
    /// Gets or sets the numerical minimum for Value.
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the numerical maximum for Value.
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// A function for customizing the format of the NumberBox Value Text.
    /// </summary>
    /// <remarks>
    /// .NET doesn't have all of the formatting stuff from WinUI/WinRT, thus doing fancy things
    /// requires a bit more manual work, and I'm not about to attempt to replicate the NumberFormatters :D
    /// NOTE: This cannot be used if <see cref="SimpleNumberFormat"/> is in use
    /// </remarks>
    public Func<double, string> NumberFormatter { get; set; }

    /// <summary>
    /// Use this for simple number formatting using normal .net formatting. Resulting string must still
    /// be numeric in value, no special characters, as they are not removed when attempting to convert
    /// text to value
    /// </summary>
    /// <remarks>
    /// This property cannot be used if <see cref="NumberFormatter"/> is also in use
    /// </remarks>
    public string SimpleNumberFormat
    {
        get => GetValue(SimpleNumberFormatProperty);
        set => SetValue(SimpleNumberFormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the text that is displayed in the control until the value is changed by a 
    /// user action or some other operation.
    /// </summary>
    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to highlight the selected text.
    /// </summary>
    public IBrush SelectionHighlightColor
    {
        get => GetValue(SelectionHighlightColorProperty);
        set => SetValue(SelectionHighlightColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the value that is added to or subtracted from Value when a small 
    /// change is made, such as with an arrow key or scrolling.
    /// </summary>
    public double SmallChange
    {
        get => GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the placement of buttons used to increment 
    /// or decrement the Value property.
    /// </summary>
    public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
    {
        get => GetValue(SpinButtonPlacementModeProperty);
        set => SetValue(SpinButtonPlacementModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the string type representation of the Value property.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            if (!_textUpdating && SetAndRaise(TextProperty, ref _text, value))
            {
                UpdateValueToText();
            }
        }
    }

    /// <summary>
    /// Gets or sets the input validation behavior to invoke when invalid input is entered.
    /// </summary>
    public NumberBoxValidationMode ValidationMode
    {
        get => GetValue(ValidationModeProperty);
        set => SetValue(ValidationModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the numeric value of a NumberBox.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the TextAlignment of the text in the NumberBox
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner left content of the TextBox within the NumberBox
    /// </summary>
    public object InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    /// <summary>
    /// Occurs after the user triggers evaluation of new input by pressing the Enter key, 
    /// clicking a spin button, or by changing focus.
    /// </summary>
    public event TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs> ValueChanged;

    public string _text = null;

    private const string s_tpDownSpinButton = "DownSpinButton";
    private const string s_tpPopupDownSpinButton = "PopupDownSpinButton";
    private const string s_tpUpSpinButton = "UpSpinButton";
    private const string s_tpPopupUpSpinButton = "PopupUpSpinButton";
    private const string s_tpInputBox = "InputBox";
    private const string s_tpUpDownPopup = "UpDownPopup";

    private const string s_pcSpinVisible = ":spinvisible";
    private const string s_pcSpinPopup = ":spinpopup";
    private const string s_pcSpinCollapsed = ":spincollapsed";
    private const string s_pcUpDisabled = ":updisabled";
    private const string s_pcDownDisabled = ":downdisabled";
    
    public NumberBox()
    {
        AddHandler(PointerPressedEvent, OnPointerPressedPreview, RoutingStrategies.Tunnel);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _spinDown = e.NameScope.Find<RepeatButton>(s_tpDownSpinButton);
        _popupDownButton = e.NameScope.Find<RepeatButton>(s_tpPopupDownSpinButton);
        if (_spinDown != null)
        {
            _spinDown.Click += OnSpinDownClick;
        }
        if (_popupDownButton != null)
        {
            _popupDownButton.Click += OnSpinDownClick;
        }

        _spinUp = e.NameScope.Find<RepeatButton>(s_tpUpSpinButton);
        _popupUpButton = e.NameScope.Find<RepeatButton>(s_tpPopupUpSpinButton);
        if (_spinUp != null)
        {
            _spinUp.Click += OnSpinUpClick;
        }
        if (_popupUpButton != null)
        {
            _popupUpButton.Click += OnSpinUpClick;
        }

        _textBox = e.NameScope.Find<TextBox>(s_tpInputBox);
        if (_textBox != null)
        {
            _textBox.AddHandler(KeyDownEvent, OnNumberBoxKeyDown, RoutingStrategies.Tunnel);

            _textBox.KeyUp += OnNumberBoxKeyUp;
        }

        _popup = e.NameScope.Find<Popup>(s_tpUpDownPopup);
        if (_popup != null)
        {
            _popup.OverlayInputPassThroughElement = this;
        }

        UpdateSpinButtonPlacement();
        UpdateSpinButtonEnabled();

        //UpdateVisualStateForIsEnabledChange();

        if (double.IsNaN(Value) &&
            !string.IsNullOrEmpty(_text))
        {
            // If Text has been set, but Value hasn't, update Value based on Text.
            UpdateValueToText();
        }
        else
        {
            UpdateTextToValue();
        }
    }

    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception error)
    {
        base.UpdateDataValidation(property, state, error);

        if (property == ValueProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            OnValueChanged(change.GetOldValue<double>(), change.GetNewValue<double>());
        }
        if (change.Property == IsWrapEnabledProperty)
        {
            UpdateSpinButtonEnabled();
        }
        else if (change.Property == SpinButtonPlacementModeProperty)
        {
            UpdateSpinButtonPlacement();
        }
        else if (change.Property == HeaderProperty || change.Property == HeaderTemplateProperty)
        {
            UpdateHeaderPresenterState();
        }
        else if (change.Property == LargeChangeProperty || change.Property == SmallChangeProperty)
        {
            UpdateSpinButtonEnabled();
        }
        else if (change.Property == ValidationModeProperty)
        {
            ValidateInput();
            UpdateSpinButtonEnabled();
        }
        else if (change.Property == SimpleNumberFormatProperty)
        {
            if (NumberFormatter != null)
                throw new InvalidOperationException("NumberFormatter must be null");

            UpdateTextToValue();
        }
        else if (change.Property == MinimumProperty || change.Property == MaximumProperty)
        {
            UpdateSpinButtonEnabled();
        }
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);

        if (_textBox != null)
        {
            _textBox.SelectAll();
        }

        if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Compact)
        {
            if (_popup != null)
            {
                _popup.IsOpen = true;
            }
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        ValidateInput();

        if (_popup != null)
        {
            _popup.IsOpen = false;
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (_textBox != null && IsKeyboardFocusWithin)
        {
            var delta = e.Delta.Y;
            if (delta > 0)
            {
                StepValue(SmallChange);
            }
            else
            {
                StepValue(-SmallChange);
            }
            e.Handled = true;
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NumberBoxAutomationPeer(this);
    }

    private void OnPointerPressedPreview(object sender, PointerPressedEventArgs args)
    {
        // Hack: B/c we make popup lightdismissable, we need to ensure we can reopen the popup if focus
        // never leaves the control, but we click back on it
        // Do this in Preview b/c TextBox will handle pointer event
        if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Compact &&
            _popup != null && !_popup.IsOpen && IsKeyboardFocusWithin)
        {
            _popup.IsOpen = true;
        }
    }

    private void OnValueChanged(double oldValue, double newValue)
    {
        // This handler may change Value; don't send extra events in that case.
        if (!_valueUpdating)
        {
            try
            {
                _valueUpdating = true;

                if (newValue != oldValue && !(double.IsNaN(newValue) && double.IsNaN(oldValue)))
                {
                    // Fire ValueChanged event
                    var ea = new NumberBoxValueChangedEventArgs(oldValue, newValue);

                    ValueChanged?.Invoke(this, ea);

                    var peer = ControlAutomationPeer.FromElement(this) as NumberBoxAutomationPeer;
                    peer?.RaiseValueChangedEvent(oldValue, newValue);
                }

                UpdateTextToValue();
                UpdateSpinButtonEnabled();

            }
            finally
            {
                _valueUpdating = false;
            }
        }
    }

    private void UpdateValueToText()
    {
        if (_textBox != null)
        {
            _textBox.Text = Text;
            ValidateInput();
        }
    }

    private void ValidateInput()
    {
        // Validate the content of the inner textbox
        if (_textBox == null)
            return;

        var text = _textBox.Text?.Trim();

        // Handles empty TextBox case, set text ot current value
        if (string.IsNullOrEmpty(text))
        {
            Value = double.NaN;
        }
        else
        {
            var value = AcceptsExpression ? NumberBoxParser.Compute(text) :
                ParseDouble(text);

            if (value == null)
            {
                if (ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
                {
                    // Override text to current value
                    UpdateTextToValue();
                }
            }
            else
            {
                if (value.Value == Value)
                {
                    // Even if the value hasn't changed, we still want to update the text (e.g. Value is 3, user types 1 + 2, we want to replace the text with 3)
                    UpdateTextToValue();
                }
                else
                {
                    Value = value.Value;
                }
            }
        }
    }

    //Replaces INumberParser in winrt
    private double? ParseDouble(string txt)
    {
        if (double.TryParse(txt, NumberStyles.Any, CultureInfo.CurrentCulture, out double result))
        {
            return result;
        }

        return null;
    }

    private void OnSpinDownClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StepValue(-SmallChange);
    }

    private void OnSpinUpClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StepValue(SmallChange);
    }

    private void OnNumberBoxKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                StepValue(SmallChange);
                e.Handled = true;
                break;

            case Key.Down:
                StepValue(-SmallChange);
                e.Handled = true;
                break;

            case Key.PageUp:
                StepValue(LargeChange);
                e.Handled = true;
                break;

            case Key.PageDown:
                StepValue(-LargeChange);
                e.Handled = true;
                break;
        }
    }

    private void OnNumberBoxKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                ValidateInput();
                e.Handled = true;
                break;

            case Key.Escape:
                UpdateTextToValue();
                e.Handled = true;
                break;
        }
    }

    public void StepValue(double change)
    {
        // Before adjusting the value, validate the contents of the textbox so we don't override it.
        ValidateInput();

        var newVal = Value;
        var max = Maximum;
        var min = Minimum;
        if (!double.IsNaN(newVal))
        {
            newVal += change;

            if (IsWrapEnabled)
            {
                if (newVal > max)
                    newVal = min;
                else if (newVal < min)
                    newVal = max;
            }

            Value = newVal;

            // We don't want the caret to move to the front of the text for example when using the up/down arrows
            // to change the numberbox value.
            MoveCaretToTextEnd();
        }
    }

    // Updates TextBox.Text with the formatted Value
    private void UpdateTextToValue()
    {
        if (_textBox == null)
            return;

        string newText = "";
        var value = Value;

        if (!double.IsNaN(value))
        {
            // Round to 12 digits (standard .net rounding per WinUI in the NumberBox source)
            // We do this to prevent weirdness from floating point imprecision
            var newValue = Math.Round(value, 12);
            if (SimpleNumberFormat != null)
            {
                newText = newValue.ToString(SimpleNumberFormat);
            }
            else if (NumberFormatter != null)
            {
                newText = NumberFormatter(newValue);
            }
            else
            {
                newText = newValue.ToString();
            }
        }

        _textBox.Text = newText;

        try
        {
            _textUpdating = true;
            Text = newText;
        }
        finally
        {
            _textUpdating = false;

            // GH 389: only move caret if we're focused, otherwise it triggers a bring into view event
            if (IsKeyboardFocusWithin)
                MoveCaretToTextEnd(); // Add this
        }
    }

    private void UpdateSpinButtonPlacement()
    {
        // v2: 
        // Control themes don't support nested /template/ which was previously used here
        // So we set :spinvisible and :spinpopup on the TextBox too since the TextBox
        // is already a custom style for the NumberBox. Will keep the pseudoclasses on
        // the NumberBox too for external styles that may want this info.
        // :spincollapsed will not be set on TextBox
        var sbm = SpinButtonPlacementMode;

        if (sbm == NumberBoxSpinButtonPlacementMode.Inline)
        {
            PseudoClasses.Set(s_pcSpinVisible, true);
            PseudoClasses.Set(s_pcSpinPopup, false);
            PseudoClasses.Set(s_pcSpinCollapsed, false);

            if (_textBox != null)
            {
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinVisible, true);
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinPopup, false);
            }
        }
        else if (sbm == NumberBoxSpinButtonPlacementMode.Compact)
        {
            PseudoClasses.Set(s_pcSpinVisible, false);
            PseudoClasses.Set(s_pcSpinPopup, true);
            PseudoClasses.Set(s_pcSpinCollapsed, false);

            if (_textBox != null)
            {
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinVisible, false);
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinPopup, true);
            }
        }
        else
        {
            PseudoClasses.Set(s_pcSpinVisible, false);
            PseudoClasses.Set(s_pcSpinPopup, false);
            PseudoClasses.Set(s_pcSpinCollapsed, true);

            if (_textBox != null)
            {
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinVisible, false);
                ((IPseudoClasses)_textBox.Classes).Set(s_pcSpinPopup, false);
            }
        }
    }

    private void UpdateSpinButtonEnabled()
    {
        bool isUpEnabled = false;
        bool isDownEnabled = false;

        var value = Value;
        if (!double.IsNaN(value))
        {
            if (IsWrapEnabled || ValidationMode != NumberBoxValidationMode.InvalidInputOverwritten)
            {
                // If wrapping is enabled, or invalid values are allowed, then the buttons should be enabled
                isUpEnabled = true;
                isDownEnabled = true;
            }
            else
            {
                if (value < Maximum)
                    isUpEnabled = true;

                if (value > Minimum)
                    isDownEnabled = true;
            }
        }

        PseudoClasses.Set(s_pcUpDisabled, !isUpEnabled);
        PseudoClasses.Set(s_pcDownDisabled, !isDownEnabled);
    }

    private void UpdateHeaderPresenterState()
    {
        bool showHeader = false;

        if (Header != null)
        {
            if (Header is string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    showHeader = true;
                }
            }
            else
            {
                showHeader = true;
            }
        }

        if (HeaderTemplate != null)
        {
            showHeader = true;
        }

        //Changed to Pseudoclass rather than keeping a ref to the ContentPresenter
        PseudoClasses.Set(SharedPseudoclasses.s_pcHeader, showHeader);
    }

    private void MoveCaretToTextEnd()
    {
        if (_textBox != null)
        {
            _textBox.SelectionStart = _textBox.SelectionEnd = _textBox.CaretIndex = _textBox.Text.Length;
        }
    }

    private void CoerceValueIfNeeded(double min, double max)
    {
        var value = Value;
        if (double.IsNaN(value))
            return;

        if (value < min)
            Value = min;
        else if (value > max)
            Value = max;
    }

    private double CoerceValueToRange(double val)
    {
        var maximum = Maximum;
        var minimum = Minimum;
        if (!double.IsNaN(val) && (val > maximum || val < minimum) && ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
        {
            if (val > maximum)
                return maximum;

            if (val < minimum)
                return minimum;
        }

        return val;
    }

    //Template parts
    private RepeatButton _spinDown;
    private RepeatButton _spinUp;
    private TextBox _textBox;
    private Popup _popup;
    private RepeatButton _popupUpButton;
    private RepeatButton _popupDownButton;

    private bool _textUpdating;
    private bool _valueUpdating;
}
