using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Input;
using AvaloniaFluentUI.Controls;
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
                // Label = "剪切";
                Label = LocalizationService.Instance.GetString("Cut");
                IconSource = new SymbolIconSource { Symbol = Symbol.Cut };
                // Description = "删除选定的内容并放到剪贴板上";
                Description = LocalizationService.Instance.GetString("CutDescription");
                HotKey = new KeyGesture(Key.X, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Copy:
                // Label = "复制";
                Label = LocalizationService.Instance.GetString("Copy");
                IconSource = new SymbolIconSource { Symbol = Symbol.Copy };
                // Description = "将选定的内容复制到剪贴板上";
                Description = LocalizationService.Instance.GetString("CopyDescription");
                HotKey = new KeyGesture(Key.C, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Paste:
                // Label = "粘贴";
                Label = LocalizationService.Instance.GetString("Paste");
                IconSource = new SymbolIconSource { Symbol = Symbol.Paste };
                // Description = "将剪贴板内容插入当前位置";
                Description  = LocalizationService.Instance.GetString("PasteDescription");
                HotKey = new KeyGesture(Key.V, KeyModifiers.Control);
                break;

            case StandardUICommandKind.SelectAll:
                // Label = "全部选择";
                Label  = LocalizationService.Instance.GetString("SelectAll");
                IconSource = new SymbolIconSource { Symbol = Symbol.SelectAll };
                // Description = "选择所有内容";
                Description  = LocalizationService.Instance.GetString("SelectAllDescription");
                HotKey = new KeyGesture(Key.A, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Delete:
                // Label = "删除";
                Label = LocalizationService.Instance.GetString("Delete");
                IconSource = new SymbolIconSource { Symbol = Symbol.Delete };
                // Description = "删除选中的内容";
                Description  = LocalizationService.Instance.GetString("DeleteDescription");
                HotKey = new KeyGesture(Key.Delete);
                break;

            case StandardUICommandKind.Share:
                // Label = "共享";
                Label = LocalizationService.Instance.GetString("Share");
                IconSource = new SymbolIconSource { Symbol = Symbol.Share };
                // Description = "分享精选内容";
                Description  = LocalizationService.Instance.GetString("ShareDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Save:
                // Label = "保存";
                Label = LocalizationService.Instance.GetString("Save");
                IconSource = new SymbolIconSource { Symbol = Symbol.Save };
                // Description = "保存你的更改";
                Description  = LocalizationService.Instance.GetString("SaveDescription");
                HotKey = new KeyGesture(Key.S, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Open:
                // Label = Description = "打开";
                Label = Description = LocalizationService.Instance.GetString("Open");
                IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                HotKey = new KeyGesture(Key.O, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Close:
                // Label = Description = "关闭";
                Label = Description = LocalizationService.Instance.GetString("Close");
                IconSource = new SymbolIconSource { Symbol = Symbol.Dismiss };
                HotKey = new KeyGesture(Key.W, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Pause:
                // Label = Description = "暂停";
                Label = Description = LocalizationService.Instance.GetString("Pause");
                IconSource = new SymbolIconSource { Symbol = Symbol.Pause };
                // No HotKey
                break;

            case StandardUICommandKind.Play:
                // Label = Description = "播放";
                Label = Description = LocalizationService.Instance.GetString("Play");
                IconSource = new SymbolIconSource { Symbol = Symbol.Play };
                // No HotKey
                break;

            case StandardUICommandKind.Stop:
                // Label = Description = "停止";
                Label  = Description = LocalizationService.Instance.GetString("Stop");
                IconSource = new SymbolIconSource { Symbol = Symbol.Stop };
                // No HotKey
                break;

            case StandardUICommandKind.Forward:
                // Label = "前进";
                Label  = LocalizationService.Instance.GetString("Forward");
                IconSource = new SymbolIconSource { Symbol = Symbol.Forward };
                // Description = "进入下一个项目";
                Description =  LocalizationService.Instance.GetString("ForwardDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Backward:
                // Label = "后退";
                Label = LocalizationService.Instance.GetString("Backward");
                IconSource = new SymbolIconSource { Symbol = Symbol.Back };
                // Description = "返回";
                Description = LocalizationService.Instance.GetString("BackwardDescription");
                // No HotKey
                break;

            case StandardUICommandKind.Undo:
                // Label = "撤销";
                Label = LocalizationService.Instance.GetString("Undo");
                IconSource = new SymbolIconSource { Symbol = Symbol.Undo };
                // Description = "撤销最近的行动";
                Description = LocalizationService.Instance.GetString("UndoDescription");
                HotKey = new KeyGesture(Key.Z, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Redo:
                // Label = "重来";
                Label = LocalizationService.Instance.GetString("Redo");
                IconSource = new SymbolIconSource { Symbol = Symbol.Redo };
                // Description = "重复最近一次撤销的动作";
                Description = LocalizationService.Instance.GetString("RedoDescription");
                HotKey = new KeyGesture(Key.Y, KeyModifiers.Control);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
