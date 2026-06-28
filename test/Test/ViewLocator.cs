using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Test.ViewModels;
using Test.Views;

namespace Test;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _factory = new();

    public ViewLocator()
    {
        Register();
    }

    private void Register()
    {
        _factory[typeof(MainViewModel)] = () => new MainView();
        _factory[typeof(FlipViewModel)] = () => new FlipView();
        _factory[typeof(NaviViewModel)] = () => new NaviView();
        _factory[typeof(CardViewModel)] = () => new CardView();
        _factory[typeof(InfoBarViewModel)] = () => new InfoBarView();
        _factory[typeof(FlyoutViewModel)] = () => new FlyoutView();
        _factory[typeof(ComboBoxViewModel)] = () => new ComboBoxView();
    }

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        return _factory.TryGetValue(param.GetType(), out var creator)
            ? creator()
            : new TextBlock
            {
                Text = $"View not registered: {param.GetType().Name}"
            };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
