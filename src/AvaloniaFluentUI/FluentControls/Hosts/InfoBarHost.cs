using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class InfoBarHost : Canvas
{
    private Dictionary<Type, object> _managers = new Dictionary<Type, object>();

    public InfoBarHost()
    {
        ZIndex = 999;
        IsHitTestVisible = true;
    }

    public void RegisterManager<T>(T manager)
    {
        _managers.Add(typeof(T), manager);

        if (manager is IInfoBarManager im)
        {
            im.SetHost(this);
        }
    }

    public T GetManager<T>()
    {
        return (T)_managers[typeof(T)];
    }
}
