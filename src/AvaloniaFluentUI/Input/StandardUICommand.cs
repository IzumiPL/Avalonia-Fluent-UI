using System;
using Avalonia;
using Avalonia.Input;
using AvaloniaFluentUI.Controls;

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
                Label = "剪切";
                IconSource = new SymbolIconSource { Symbol = Symbol.Cut };
                Description = "删除选定的内容并放到剪贴板上";
                HotKey = new KeyGesture(Key.X, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Copy:
                Label = "复制";
                IconSource = new SymbolIconSource { Symbol = Symbol.Copy };
                Description = "将选定的内容复制到剪贴板上";
                HotKey = new KeyGesture(Key.C, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Paste:
                Label = "粘贴";
                IconSource = new SymbolIconSource { Symbol = Symbol.Paste };
                Description = "将剪贴板内容插入当前位置";
                HotKey = new KeyGesture(Key.V, KeyModifiers.Control);
                break;

            case StandardUICommandKind.SelectAll:
                Label = "全部选择";
                IconSource = new SymbolIconSource { Symbol = Symbol.SelectAll };
                Description = "选择所有内容";
                HotKey = new KeyGesture(Key.A, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Delete:
                Label = "删除";
                IconSource = new SymbolIconSource { Symbol = Symbol.Delete };
                Description = "删除选中的内容";
                HotKey = new KeyGesture(Key.Delete);
                break;

            case StandardUICommandKind.Share:
                Label = "共享";
                IconSource = new SymbolIconSource { Symbol = Symbol.Share };
                Description = "分享精选内容";
                // No HotKey
                break;

            case StandardUICommandKind.Save:
                Label = "保存";
                IconSource = new SymbolIconSource { Symbol = Symbol.Save };
                Description = "保存你的更改";
                HotKey = new KeyGesture(Key.S, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Open:
                Label = Description = "打开";
                IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                HotKey = new KeyGesture(Key.O, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Close:
                Label = Description = "关闭";
                IconSource = new SymbolIconSource { Symbol = Symbol.Dismiss };
                HotKey = new KeyGesture(Key.W, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Pause:
                Label = Description = "暂停";
                IconSource = new SymbolIconSource { Symbol = Symbol.Pause };
                // No HotKey
                break;

            case StandardUICommandKind.Play:
                Label = Description = "播放";
                IconSource = new SymbolIconSource { Symbol = Symbol.Play };
                // No HotKey
                break;

            case StandardUICommandKind.Stop:
                Label = Description = "停止";
                IconSource = new SymbolIconSource { Symbol = Symbol.Stop };
                // No HotKey
                break;

            case StandardUICommandKind.Forward:
                Label = "前进";
                IconSource = new SymbolIconSource { Symbol = Symbol.Forward };
                Description = "进入下一个项目";
                // No HotKey
                break;

            case StandardUICommandKind.Backward:
                Label = "后退";
                IconSource = new SymbolIconSource { Symbol = Symbol.Back };
                Description = "返回";
                // No HotKey
                break;

            case StandardUICommandKind.Undo:
                Label = "撤销";
                IconSource = new SymbolIconSource { Symbol = Symbol.Undo };
                Description = "逆转最近的行动";
                HotKey = new KeyGesture(Key.Z, KeyModifiers.Control);
                break;

            case StandardUICommandKind.Redo:
                Label = "重来";
                IconSource = new SymbolIconSource { Symbol = Symbol.Redo };
                Description = "重复最近一次撤销的动作";
                HotKey = new KeyGesture(Key.Y, KeyModifiers.Control);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
