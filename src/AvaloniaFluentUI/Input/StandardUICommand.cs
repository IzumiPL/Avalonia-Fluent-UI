using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Input;
using AvaloniaFluentUI.Locale;

namespace AvaloniaFluentUI.Controls.Input;

/// <summary>
/// Derives from XamlUICommand, adding a set of standard platform commands with pre-defined properties.
/// </summary>
public class StandardUICommand : XamlUICommand
{
    public StandardUICommand() { }

    public StandardUICommand(StandardUICommandKind kind)
    {
        Kind = kind;

        SetupCommand();

        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        SetupCommand();
    }

    /// <summary>
    /// Defines the <see cref="Kind"/> property
    /// </summary>
    public static readonly StyledProperty<StandardUICommandKind> KindProperty =
        AvaloniaProperty.Register<StandardUICommand, StandardUICommandKind>(nameof(Kind));

    /// <summary>
    /// Gets the platform command (with pre-defined properties such as icon, keyboard accelerator, 
    /// and description) that can be used with a StandardUICommand.
    /// </summary>
    public StandardUICommandKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == KindProperty)
        {
            SetupCommand();
        }
    }

    /// <summary>
    /// Called by TextCommandBarFlyout when the UI culture changes.
    /// </summary>
    internal void RefreshLocale()
    {
        SetupCommand();
    }

    private void SetupCommand()
    {
        switch (Kind)
        {
            case StandardUICommandKind.None:
                Label = string.Empty;
                IconSource = null;
                Description = string.Empty;
                HotKey = null;
                break;

            case StandardUICommandKind.Cut:
                Label = LocalizationService.Instance.GetString("Cut");
                IconSource = new SymbolIconSource { Symbol = Symbol.Cut };
                Description = LocalizationService.Instance.GetString("CutDescription");
                HotKey = new KeyGesture(Key.X, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Copy:
                Label = LocalizationService.Instance.GetString("Copy");
                IconSource = new SymbolIconSource { Symbol = Symbol.Copy };
                Description = LocalizationService.Instance.GetString("CopyDescription");
                HotKey = new KeyGesture(Key.C, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Paste:
                Label = LocalizationService.Instance.GetString("Paste");
                IconSource = new SymbolIconSource { Symbol = Symbol.Paste };
                Description  = LocalizationService.Instance.GetString("PasteDescription");
                HotKey = new KeyGesture(Key.V, KeyModifiers.Control);
                break;

            case StandardUICommandKind.SelectAll:
                Label  = LocalizationService.Instance.GetString("SelectAll");
                IconSource = new SymbolIconSource { Symbol = Symbol.SelectAll };
                Description  = LocalizationService.Instance.GetString("SelectAllDescription");
                HotKey = new KeyGesture(Key.A, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Delete:
                Label = LocalizationService.Instance.GetString("Delete");
                IconSource = new SymbolIconSource { Symbol = Symbol.Delete };
                Description  = LocalizationService.Instance.GetString("DeleteDescription");
                HotKey = new KeyGesture(Key.Delete);
                break;

            case StandardUICommandKind.Share:
                Label = LocalizationService.Instance.GetString("Share");
                IconSource = new SymbolIconSource { Symbol = Symbol.Share };
                Description  = LocalizationService.Instance.GetString("ShareDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Save:
                Label = LocalizationService.Instance.GetString("Save");
                IconSource = new SymbolIconSource { Symbol = Symbol.Save };
                Description  = LocalizationService.Instance.GetString("SaveDescription");
                HotKey = new KeyGesture(Key.S, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Open:
                Label = Description = LocalizationService.Instance.GetString("Open");
                IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                HotKey = new KeyGesture(Key.O, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Close:
                Label = Description = LocalizationService.Instance.GetString("Close");
                IconSource = new SymbolIconSource { Symbol = Symbol.Dismiss };
                HotKey = new KeyGesture(Key.W, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Pause:
                Label = Description = LocalizationService.Instance.GetString("Pause");
                IconSource = new SymbolIconSource { Symbol = Symbol.Pause };
                // No HotKey
                break;

            case StandardUICommandKind.Play:
                Label = Description = LocalizationService.Instance.GetString("Play");
                IconSource = new SymbolIconSource { Symbol = Symbol.Play };
                // No HotKey
                break;

            case StandardUICommandKind.Stop:
                Label  = Description = LocalizationService.Instance.GetString("Stop");
                IconSource = new SymbolIconSource { Symbol = Symbol.Stop };
                // No HotKey
                break;

            case StandardUICommandKind.Forward:
                Label  = LocalizationService.Instance.GetString("Forward");
                IconSource = new SymbolIconSource { Symbol = Symbol.Forward };
                Description =  LocalizationService.Instance.GetString("ForwardDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Backward:
                Label = LocalizationService.Instance.GetString("Backward");
                IconSource = new SymbolIconSource { Symbol = Symbol.Back };
                Description = LocalizationService.Instance.GetString("BackwardDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Undo:
                Label = LocalizationService.Instance.GetString("Undo");
                IconSource = new SymbolIconSource { Symbol = Symbol.Undo };
                Description = LocalizationService.Instance.GetString("UndoDescription");
                HotKey = new KeyGesture(Key.Z, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Redo:
                Label = LocalizationService.Instance.GetString("Redo");
                IconSource = new SymbolIconSource { Symbol = Symbol.Redo };
                Description = LocalizationService.Instance.GetString("RedoDescription");
                HotKey = new KeyGesture(Key.Y, KeyModifiers.Control);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
