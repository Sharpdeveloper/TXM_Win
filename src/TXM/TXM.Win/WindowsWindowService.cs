using System;
using System.Collections.Generic;
using System.Windows;

using TXM.Core.Interfaces;

namespace TXM.Win;

public sealed class WindowsWindowService : IWindowService
{
    private WindowsWindowService() { }
    private static WindowsWindowService _instance = new WindowsWindowService();
    public static WindowsWindowService GetInstance() => _instance;
    
    private Dictionary<Type, Type> _mappings = new();

    public void RegisterWindow<TViewModel, TView>()
    {
        _mappings.Add(typeof(TViewModel), typeof(TView));
    }
    public IWindow Show<TViewModel>(Action<IWindow> callback, object vm)
    {
        var type = _mappings[typeof(TViewModel)];
        var window = (Window)Activator.CreateInstance(type)!;

        window.DataContext = vm;
        
        EventHandler closeEventHandler = null;
        closeEventHandler = (s, e) =>
        {
            callback((IWindow) window);
            window.Closed -= closeEventHandler;
        };
        window.Closed += closeEventHandler;
        
        window.Show();

        return (IWindow)window;
    }
}