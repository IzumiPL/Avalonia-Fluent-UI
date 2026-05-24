using System;
using Avalonia;
using System.Windows.Input;
using Avalonia.Input;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls.Input;

/// <summary>
/// Provides a base class for defining the command behavior of an interactive UI element that 
/// performs an action when invoked (such as sending an email, deleting an item, or submitting a form).
/// </summary>
public partial class XamlUICommand : AvaloniaObject, ICommand
{
   /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<XamlUICommand, ICommand>(nameof(Command));

    /// <summary>
    /// Defines the <see cref="Description"/> property
    /// </summary>
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<XamlUICommand, string>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        AvaloniaProperty.Register<XamlUICommand, IconSource>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="HotKey"/> property
    /// </summary>
    public static readonly StyledProperty<KeyGesture> HotKeyProperty =
        AvaloniaProperty.Register<XamlUICommand, KeyGesture>(nameof(HotKey));

    /// <summary>
    /// Defines the <see cref="Label"/> property
    /// </summary>
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<XamlUICommand, string>(nameof(Label));

    /// <summary>
    /// Gets or sets the command behavior of an interactive UI element that performs an action when invoked, 
    /// such as sending an email, deleting an item, or submitting a form.
    /// </summary>
    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a description for this element.
    /// </summary>
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets an IconSource for this element.
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a KeyGesture used to invoke this XamlUICommand
    /// </summary>
    public KeyGesture HotKey
    {
        get => GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the label for this element.
    /// </summary>
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Occurs whenever something happens that affects whether the command can execute.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Occurs when a CanExecute call is made.
    /// </summary>
    public event TypedEventHandler<XamlUICommand, CanExecuteRequestedEventArgs> CanExecuteRequested;

    /// <summary>
    /// Occurs when an Execute call is made.
    /// </summary>
    public event TypedEventHandler<XamlUICommand, ExecuteRequestedEventArgs> ExecuteRequested; 
    
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, null);

    public bool CanExecute(object param)
    {
        bool canExec = false;

        var args = new CanExecuteRequestedEventArgs(param);

        CanExecuteRequested?.Invoke(this, args);

        canExec = args.CanExecute;

        var command = Command;
        if (command != null)
        {
            bool canExecCommand = command.CanExecute(param);
            canExec = canExec && canExecCommand;
        }

        return canExec;
    }

    public void Execute(object param)
    {
        var args = new ExecuteRequestedEventArgs(param);

        ExecuteRequested?.Invoke(this, args);

        Command?.Execute(param);
    }
}
