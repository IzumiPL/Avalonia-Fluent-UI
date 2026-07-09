using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
///     A button that displays a color swatch and opens a
///     <see cref="ColorDialog" /> when clicked.
/// </summary>
public class ColorPickerButton : PickerButton 
{
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorPickerButton, Color>(nameof(Color), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ColorPickerButton, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ColorPickerButton, object?>(
            nameof(CommandParameter));

    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    private readonly ColorDialog _colorDialog = new();
    
    public event EventHandler<Color>? ColorChanged;

    static ColorPickerButton()
    {
        ColorProperty.Changed.AddClassHandler<ColorPickerButton>((s, _) =>
        {
            s.Background = new SolidColorBrush(s.Color);
        });
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!IsEnabled)
            return;

        if (e.Key is Key.Space or Key.Enter)
        {
            OnClick();
            e.Handled = true;
        }
    }

    protected override void OnClick()
    {
        base.OnClick();
        
        ShowColorDialogAsync();
    } 

    private void ExecuteCommand()
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
    
    private async void ShowColorDialogAsync()
    {
        var dialog = new ColorDialog { Color = this.Color };
        var toplevel = TopLevel.GetTopLevel(this);
        
        var result = toplevel is PopupRoot ? await dialog.ShowAsync() : await dialog.ShowAsync(toplevel);
        if (result == ContentDialogResult.Primary)
        {
            Color = dialog.Color;
            ColorChanged?.Invoke(this, dialog.Color);
            ExecuteCommand();
        }
    }
}
