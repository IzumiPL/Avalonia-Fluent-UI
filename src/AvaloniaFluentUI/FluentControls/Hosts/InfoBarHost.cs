using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class InfoBarHost : Canvas
{
    private readonly Dictionary<Type, IInfoBarManager> _managers = new();

    public InfoBarHost()
    {
        ZIndex = int.MaxValue;
        IsHitTestVisible = true;
    }
    
    public void RegisterManager<T>() where T : IInfoBarManager, new()
    {
        var manager = new T();
        if (_managers.ContainsKey(typeof(T))) throw new InvalidOperationException($"Manager '{typeof(T).Name}' has already been registered.");
        
        manager.SetHost(this);
        _managers.Add(typeof(T), manager);
    }

    public T GetManager<T>() where T : IInfoBarManager
    {
        return (T)_managers[typeof(T)];
    }
}
