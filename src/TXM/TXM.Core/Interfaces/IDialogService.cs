namespace TXM.Core.Interfaces;

public interface IDialogService
{
    public void RegisterDialog<TViewModel, TView>();
    bool? ShowDialog<TViewModel>(Action<object> callback, object vm);
}