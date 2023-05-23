namespace TXM.Core.Interfaces;

public interface IWindowService
{
    public void RegisterWindow<TViewModel, TView>();
    IWindow Show<TViewModel>(Action<IWindow> callback, object vm);
}