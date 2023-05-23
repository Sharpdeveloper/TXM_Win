using System;
using System.Collections.Generic;
using System.Windows;

using TXM.Core.Interfaces;

namespace TXM.Win;

public sealed class WindowsDialogService : IDialogService
{
    private WindowsDialogService() { }
    private static WindowsDialogService _instance = new WindowsDialogService();
    public static WindowsDialogService GetInstance() => _instance;
    
    private Dictionary<Type, Type> _mappings = new();

    public void RegisterDialog<TViewModel, TView>()
    {
        _mappings.Add(typeof(TViewModel), typeof(TView));
    }
    public bool? ShowDialog<TViewModel>(Action<object?> callback, object vm)
    {
        var type = _mappings[typeof(TViewModel)];
        var vmType = typeof(TViewModel);
        var dialog = (Window)Activator.CreateInstance(type);

        dialog.DataContext = vm;
        
        EventHandler closeEventHandler = null;
        closeEventHandler = (s, e) =>
        {
            callback(dialog.DataContext);
            dialog.Closed -= closeEventHandler;
        };
        dialog.Closed += closeEventHandler;
        
        return dialog.ShowDialog();
    }
}